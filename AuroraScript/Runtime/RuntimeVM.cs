using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// AuroraScript 运行时虚拟机，负责执行字节码并管理运行时环境
    /// 作为脚本引擎的核心组件，实现了字节码的解释执行和运行时环境的管理
    /// </summary>
    internal class RuntimeVM
    {
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





        /// <summary>
        /// 使用指定的字节码和字符串常量池初始化虚拟机
        /// </summary>
        /// <param name="bytecode">要执行的字节码，由编译器生成的二进制指令序列</param>
        /// <param name="stringConstants">字符串常量池，包含脚本中所有的字符串字面量</param>

        public RuntimeVM(byte[] bytecode, ImmutableArray<String> stringConstants, DebugSymbolInfo debugSymbols, ClrTypeRegistry clrRegistry)
        {
            // 创建字节码缓冲区，用于读取和解析字节码指令
            _codeBuffer = new ByteCodeBuffer(bytecode);
            // 将字符串常量转换为StringValue对象并存储在不可变数组中
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
            // 调试符号信息
            _debugSymbols = debugSymbols;
            _clrRegistry = clrRegistry;
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
            Int32 localIndex = 0;
            StringValue propName = null;
            ScriptDatum datumValue;
            ScriptDatum datumLeft;
            ScriptDatum datumRight;
            ScriptObject value = null;
            ScriptObject obj = null;

            // 获取当前调用帧
            var frame = _callStack.Peek();

            // 获取当前域的全局对象
            ScriptGlobal domainGlobal = frame.Global;

            ScriptDatum PopDatum() => _operandStack.PopDatum();
            void PushDatum(ScriptDatum datum) => _operandStack.PushDatum(datum);
            ScriptObject PopObject() => _operandStack.Pop();
            void PushObject(ScriptObject obj) => _operandStack.Push(obj);
            ScriptDatum PeekDatum() => _operandStack.PeekDatum();


            // 数值一元操作的lambda函数，用于简化代码
            // 对栈顶的数值执行一元操作，如果不是数值则返回默认值
            var NumberUnaryOperation = (double defaultValue, Func<double, double> operation) =>
            {
                var datum = PopDatum();
                if (datum.Kind == ValueKind.Number)
                {
                    PushDatum(ScriptDatum.FromNumber(operation(datum.Number)));
                }
                else
                {
                    PushDatum(ScriptDatum.FromNumber(defaultValue));
                }
            };







            var NumberBinaryOperation = (double defaultValue, Func<double, double, double> operation) =>
            {
                var right = PopDatum();
                var left = PopDatum();
                if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
                {
                    PushDatum(ScriptDatum.FromNumber(operation(left.Number, right.Number)));
                }
                else
                {
                    PushDatum(ScriptDatum.FromNumber(defaultValue));
                }
            };

            Boolean DatumEquals(ScriptDatum leftDatum, ScriptDatum rightDatum)
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

            String ExtractPropertyKey(ScriptDatum keyDatum)
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
                        return keyDatum.ToObject()?.ToString() ?? String.Empty;
                }
            }

            var NumberBinaryPredicate = (Func<double, double, bool> predicate) =>
            {
                var right = PopDatum();
                var left = PopDatum();
                if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
                {
                    PushDatum(ScriptDatum.FromBoolean(predicate(left.Number, right.Number)));
                }
                else
                {
                    PushDatum(ScriptDatum.FromBoolean(false));
                }
            };



            // 主执行循环，不断读取并执行指令，直到遇到返回指令或发生异常
            while (true)
            {
                // 从当前指令指针位置读取操作码
                var opCode = _codeBuffer.ReadOpCode(frame);
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
                        var topDatum = PeekDatum();
                        PushDatum(topDatum);
                        break;

                    case OpCode.SWAP:
                        // 交换栈顶两个元素
                        var first = PopDatum();
                        var second = PopDatum();
                        PushDatum(first);
                        PushDatum(second);
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
                        localIndex = _codeBuffer.ReadInt32(frame);
                        PushDatum(frame.GetLocalDatum(localIndex));
                        break;

                    case OpCode.STORE_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        frame.SetLocalDatum(localIndex, PopDatum());
                        break;

                    case OpCode.LOAD_CAPTURE:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        datumValue = frame.GetLocalDatum(localIndex);
                        if (datumValue.Kind != ValueKind.Object || datumValue.Object is not Upvalue upvalueToRead)
                        {
                            throw new AuroraVMException("Invalid captured upvalue");
                        }
                        PushDatum(upvalueToRead.Get());
                        break;

                    case OpCode.STORE_CAPTURE:
                        datumValue = PopDatum();
                        localIndex = _codeBuffer.ReadInt32(frame);
                        var upvalueDatum = frame.GetLocalDatum(localIndex);
                        if (upvalueDatum.Kind != ValueKind.Object || upvalueDatum.Object is not Upvalue upvalueToWrite)
                        {
                            throw new AuroraVMException("Invalid captured upvalue");
                        }
                        upvalueToWrite.Set(datumValue);
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

                        var closure = new ClosureFunction(moduleForClosure, entryPointer, capturedUpvalues);
                        PushObject(closure);
                        break;

                    case OpCode.CAPTURE_VAR:
                        var slotIndex = _codeBuffer.ReadInt32(frame);
                        var capturedUpvalue = frame.GetCapturedUpvalue(slotIndex) ?? frame.GetOrCreateUpvalue(slotIndex);
                        capturedUpvalue.MarkAliasSlot(slotIndex);
                        PushObject(capturedUpvalue);
                        break;

                    case OpCode.NEW_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        PushObject(new ScriptModule(propName.Value));
                        break;

                    case OpCode.DEFINE_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = PopObject();
                        domainGlobal.Define(propName.Value, value, writeable: false, enumerable: true);
                        break;

                    case OpCode.NEW_MAP:
                        PushObject(new ScriptObject());
                        break;

                    case OpCode.NEW_ARRAY:
                        var count = _codeBuffer.ReadInt32(frame);
                        var datumBuffer = new ScriptDatum[count];
                        for (int i = count - 1; i >= 0; i--)
                        {
                            datumBuffer[i] = PopDatum();
                        }
                        var newArray = new ScriptArray(count);
                        for (int i = 0; i < count; i++)
                        {
                            newArray.SetDatum(i, datumBuffer[i]);
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
                            PushObject(iterator.Value());
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
                            targetScriptObj.SetPropertyValue(propName.Value, (Base.ScriptObject)value);
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
                        frame.Module.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        if (_clrRegistry != null && _clrRegistry.TryGetDescriptor(propName.Value, out var descriptor))
                        {
                            value = descriptor.GetOrCreateTypeObject();
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
                        if (datumObjValue.Kind == ValueKind.Array && datumObjValue.Object is ScriptArray scriptArray && datumValue.Kind == ValueKind.Number)
                        {
                            PushDatum(scriptArray.GetDatum((Int32)datumValue.Number));
                        }
                        else if (datumObjValue.Kind == ValueKind.Object)
                        {
                            var key = ExtractPropertyKey(datumValue);
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
                        if (datumTargetObj.Kind == ValueKind.Array && datumTargetObj.Object is ScriptArray scriptArray2 && datumValue.Kind == ValueKind.Number)
                        {
                            scriptArray2.SetDatum((Int32)datumValue.Number, datumAssignedValue);
                        }
                        else if (datumTargetObj.Kind == ValueKind.Object)
                        {
                            var key = ExtractPropertyKey(datumValue);
                            datumTargetObj.Object.SetPropertyValue(key, datumAssignedValue.ToObject());
                        }
                        break;

                    case OpCode.LOGIC_NOT:
                        datumValue = PopDatum();
                        PushDatum(ScriptDatum.FromBoolean(!datumValue.IsTrue()));
                        break;
                    case OpCode.LOGIC_AND:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        PushDatum(ScriptDatum.FromBoolean(datumLeft.IsTrue() && datumRight.IsTrue()));
                        break;
                    case OpCode.LOGIC_OR:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        PushDatum(ScriptDatum.FromBoolean(datumLeft.IsTrue() || datumRight.IsTrue()));
                        break;

                    case OpCode.EQUAL:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        PushDatum(ScriptDatum.FromBoolean(DatumEquals(datumLeft, datumRight)));
                        break;

                    case OpCode.NOT_EQUAL:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        PushDatum(ScriptDatum.FromBoolean(!DatumEquals(datumLeft, datumRight)));
                        break;

                    case OpCode.LESS_THAN:
                        NumberBinaryPredicate((l, r) => l < r);
                        break;

                    case OpCode.LESS_EQUAL:
                        NumberBinaryPredicate((l, r) => l <= r);
                        break;

                    case OpCode.GREATER_THAN:
                        NumberBinaryPredicate((l, r) => l > r);
                        break;
                    case OpCode.GREATER_EQUAL:
                        NumberBinaryPredicate((l, r) => l >= r);
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
                            var result = datumLeft.ToObject() + datumRight.ToObject();
                            PushObject(result);
                        }
                        break;
                    case OpCode.SUBTRACT:
                        NumberBinaryOperation(double.NaN, (l, r) => l - r);
                        break;
                    case OpCode.MULTIPLY:
                        NumberBinaryOperation(double.NaN, (l, r) => l * r);
                        break;
                    case OpCode.DIVIDE:
                        NumberBinaryOperation(double.NaN, (l, r) => l / r);
                        break;
                    case OpCode.MOD:
                        NumberBinaryOperation(double.NaN, (l, r) => l % r);
                        break;
                    case OpCode.NEGATE:
                        NumberUnaryOperation(double.NaN, (v) => -v);
                        break;
                    case OpCode.INCREMENT:
                        NumberUnaryOperation(double.NaN, (v) => v + 1);
                        break;
                    case OpCode.DECREMENT:
                        NumberUnaryOperation(double.NaN, (v) => v - 1);
                        break;
                    case OpCode.BIT_NOT:
                        NumberUnaryOperation(-1, v => ~(long)v);
                        break;



                    case OpCode.BIT_SHIFT_L:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            PushDatum(ScriptDatum.FromNumber((double)((int)datumLeft.Number << (int)datumRight.Number)));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNumber(double.NaN));
                        }
                        break;

                    case OpCode.BIT_SHIFT_R:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            PushDatum(ScriptDatum.FromNumber((double)((int)datumLeft.Number >> (int)datumRight.Number)));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNumber(double.NaN));
                        }
                        break;

                    case OpCode.BIT_USHIFT_R:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            PushDatum(ScriptDatum.FromNumber((double)((int)datumLeft.Number >>> (int)datumRight.Number)));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNumber(double.NaN));
                        }
                        break;

                    case OpCode.BIT_AND:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            var v = unchecked((Int32)(Int64)datumLeft.Number) & unchecked((Int32)(Int64)datumRight.Number);
                            PushDatum(ScriptDatum.FromNumber((double)v));
                        }
                        else if (datumLeft.Kind == ValueKind.Null || datumRight.Kind == ValueKind.Null)
                        {
                            PushDatum(ScriptDatum.FromNumber(0));
                        }
                        else
                        {
                            PushDatum(ScriptDatum.FromNumber(double.NaN));
                        }
                        break;

                    case OpCode.BIT_OR:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            var v = unchecked((Int32)(Int64)datumLeft.Number) | unchecked((Int32)(Int64)datumRight.Number);
                            PushDatum(ScriptDatum.FromNumber((double)v));
                        }
                        else if (datumLeft.Kind == ValueKind.Null)
                        {
                            PushDatum(datumRight);
                        }
                        else
                        {
                            PushDatum(datumLeft);
                        }
                        break;

                    case OpCode.BIT_XOR:
                        datumRight = PopDatum();
                        datumLeft = PopDatum();
                        if (datumLeft.Kind == ValueKind.Number && datumRight.Kind == ValueKind.Number)
                        {
                            var v = unchecked((Int32)(Int64)datumLeft.Number) ^ unchecked((Int32)(Int64)datumRight.Number);
                            PushDatum(ScriptDatum.FromNumber((double)v));
                        }
                        else if (datumLeft.Kind == ValueKind.Null)
                        {
                            PushDatum(datumRight);
                        }
                        else
                        {
                            PushDatum(datumLeft);
                        }
                        break;

                    case OpCode.TYPEOF:
                        datumRight = PopDatum();
                        PushDatum(datumRight.TypeOf());
                        break;
                    case OpCode.ALLOC_LOCALS:
                        var localsRequested = _codeBuffer.ReadInt32(frame);
                        frame.EnsureLocalStorage(localsRequested);
                        break;

                    case OpCode.JUMP:
                        var offset = _codeBuffer.ReadInt32(frame);
                        frame.Pointer += offset;
                        break;

                    case OpCode.JUMP_IF_FALSE:
                        offset = _codeBuffer.ReadInt32(frame);
                        datumValue = PopDatum();
                        if (!datumValue.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;
                    case OpCode.JUMP_IF_TRUE:
                        offset = _codeBuffer.ReadInt32(frame);
                        datumValue = PopDatum();
                        if (datumValue.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;
                    case OpCode.CALL:
                        // 函数调用指令
                        // 从栈顶弹出可调用对象
                        var callable = PopObject();
                        // 读取参数数量
                        var argCount = _codeBuffer.ReadByte(frame);
                        // 创建参数数组
                        var argDatums = new ScriptDatum[argCount];
                        // 从栈中弹出参数，注意参数顺序是从右到左
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            argDatums[i] = PopDatum();
                        }
                        if (callable is ClosureFunction closureFunc)
                        {
                            if (_callStack.Count > exeContext.ExecuteOptions.MaxCallStackDepth)
                            {
                                throw new AuroraVMException("The number of method call stacks exceeds the limit of " + exeContext.ExecuteOptions.MaxCallStackDepth);
                            }
                            // 如果是脚本中定义的闭包函数
                            // 创建新的调用帧，包含环境、全局对象、模块和入口点
                            var callFrame = CallFramePool.Rent(frame.Global, closureFunc.Module, closureFunc.EntryPointer, argDatums, closureFunc.CapturedUpvalues);
                            // 将新帧压入调用栈
                            _callStack.Push(callFrame);
                            // 更新当前帧引用
                            frame = callFrame;
                        }
                        else if (callable is Callable callableFunc)
                        {
                            var callResult = callableFunc.Invoke(exeContext, null, argDatums);
                            PushObject(callResult);
                        }
                        else if (callable is IClrInvokable clrInvokable)
                        {
                            var callResult = clrInvokable.Invoke(exeContext, callable as ScriptObject, argDatums);
                            PushDatum(callResult);
                        }
                        else
                        {
                            // 如果不是可调用对象，抛出异常
                            throw new InvalidOperationException($"Cannot call {callable}");
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

                    case OpCode.YIELD:
                        // TODO
                        if (exeContext.ExecuteOptions.YieldEnabled)
                        {
                            exeContext.SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
                            return;
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
                }
            }
        }

    }
}
