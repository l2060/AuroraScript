using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// AuroraScript 运行时虚拟机，负责执行字节码并管理运行时环境
    /// 作为脚本引擎的核心组件，实现了字节码的解释执行和运行时环境的管理
    /// </summary>
    internal unsafe class RuntimeVM
    {
        private static readonly delegate*<RuntimeVM, ExecuteContext, ref CallFrame, OpCode, bool>[] _opDispatch;

        static RuntimeVM()
        {
            var maxOp = Enum.GetValues(typeof(OpCode)).Cast<byte>().Max();
            _opDispatch = new delegate*<RuntimeVM, ExecuteContext, ref CallFrame, OpCode, bool>[maxOp + 1];
            RegisterHandler(OpCode.ALLOC_LOCALS, &HandleAllocLocals);
            RegisterHandler(OpCode.LOAD_LOCAL, &HandleLoadLocal);
            RegisterHandler(OpCode.STORE_LOCAL, &HandleStoreLocal);
            RegisterHandler(OpCode.LOAD_LOCAL_L, &HandleLoadLocalLong);
            RegisterHandler(OpCode.STORE_LOCAL_L, &HandleStoreLocalLong);

            RegisterHandler(OpCode.LOAD_CAPTURE, &HandleLoadCapture);
            RegisterHandler(OpCode.STORE_CAPTURE, &HandleStoreCapture);
            RegisterHandler(OpCode.SUBTRACT, &HandleSubtract);
            RegisterHandler(OpCode.MULTIPLY, &HandleMultiply);
            RegisterHandler(OpCode.DIVIDE, &HandleDivide);
            RegisterHandler(OpCode.MOD, &HandleMod);
            RegisterHandler(OpCode.NEGATE, &HandleNegate);
            RegisterHandler(OpCode.INCREMENT, &HandleIncrement);
            RegisterHandler(OpCode.DECREMENT, &HandleDecrement);
            RegisterHandler(OpCode.BIT_NOT, &HandleBitNot);
            RegisterHandler(OpCode.BIT_SHIFT_L, &HandleBitShiftLeft);
            RegisterHandler(OpCode.BIT_SHIFT_R, &HandleBitShiftRight);
            RegisterHandler(OpCode.BIT_USHIFT_R, &HandleBitUnsignedShiftRight);
            RegisterHandler(OpCode.BIT_AND, &HandleBitAnd);
            RegisterHandler(OpCode.BIT_OR, &HandleBitOr);
            RegisterHandler(OpCode.BIT_XOR, &HandleBitXor);
            RegisterHandler(OpCode.LOGIC_NOT, &HandleLogicNot);
            RegisterHandler(OpCode.LOGIC_AND, &HandleLogicAnd);
            RegisterHandler(OpCode.LOGIC_OR, &HandleLogicOr);
            RegisterHandler(OpCode.EQUAL, &HandleEqual);
            RegisterHandler(OpCode.NOT_EQUAL, &HandleNotEqual);
            RegisterHandler(OpCode.LESS_THAN, &HandleLessThan);
            RegisterHandler(OpCode.LESS_EQUAL, &HandleLessEqual);
            RegisterHandler(OpCode.GREATER_THAN, &HandleGreaterThan);
            RegisterHandler(OpCode.GREATER_EQUAL, &HandleGreaterEqual);
            RegisterHandler(OpCode.JUMP, &HandleJump);
            RegisterHandler(OpCode.JUMP_IF_FALSE, &HandleJumpIfFalse);
            RegisterHandler(OpCode.JUMP_IF_TRUE, &HandleJumpIfTrue);
        }

        private static void RegisterHandler(OpCode opCode, delegate*<RuntimeVM, ExecuteContext, ref CallFrame, OpCode, bool> handler)
        {
            _opDispatch[(Int32)opCode] = handler;
        }

        private enum UnaryNumberOp : byte
        {
            Negate,
            Increment,
            Decrement,
            BitNot
        }

        private enum BinaryNumberOp : byte
        {
            Subtract,
            Multiply,
            Divide,
            Mod
        }

        private enum BinaryPredicateOp : byte
        {
            LessThan,
            LessEqual,
            GreaterThan,
            GreaterEqual
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ApplyUnaryOp(UnaryNumberOp op, double value)
        {
            switch (op)
            {
                case UnaryNumberOp.Negate:
                    return -value;
                case UnaryNumberOp.Increment:
                    return value + 1d;
                case UnaryNumberOp.Decrement:
                    return value - 1d;
                case UnaryNumberOp.BitNot:
                    return ~(long)value;
                default:
                    return value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double ApplyBinaryOp(BinaryNumberOp op, double left, double right)
        {
            switch (op)
            {
                case BinaryNumberOp.Subtract:
                    return left - right;
                case BinaryNumberOp.Multiply:
                    return left * right;
                case BinaryNumberOp.Divide:
                    return left / right;
                case BinaryNumberOp.Mod:
                    return left % right;
                default:
                    return double.NaN;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ApplyPredicate(BinaryPredicateOp op, double left, double right)
        {
            switch (op)
            {
                case BinaryPredicateOp.LessThan:
                    return left < right;
                case BinaryPredicateOp.LessEqual:
                    return left <= right;
                case BinaryPredicateOp.GreaterThan:
                    return left > right;
                case BinaryPredicateOp.GreaterEqual:
                    return left >= right;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetNumber(in ScriptDatum datum, out double value)
        {
            if (datum.Kind == ValueKind.Number)
            {
                value = datum.Number;
                return true;
            }
            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetBinaryNumbers(in ScriptDatum left, in ScriptDatum right, out double leftNumber, out double rightNumber)
        {
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                leftNumber = left.Number;
                rightNumber = right.Number;
                return true;
            }
            leftNumber = rightNumber = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Boolean DatumEquals(ScriptDatum leftDatum, ScriptDatum rightDatum)
        {
            if (leftDatum.Kind == rightDatum.Kind)
            {
                switch (leftDatum.Kind)
                {
                    case ValueKind.Null:
                        return true;
                    case ValueKind.Boolean:
                        return leftDatum.Boolean == rightDatum.Boolean;
                    case ValueKind.Number:
                        return leftDatum.Number == rightDatum.Number;
                    case ValueKind.String:
                        return String.Equals(leftDatum.String?.Value, rightDatum.String?.Value, StringComparison.Ordinal);
                    case ValueKind.Object:
                        return Equals(leftDatum.Object, rightDatum.Object);
                }
            }

            if (leftDatum.Kind == ValueKind.Null || rightDatum.Kind == ValueKind.Null)
            {
                return leftDatum.Kind == ValueKind.Null && rightDatum.Kind == ValueKind.Null;
            }

            var leftObj = leftDatum.ToObject();
            var rightObj = rightDatum.ToObject();
            return leftObj?.Equals(rightObj) ?? rightObj == null;
        }

        /// <summary>
        /// 字符串常量池，存储脚本中使用的所有字符串常量
        /// 通过索引快速访问字符串值，避免重复创建相同的字符串对象
        /// </summary>
        private readonly ImmutableArray<StringValue> _stringConstants;

        /// <summary>
        /// 当前执行的字节码缓冲区
        /// 包含编译后的指令序列，由虚拟机解释执行
        /// </summary>
        private readonly ByteCodeBuffer _codeBuffer;

        /// <summary>
        /// 调试符号信息，包含脚本的调试信息，如行号、函数名等
        /// </summary>
        private readonly DebugSymbolInfo _debugSymbols;

        private readonly ClrTypeRegistry _clrRegistry;

        private readonly AuroraEngine _engine;

        public readonly long[] _opCounts = new long[255];
        public readonly long[] _opTicks = new long[255];


        public PatchVM PatchVM()
        {
            return new PatchVM(_codeBuffer);
        }


        internal DebugSymbol ResolveSymbol(Int32 pointer)
        {
            return _debugSymbols.Resolve(pointer);
        }

        internal ModuleSymbol ResolveModule(Int32 pointer)
        {
            return _debugSymbols.ResolveModule(pointer);
        }


        public void PrintOpCounts(int maxEntries = 0)
        {
            var length = Math.Min(_opCounts.Length, _opDispatch.Length);
            IEnumerable<(int Index, long Count)> ordered = Enumerable.Range(0, length)
                .Select(i => (Index: i, Count: _opCounts[i]))
                .Where(item => item.Count > 0)
                .OrderByDescending(item => item.Count);

            if (maxEntries > 0)
            {
                ordered = ordered.Take(maxEntries);
            }

            foreach (var (index, count) in ordered)
            {
                var code = (OpCode)index;
                Console.WriteLine($"OPCODE {code,-20} : {count,-20}   useTicks: {_opTicks[index],-20}");
            }
        }



        /// <summary>
        /// 使用指定的字节码和字符串常量池初始化虚拟机
        /// </summary>
        /// <param name="bytecode">要执行的字节码，由编译器生成的二进制指令序列</param>
        /// <param name="stringConstants">字符串常量池，包含脚本中所有的字符串字面量</param>

        public RuntimeVM(AuroraEngine engine, byte[] bytecode, ImmutableArray<String> stringConstants, DebugSymbolInfo debugSymbols)
        {
            _engine = engine;
            // 创建字节码缓冲区，用于读取和解析字节码指令
            _codeBuffer = new ByteCodeBuffer(bytecode);
            // 将字符串常量转换为StringValue对象并存储在不可变数组中
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
            // 调试符号信息
            _debugSymbols = debugSymbols;
            _clrRegistry = engine.ClrRegistry;
        }


        /// <summary>
        /// 执行脚本代码
        /// </summary>
        /// <param name="exeContext">执行上下文，包含操作数栈、调用栈和全局环境</param>
        public void Execute(ExecuteContext exeContext)
        {
            try
            {
                // 开始执行调用帧
                ExecuteFrame(exeContext);
            }
            catch (Exception ex)
            {
                // 捕获执行过程中的异常，并设置执行状态为错误
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, ex);
            }
        }


        /// <summary>
        /// 执行当前调用帧中的指令
        /// 这是虚拟机的核心方法，实现了字节码的解释执行
        /// </summary>
        /// <param name="exeContext">执行上下文，包含操作数栈、调用栈和全局环境</param>
        private void ExecuteFrame(ExecuteContext exeContext)
        {
            // 设置执行状态为运行中
            exeContext.SetStatus(ExecuteStatus.Running, ScriptObject.Null, null);

            // 获取调用栈和操作数栈的引用，提高访问效率
            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;

            // 临时变量，用于存储指令执行过程中的中间值
            Int32 propNameIndex = 0;
            StringValue propName = null;
            ScriptDatum datumValue;
            ScriptDatum datumLeft;
            ScriptDatum datumRight;
            ScriptObject value = null;
            ScriptObject obj = null;



            // 获取当前调用帧
            var frame = _callStack.Peek();
            ref ScriptDatum[] _locals = ref frame._locals;
            // 获取当前域的全局对象
            ScriptGlobal domainGlobal = frame.Domain.Global;

            Func<ScriptDatum> PopDatum = _operandStack.PopDatum;
            Action<ScriptDatum> PushDatum = _operandStack.PushDatum;
            Func<ScriptObject> PopObject = _operandStack.Pop;
            Action<ScriptObject> PushObject = _operandStack.Push;


        
            // 主执行循环，不断读取并执行指令，直到遇到返回指令或发生异常
            while (exeContext.Status == ExecuteStatus.Running)
            {
                // 从当前指令指针位置读取操作码
                var opCode = _codeBuffer.ReadOpCode(frame);
                var opIndex = (Int32)opCode;
                _opCounts[opIndex]++;

      
                var start = Stopwatch.GetTimestamp();

                delegate*<RuntimeVM, ExecuteContext, ref CallFrame, OpCode, bool> handler = _opDispatch[opIndex];
                if (handler != null)
                {
                    handler(this, exeContext, ref frame, opCode);
 
                }

                // 根据操作码执行相应的操作
                // 这是虚拟机的指令分派表，每个case对应一种字节码指令
                switch (opCode)
                {
                    // 基本栈操作
                    case OpCode.NOP:
                        // 空操作
                        break;

                    case OpCode.POP:
                        // 弹出栈顶元素
                        PopDatum();
                        break;

                    case OpCode.DUP:
                        // 复制栈顶元素
                        _operandStack.Duplicate();
                        //var topDatum = PeekDatum();
                        //PushDatum(topDatum);
                        break;

                    case OpCode.SWAP:
                        // 交换栈顶两个元素
                        _operandStack.Swap();
                        break;

                    case OpCode.LOAD_ARG:
                        //
                        var argIndex = _codeBuffer.ReadByte(frame);
                        var argDatum = frame.GetArgumentDatum(argIndex);
                        PushDatum(argDatum);
                        break;

                    case OpCode.TRY_LOAD_ARG:
                        propNameIndex = _codeBuffer.ReadByte(frame);
                        if (frame.TryGetArgumentDatum(propNameIndex, out var tryArgDatum))
                        {
                            PopDatum();
                            PushDatum(tryArgDatum);
                        }
                        break;

                    case OpCode.PUSH_I8:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadSByte(frame)));
                        break;

                    case OpCode.PUSH_I16:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadInt16(frame)));
                        break;

                    case OpCode.PUSH_I32:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadInt32(frame)));
                        break;

                    case OpCode.PUSH_I64:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadInt64(frame)));
                        break;
                    case OpCode.PUSH_F32:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadFloat(frame)));
                        break;

                    case OpCode.PUSH_F64:
                        PushDatum(ScriptDatum.FromNumber(_codeBuffer.ReadDouble(frame)));
                        break;

                    case OpCode.PUSH_STRING:
                        var stringIndex = _codeBuffer.ReadInt32(frame);
                        PushDatum(ScriptDatum.FromString(_stringConstants[stringIndex]));
                        break;


                    case OpCode.LOAD_LOCAL:
                    case OpCode.STORE_LOCAL:
                    case OpCode.LOAD_CAPTURE:
                    case OpCode.STORE_CAPTURE:
                        // handled by delegate* jump table fast path
                        break;

                    case OpCode.CREATE_CLOSURE:
                        var closureOffset = _codeBuffer.ReadInt32(frame);
                        var captureCount = _codeBuffer.ReadByte(frame);
                        var entryPointer = frame.Pointer + closureOffset;

                        var moduleObject = PopObject();
                        var moduleForClosure = moduleObject as ScriptModule;

                        ClosureUpvalue[] capturedUpvalues;
                        if (captureCount == 0)
                        {
                            capturedUpvalues = Array.Empty<ClosureUpvalue>();
                        }
                        else
                        {
                            capturedUpvalues = new ClosureUpvalue[captureCount];
                            for (int i = captureCount - 1; i >= 0; i--)
                            {
                                var upvalueObj = PopObject();
                                if (upvalueObj is not Upvalue upvalue)
                                {
                                    throw new AuroraVMException("Invalid captured upvalue");
                                }
                                var aliasSlot = upvalue.ConsumeAliasSlot();
                                capturedUpvalues[i] = new ClosureUpvalue(aliasSlot, upvalue);
                            }
                        }

                        var closure = new ClosureFunction(frame.Domain, moduleForClosure, entryPointer, capturedUpvalues);
                        PushObject(closure);
                        break;

                    case OpCode.CAPTURE_VAR:
                        var slotIndex = _codeBuffer.ReadInt32(frame);
                        var capturedUpvalue = frame.GetCapturedUpvalue(slotIndex) ?? frame.GetOrCreateUpvalue(slotIndex);
                        capturedUpvalue.MarkAliasSlot(slotIndex);
                        PushObject(capturedUpvalue);
                        break;

                    case OpCode.INIT_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        var module = new ScriptModule(propName.Value);
                        domainGlobal.Define("@" + propName.Value, module, writeable: false, enumerable: true);
                        break;

                    case OpCode.NEW_MAP:
                        PushObject(new ScriptObject());
                        break;

                    case OpCode.NEW_ARRAY:
                        var count = _codeBuffer.ReadInt32(frame);
                        var newArray = new ScriptArray(count);
                        var datumBuffer = new ScriptDatum[count];
                        for (int i = count - 1; i >= 0; i--)
                        {
                            datumBuffer[i] = PopDatum();
                        }
                        int index = 0;
                        for (int i = 0; i < count; i++)
                        {
                            if (datumBuffer[i].Kind.Include(ValueKind.Object) && datumBuffer[i].Object is ScriptDeConstruct deConstruct)
                            {
                                if (deConstruct.Kind == ValueKind.Array && deConstruct.Object is ScriptArray array1)
                                {
                                    for (int n = 0; n < array1.Length; n++)
                                    {
                                        newArray.Set(index, array1.Get(n));
                                        index++;
                                    }
                                }
                            }
                            else
                            {
                                newArray.Set(index, datumBuffer[i]);
                                index++;
                            }

                        }
                        PushObject(newArray);
                        break;

                    case OpCode.GET_ITERATOR:
                        obj = PopObject();
                        if (obj is IEnumerator iterable)
                        {
                            PushObject(iterable.GetIterator());
                        }
                        else
                        {
                            throw new AuroraVMException($"Object {obj} does not support iterators.");
                        }
                        break;

                    case OpCode.ITERATOR_VALUE:
                        obj = PopObject();
                        if (obj is ItemIterator iterator)
                        {
                            PushDatum(iterator.Value());
                        }
                        else
                        {
                            throw new AuroraVMException($"Object {obj} not iterator.");
                        }
                        break;

                    case OpCode.ITERATOR_HAS_VALUE:
                        obj = PopObject();
                        if (obj is ItemIterator iterator2)
                        {
                            PushDatum(ScriptDatum.FromBoolean(iterator2.HasValue()));
                        }
                        else
                        {
                            throw new AuroraVMException($"Object {obj} not iterator.");
                        }
                        break;

                    case OpCode.ITERATOR_NEXT:
                        obj = PopObject();
                        if (obj is ItemIterator iterator3)
                        {
                            iterator3.Next();
                        }
                        else
                        {
                            throw new AuroraVMException($"Object {obj} not iterator.");
                        }
                        break;
                    case OpCode.GET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        obj = PopObject();

                        if (obj is ScriptObject scriptObj)
                        {
                            PushObject(scriptObj.GetPropertyValue(propName.Value));
                        }
                        else
                        {
                            throw new AuroraVMException($"Cannot get property '{propName}' from {obj}");
                        }
                        break;

                    case OpCode.SET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = PopObject();
                        obj = PopObject();

                        if (obj is ScriptObject targetScriptObj)
                        {
                            targetScriptObj.SetPropertyValue(propName, value);
                        }
                        else
                        {
                            throw new AuroraVMException($"Cannot set property '{propName}' on {obj}");
                        }
                        break;

                    case OpCode.DELETE_PROPERTY:
                        value = PopObject();
                        obj = PopObject();
                        obj.DeletePropertyValue(value.ToString());
                        break;
                    case OpCode.GET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = frame.Module.GetPropertyValue(propName.Value);
                        PushObject(value);
                        break;

                    case OpCode.SET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = PopObject();
                        frame.Module.SetPropertyValue(propName, value);
                        break;

                    case OpCode.GET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        if (_clrRegistry.TryGetClrType(propName.Value, out var clrType))
                        {
                            value = clrType;
                        }
                        else
                        {
                            value = domainGlobal.GetPropertyValue(propName.Value);
                        }
                        PushObject(value);
                        break;

                    case OpCode.SET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = PopObject();
                        domainGlobal.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_ELEMENT:
                        datumValue = PopDatum();
                        var datumObjValue = PopDatum();
                        if (datumObjValue.TryGetArray(out var scriptArray) && datumValue.Kind == ValueKind.Number)
                        {
                            PushDatum(scriptArray.Get((Int32)datumValue.Number));
                        }
                        else if (datumObjValue.Kind.Include(ValueKind.Object))
                        {
                            var key = ExtractPropertyKey(ref datumValue);
                            PushObject(datumObjValue.Object.GetPropertyValue(key));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNull());
                        }
                        break;

                    case OpCode.SET_ELEMENT:
                        var datumTargetObj = PopDatum();
                        datumValue = PopDatum();
                        var datumAssignedValue = PopDatum();
                        if (datumTargetObj.TryGetArray(out scriptArray) && datumValue.Kind == ValueKind.Number)
                        {
                            scriptArray.Set((Int32)datumValue.Number, datumAssignedValue);
                        }
                        else if (datumTargetObj.Kind.Include(ValueKind.Object))
                        {
                            var key = ExtractPropertyKey(ref datumValue);
                            datumTargetObj.Object.SetPropertyValue(key, datumAssignedValue.ToObject());
                        }
                        break;

                    case OpCode.LOGIC_NOT:
                    case OpCode.LOGIC_AND:
                    case OpCode.LOGIC_OR:
                    case OpCode.EQUAL:
                    case OpCode.NOT_EQUAL:
                    case OpCode.LESS_THAN:
                    case OpCode.LESS_EQUAL:
                    case OpCode.GREATER_THAN:
                    case OpCode.GREATER_EQUAL:
                        // handled by delegate* jump table fast path
                        break;

                    case OpCode.ADD:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            PushDatum(ScriptDatum.FromNumber(datumLeft.Number + datumRight.Number));
                        }
                        else
                        {
                            var result = datumLeft.ToString() + datumRight.ToString();
                            PushDatum(ScriptDatum.FromString(StringValue.Of(result)));
                        }
                        break;
                    case OpCode.TYPEOF:
                        datumRight = PopDatum();
                        PushDatum(datumRight.TypeOf());
                        break;
                    case OpCode.ALLOC_LOCALS:
                    case OpCode.JUMP:
                    case OpCode.JUMP_IF_FALSE:
                    case OpCode.JUMP_IF_TRUE:
                        // handled by delegate* jump table fast path
                        break;
                    case OpCode.CALL:
                        // 函数调用指令
                        // 从栈顶弹出可调用对象
                        var callable = PopDatum();
                        // 读取参数数量
                        var argCount = _codeBuffer.ReadByte(frame);
                        // 创建参数数组
                        var argDatums = new ScriptDatum[argCount];
                        // 从栈中弹出参数，注意参数顺序是从右到左
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            argDatums[i] = PopDatum();
                        }
                        if (callable.Kind == ValueKind.Function && callable.Object is ClosureFunction closureFunc)
                        {
                            if (_callStack.Count > exeContext.ExecuteOptions.MaxCallStackDepth)
                            {
                                throw new AuroraVMException("The number of method call stacks exceeds the limit of " + exeContext.ExecuteOptions.MaxCallStackDepth);
                            }
                            // 如果是脚本中定义的闭包函数
                            // 创建新的调用帧，包含环境、全局对象、模块和入口点
                            var callFrame = CallFramePool.Rent(frame.Domain, closureFunc.Module, closureFunc.EntryPointer, argDatums, closureFunc.CapturedUpvalues);
                            // 将新帧压入调用栈
                            _callStack.Push(callFrame);
                            // 更新当前帧引用
                            frame = callFrame;
                        }
                        else if (callable.Kind == ValueKind.ClrBonding && callable.Object is Callable callableFunc)
                        {
                            var callResult = callableFunc.Invoke(exeContext, null, argDatums);
                            PushObject(callResult);
                        }
                        else if ((callable.Kind == ValueKind.ClrType || callable.Kind == ValueKind.ClrFunction) && callable.Object is IClrInvokable clrInvokable)
                        {
                            var callResult = clrInvokable.Invoke(exeContext, callable.ToObject(), argDatums);
                            PushDatum(callResult);
                        }
                        else
                        {
                            // 如果不是可调用对象，抛出异常
                            throw new InvalidOperationException($"Cannot call {callable.Kind}");
                        }
                        break;

                    case OpCode.RETURN:
                        // 函数返回指令
                        // 获取返回值（如果有）
                        datumValue = _operandStack.Count > 0 ? PopDatum() : ScriptDatum.FromNull();
                        value = datumValue.ToObject();
                        // 弹出当前调用帧
                        var finishedFrame = _callStack.Pop();
                        CallFramePool.Return(finishedFrame);

                        // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
                        if (_callStack.Count == 0)
                        {
                            // 设置执行状态为完成，并返回最终结果
                            exeContext.SetStatus(ExecuteStatus.Complete, value, null);
                            return;
                        }

                        // 如果调用栈不为空，说明是从子函数返回到调用者
                        // 将返回值压入操作数栈，供调用者使用
                        PushDatum(datumValue);
                        // 切换到调用者的帧继续执行
                        frame = _callStack.Peek();
                        break;

                    case OpCode.RETURN_NULL:

                        // 弹出当前调用帧
                        CallFramePool.Return(_callStack.Pop());
                        // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
                        if (_callStack.Count == 0)
                        {
                            // 设置执行状态为完成，并返回最终结果
                            exeContext.SetStatus(ExecuteStatus.Complete, value, null);
                            return;
                        }
                        // 如果调用栈不为空，说明是从子函数返回到调用者
                        // 将返回值压入操作数栈，供调用者使用
                        PushDatum(ScriptDatum.FromNull());
                        // 切换到调用者的帧继续执行
                        frame = _callStack.Peek();
                        break;
                    case OpCode.YIELD:
                        // TODO
                        if (exeContext.ExecuteOptions.EnabledYield)
                        {
                            exeContext.SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
                        }
                        break;
                    case OpCode.PUSH_0:
                        PushDatum(ScriptDatum.FromNumber(0));
                        break;
                    case OpCode.PUSH_1:
                        PushDatum(ScriptDatum.FromNumber(1));
                        break;
                    case OpCode.PUSH_2:
                        PushDatum(ScriptDatum.FromNumber(2));
                        break;
                    case OpCode.PUSH_3:
                        PushDatum(ScriptDatum.FromNumber(3));
                        break;
                    case OpCode.PUSH_4:
                        PushDatum(ScriptDatum.FromNumber(4));
                        break;
                    case OpCode.PUSH_5:
                        PushDatum(ScriptDatum.FromNumber(5));
                        break;
                    case OpCode.PUSH_6:
                        PushDatum(ScriptDatum.FromNumber(6));
                        break;
                    case OpCode.PUSH_7:
                        PushDatum(ScriptDatum.FromNumber(7));
                        break;
                    case OpCode.PUSH_8:
                        PushDatum(ScriptDatum.FromNumber(8));
                        break;
                    case OpCode.PUSH_9:
                        PushDatum(ScriptDatum.FromNumber(9));
                        break;
                    case OpCode.PUSH_NULL:
                        PushDatum(ScriptDatum.FromNull());
                        break;
                    case OpCode.PUSH_FALSE:
                        PushDatum(ScriptDatum.FromBoolean(false));
                        break;
                    case OpCode.PUSH_TRUE:
                        PushDatum(ScriptDatum.FromBoolean(true));
                        break;
                    case OpCode.PUSH_THIS:
                        PushObject(frame.Module);
                        break;
                    case OpCode.PUSH_GLOBAL:
                        PushObject(domainGlobal);
                        break;
                    case OpCode.PUSH_CONTEXT:
                        var value5 = exeContext.UserState;
                        if (ClrTypeResolver.ResolveType(value5.GetType(), out var descriptor))
                        {
                            PushObject(new ClrInstanceObject(descriptor, value5));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNull());
                        }
                        break;
                    case OpCode.PUSH_ARGUMENTS:
                        argDatum = ScriptDatum.FromArray(new ScriptArray(frame.Arguments));
                        PushDatum(argDatum);
                        break;
                    case OpCode.NEW_REGEX:
                        var _pattern = _codeBuffer.ReadInt32(frame);
                        var _flags = _codeBuffer.ReadInt32(frame);
                        var flags = _stringConstants[_flags];
                        var pattern = _stringConstants[_pattern];
                        var regex = RegexManager.Resolve(pattern.Value, flags.Value);
                        PushDatum(ScriptDatum.FromRegex(regex));
                        break;

                    case OpCode.DECONSTRUCT_ARRAY:
                        datumValue = PopDatum();
                        if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array)
                        {
                            var deConstruct = new ScriptDeConstruct(array, ValueKind.Array);
                            PushObject(deConstruct);
                        }
                        break;

                    case OpCode.DECONSTRUCT_MAP:
                        datumValue = PopDatum();
                        value = PopObject();
                        if (datumValue.Kind.Include(ValueKind.Object))
                        {
                            value.CopyPropertysFrom(datumValue.Object, true);
                        }
                        else if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array1)
                        {
                            for (int i = 0; i < array1.Length; i++)
                            {
                                var ele = array1.Get(i);
                                value.SetPropertyValue(i.ToString(), ele.ToObject());
                            }
                        }
                        break;

                }

                var end = Stopwatch.GetTimestamp();

                _opTicks[opIndex] += (end - start);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static String ExtractPropertyKey(ref ScriptDatum keyDatum)
        {
            switch (keyDatum.Kind)
            {
                case ValueKind.String:
                    return keyDatum.String?.Value ?? String.Empty;
                case ValueKind.Number:
                    return keyDatum.Number.ToString(CultureInfo.InvariantCulture);
                case ValueKind.Boolean:
                    return keyDatum.Boolean ? "true" : "false";
                case ValueKind.Null:
                    return ScriptObject.Null.ToString();
                default:
                    return keyDatum.ToString();
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteUnaryNumberOp(ExecuteContext exeContext, UnaryNumberOp operation, double defaultValue)
        {
            var stack = exeContext._operandStack;
            ref var slot = ref stack.PeekRef();
            if (TryGetNumber(in slot, out var value))
            {
                slot = ScriptDatum.FromNumber(ApplyUnaryOp(operation, value));
            }
            else
            {
                slot = ScriptDatum.FromNumber(defaultValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteBinaryNumberOp(ExecuteContext exeContext, BinaryNumberOp operation, double defaultValue)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                result = ScriptDatum.FromNumber(ApplyBinaryOp(operation, leftNumber, rightNumber));
            }
            else
            {
                result = ScriptDatum.FromNumber(defaultValue);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteBinaryPredicate(ExecuteContext exeContext, BinaryPredicateOp predicate)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber)
                ? ApplyPredicate(predicate, leftNumber, rightNumber)
                : false;
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteLogicNot(ExecuteContext exeContext)
        {
            var stack = exeContext._operandStack;
            ref var slot = ref stack.PeekRef();
            slot = ScriptDatum.FromBoolean(!slot.IsTrue());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteLogicalBinary(ExecuteContext exeContext, Boolean isAnd)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = isAnd
                ? (leftSlot.IsTrue() && rightSlot.IsTrue())
                : (leftSlot.IsTrue() || rightSlot.IsTrue());
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ExecuteEquality(ExecuteContext exeContext, Boolean negate)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var equals = DatumEquals(leftSlot, rightSlot);
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(negate ? !equals : equals);
        }

        private static bool HandleAllocLocals(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localsRequested = vm._codeBuffer.ReadInt32(frame);
            frame.EnsureLocalStorage(localsRequested);
            return true;
        }

        private static bool HandleLoadLocal(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadByte(frame);
            exeContext._operandStack.PushDatum(frame.GetLocalDatum(localIndex));
            return true;
        }

        private static bool HandleStoreLocal(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var value = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadByte(frame);
            frame.SetLocalDatum(localIndex, value);
            return true;
        }


        private static bool HandleLoadLocalLong(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            exeContext._operandStack.PushDatum(frame.GetLocalDatum(localIndex));
            return true;
        }

        private static bool HandleStoreLocalLong(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var value = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            frame.SetLocalDatum(localIndex, value);
            return true;
        }



        private static bool HandleLoadCapture(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var datumValue = frame.GetLocalDatum(localIndex);
            if (datumValue.Kind != ValueKind.Object || datumValue.Object is not Upvalue upvalueToRead)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            exeContext._operandStack.PushDatum(upvalueToRead.Get());
            return true;
        }

        private static bool HandleStoreCapture(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var datumValue = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var upvalueDatum = frame.GetLocalDatum(localIndex);
            if (upvalueDatum.Kind != ValueKind.Object || upvalueDatum.Object is not Upvalue upvalueToWrite)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            upvalueToWrite.Set(datumValue);
            return true;
        }

        private static bool HandleSubtract(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Subtract, double.NaN);
            return true;
        }

        private static bool HandleMultiply(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Multiply, double.NaN);
            return true;
        }

        private static bool HandleDivide(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Divide, double.NaN);
            return true;
        }

        private static bool HandleMod(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Mod, double.NaN);
            return true;
        }

        private static bool HandleNegate(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Negate, double.NaN);
            return true;
        }

        private static bool HandleIncrement(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Increment, double.NaN);
            return true;
        }

        private static bool HandleDecrement(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Decrement, double.NaN);
            return true;
        }

        private static bool HandleBitNot(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.BitNot, -1);
            return true;
        }

        private static bool HandleBitShiftLeft(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var result = (double)((int)left.Number << (int)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber(result));
            }
            else
            {
                stack.PushDatum(ScriptDatum.FromNumber(double.NaN));
            }
            return true;
        }

        private static bool HandleBitShiftRight(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var result = (double)((int)left.Number >> (int)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber(result));
            }
            else
            {
                stack.PushDatum(ScriptDatum.FromNumber(double.NaN));
            }
            return true;
        }

        private static bool HandleBitUnsignedShiftRight(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var result = (double)((int)left.Number >>> (int)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber(result));
            }
            else
            {
                stack.PushDatum(ScriptDatum.FromNumber(double.NaN));
            }
            return true;
        }

        private static bool HandleBitAnd(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var v = unchecked((Int32)(Int64)left.Number) & unchecked((Int32)(Int64)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber((double)v));
            }
            else if (left.Kind == ValueKind.Null || right.Kind == ValueKind.Null)
            {
                stack.PushDatum(ScriptDatum.FromNumber(0));
            }
            else
            {
                stack.PushDatum(ScriptDatum.FromNumber(double.NaN));
            }
            return true;
        }

        private static bool HandleBitOr(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var v = unchecked((Int32)(Int64)left.Number) | unchecked((Int32)(Int64)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber((double)v));
            }
            else if (left.Kind == ValueKind.Null)
            {
                stack.PushDatum(right);
            }
            else
            {
                stack.PushDatum(left);
            }
            return true;
        }

        private static bool HandleBitXor(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            var right = stack.PopDatum();
            var left = stack.PopDatum();
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                var v = unchecked((Int32)(Int64)left.Number) ^ unchecked((Int32)(Int64)right.Number);
                stack.PushDatum(ScriptDatum.FromNumber((double)v));
            }
            else if (left.Kind == ValueKind.Null)
            {
                stack.PushDatum(right);
            }
            else
            {
                stack.PushDatum(left);
            }
            return true;
        }

        private static bool HandleLogicNot(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicNot(exeContext);
            return true;
        }

        private static bool HandleLogicAnd(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicalBinary(exeContext, isAnd: true);
            return true;
        }

        private static bool HandleLogicOr(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicalBinary(exeContext, isAnd: false);
            return true;
        }

        private static bool HandleEqual(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteEquality(exeContext, negate: false);
            return true;
        }

        private static bool HandleNotEqual(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteEquality(exeContext, negate: true);
            return true;
        }

        private static bool HandleLessThan(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessThan);
            return true;
        }

        private static bool HandleLessEqual(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessEqual);
            return true;
        }

        private static bool HandleGreaterThan(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterThan);
            return true;
        }

        private static bool HandleGreaterEqual(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterEqual);
            return true;
        }

        private static bool HandleJump(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            frame.Pointer += offset;
            return true;
        }

        private static bool HandleJumpIfFalse(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (!isTrue)
            {
                frame.Pointer += offset;
            }
            return true;
        }

        private static bool HandleJumpIfTrue(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (isTrue)
            {
                frame.Pointer += offset;
            }
            return true;
        }

    }
}
