using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Interop;
using System;
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
    internal unsafe partial class RuntimeVM
    {
        private static readonly delegate*<ExecuteFrameContext, void>[] _opDispatch;

        static RuntimeVM()
        {
            var maxOp = Enum.GetValues(typeof(OpCode)).Cast<byte>().Max();
            _opDispatch = new delegate*<ExecuteFrameContext, void>[maxOp + 1];




            RegisterHandler(OpCode.NOP, &NOP);
            RegisterHandler(OpCode.POP, &POP);
            RegisterHandler(OpCode.DUP, &DUP);
            RegisterHandler(OpCode.SWAP, &SWAP);
            RegisterHandler(OpCode.LOAD_ARG, &LOAD_ARG);
            RegisterHandler(OpCode.TRY_LOAD_ARG, &TRY_LOAD_ARG);
            RegisterHandler(OpCode.ALLOC_LOCALS, &ALLOC_LOCALS);
            RegisterHandler(OpCode.LOAD_LOCAL, &LOAD_LOCAL);
            RegisterHandler(OpCode.STORE_LOCAL, &STORE_LOCAL);
            RegisterHandler(OpCode.LOAD_LOCAL_L, &LOAD_LOCAL_LONG);
            RegisterHandler(OpCode.STORE_LOCAL_L, &STORE_LOCAL_LONG);
            RegisterHandler(OpCode.GET_PROPERTY, &GET_PROPERTY);
            RegisterHandler(OpCode.SET_PROPERTY, &SET_PROPERTY);

            RegisterHandler(OpCode.DELETE_PROPERTY, &DELETE_PROPERTY);
            RegisterHandler(OpCode.GET_THIS_PROPERTY, &GET_THIS_PROPERTY);
            RegisterHandler(OpCode.SET_THIS_PROPERTY, &SET_THIS_PROPERTY);
            RegisterHandler(OpCode.GET_GLOBAL_PROPERTY, &GET_GLOBAL_PROPERTY);
            RegisterHandler(OpCode.SET_GLOBAL_PROPERTY, &SET_GLOBAL_PROPERTY);





            RegisterHandler(OpCode.GET_ELEMENT, &GET_ELEMENT);
            RegisterHandler(OpCode.SET_ELEMENT, &SET_ELEMENT);
            RegisterHandler(OpCode.CALL, &CALL);
            RegisterHandler(OpCode.RETURN, &RETURN);
            RegisterHandler(OpCode.RETURN_NULL, &RETURN_NULL);
            RegisterHandler(OpCode.YIELD, &YIELD);
            RegisterHandler(OpCode.CAPTURE_VAR, &CAPTURE_VAR);
            RegisterHandler(OpCode.LOAD_CAPTURE, &LOAD_CAPTURE);
            RegisterHandler(OpCode.STORE_CAPTURE, &STORE_CAPTURE);
            RegisterHandler(OpCode.SUBTRACT, &SUBTRACT);
            RegisterHandler(OpCode.MULTIPLY, &MULTIPLY);
            RegisterHandler(OpCode.DIVIDE, &DIVIDE);
            RegisterHandler(OpCode.MOD, &MOD);
            RegisterHandler(OpCode.NEGATE, &NEGATE);
            RegisterHandler(OpCode.INCREMENT, &INCREMENT);
            RegisterHandler(OpCode.DECREMENT, &DECREMENT);
            RegisterHandler(OpCode.BIT_NOT, &BIT_NOT);
            RegisterHandler(OpCode.BIT_SHIFT_L, &BIT_SHIFT_LEFT);
            RegisterHandler(OpCode.BIT_SHIFT_R, &BIT_SHIFT_RIGHT);
            RegisterHandler(OpCode.BIT_USHIFT_R, &BIT_UNSIGNED_SHIFT_RIGHT);
            RegisterHandler(OpCode.BIT_AND, &BIT_AND);
            RegisterHandler(OpCode.BIT_OR, &BIT_OR);
            RegisterHandler(OpCode.BIT_XOR, &BIT_XOR);
            RegisterHandler(OpCode.LOGIC_NOT, &LOGIC_NOT);
            RegisterHandler(OpCode.LOGIC_AND, &LOGIC_AND);
            RegisterHandler(OpCode.LOGIC_OR, &LOGIC_OR);
            RegisterHandler(OpCode.EQUAL, &EQUAL);
            RegisterHandler(OpCode.NOT_EQUAL, &NOT_EQUAL);
            RegisterHandler(OpCode.LESS_THAN, &LESS_THAN);
            RegisterHandler(OpCode.LESS_EQUAL, &LESS_EQUAL);
            RegisterHandler(OpCode.GREATER_THAN, &GREATER_THAN);
            RegisterHandler(OpCode.GREATER_EQUAL, &GREATER_EQUAL);
            RegisterHandler(OpCode.JUMP, &JUMP);
            RegisterHandler(OpCode.JUMP_IF_FALSE, &JUMP_IF_FALSE);
            RegisterHandler(OpCode.JUMP_IF_TRUE, &JUMP_IF_TRUE);
            RegisterHandler(OpCode.ADD, &ADD);
            RegisterHandler(OpCode.INC_LOCAL, &INC_LOCAL);
            RegisterHandler(OpCode.INC_LOCAL_L, &INC_LOCAL_L);
            RegisterHandler(OpCode.DEC_LOCAL, &DEC_LOCAL);
            RegisterHandler(OpCode.DEC_LOCAL_L, &DEC_LOCAL_L);
            RegisterHandler(OpCode.INC_LOCAL_POST, &INC_LOCAL_POST);
            RegisterHandler(OpCode.INC_LOCAL_POST_L, &INC_LOCAL_POST_L);
            RegisterHandler(OpCode.DEC_LOCAL_POST, &DEC_LOCAL_POST);
            RegisterHandler(OpCode.DEC_LOCAL_POST_L, &DEC_LOCAL_POST_L);
            RegisterHandler(OpCode.ADD_LOCAL_STACK, &ADD_LOCAL_STACK);
            RegisterHandler(OpCode.ADD_LOCAL_STACK_L, &ADD_LOCAL_STACK_L);
            RegisterHandler(OpCode.SUB_LOCAL_STACK, &SUB_LOCAL_STACK);
            RegisterHandler(OpCode.SUB_LOCAL_STACK_L, &SUB_LOCAL_STACK_L);
            RegisterHandler(OpCode.MUL_LOCAL_STACK, &MUL_LOCAL_STACK);
            RegisterHandler(OpCode.MUL_LOCAL_STACK_L, &MUL_LOCAL_STACK_L);
            RegisterHandler(OpCode.DIV_LOCAL_STACK, &DIV_LOCAL_STACK);
            RegisterHandler(OpCode.DIV_LOCAL_STACK_L, &DIV_LOCAL_STACK_L);
            RegisterHandler(OpCode.MOD_LOCAL_STACK, &MOD_LOCAL_STACK);
            RegisterHandler(OpCode.MOD_LOCAL_STACK_L, &MOD_LOCAL_STACK_L);
            RegisterHandler(OpCode.TYPEOF, &TYPEOF);

            RegisterHandler(OpCode.CREATE_CLOSURE, &CREATE_CLOSURE);
            RegisterHandler(OpCode.INIT_MODULE, &INIT_MODULE);
            RegisterHandler(OpCode.NEW_MAP, &NEW_MAP);
            RegisterHandler(OpCode.NEW_ARRAY, &NEW_ARRAY);
            RegisterHandler(OpCode.NEW_REGEX, &NEW_REGEX);
            RegisterHandler(OpCode.DECONSTRUCT_ARRAY, &DECONSTRUCT_ARRAY);
            RegisterHandler(OpCode.DECONSTRUCT_MAP, &DECONSTRUCT_MAP);




            RegisterHandler(OpCode.GET_ITERATOR, &GET_ITERATOR);
            RegisterHandler(OpCode.ITERATOR_VALUE, &ITERATOR_VALUE);
            RegisterHandler(OpCode.ITERATOR_HAS_VALUE, &ITERATOR_HAS_VALUE);
            RegisterHandler(OpCode.ITERATOR_NEXT, &ITERATOR_NEXT);




            RegisterHandler(OpCode.PUSH_0, &PUSH_0);
            RegisterHandler(OpCode.PUSH_1, &PUSH_1);
            RegisterHandler(OpCode.PUSH_2, &PUSH_2);
            RegisterHandler(OpCode.PUSH_3, &PUSH_3);
            RegisterHandler(OpCode.PUSH_4, &PUSH_4);
            RegisterHandler(OpCode.PUSH_5, &PUSH_5);
            RegisterHandler(OpCode.PUSH_6, &PUSH_6);
            RegisterHandler(OpCode.PUSH_7, &PUSH_7);
            RegisterHandler(OpCode.PUSH_8, &PUSH_8);
            RegisterHandler(OpCode.PUSH_9, &PUSH_9);







            RegisterHandler(OpCode.PUSH_NULL, &PUSH_NULL);
            RegisterHandler(OpCode.PUSH_FALSE, &PUSH_FALSE);
            RegisterHandler(OpCode.PUSH_TRUE, &PUSH_TRUE);
            RegisterHandler(OpCode.PUSH_THIS, &PUSH_THIS);
            RegisterHandler(OpCode.PUSH_GLOBAL, &PUSH_GLOBAL);
            RegisterHandler(OpCode.PUSH_CONTEXT, &PUSH_CONTEXT);
            RegisterHandler(OpCode.PUSH_ARGUMENTS, &PUSH_ARGUMENTS);


            RegisterHandler(OpCode.PUSH_I8, &PUSH_I8);
            RegisterHandler(OpCode.PUSH_I16, &PUSH_I16);
            RegisterHandler(OpCode.PUSH_I32, &PUSH_I32);
            RegisterHandler(OpCode.PUSH_I64, &PUSH_I64);
            RegisterHandler(OpCode.PUSH_F32, &PUSH_F32);
            RegisterHandler(OpCode.PUSH_F64, &PUSH_F64);
            RegisterHandler(OpCode.PUSH_STRING, &PUSH_STRING);







        }

        private static void RegisterHandler(OpCode opCode, delegate*<ExecuteFrameContext, void> handler)
        {
            _opDispatch[(Int32)opCode] = handler;
        }


        /// <summary>
        /// 字符串常量池，存储脚本中使用的所有字符串常量
        /// 通过索引快速访问字符串值，避免重复创建相同的字符串对象
        /// </summary>
        internal readonly ImmutableArray<StringValue> _stringConstants;

        /// <summary>
        /// 当前执行的字节码缓冲区
        /// 包含编译后的指令序列，由虚拟机解释执行
        /// </summary>
        internal readonly ByteCodeBuffer _codeBuffer;

        /// <summary>
        /// 调试符号信息，包含脚本的调试信息，如行号、函数名等
        /// </summary>
        private readonly DebugSymbolInfo _debugSymbols;

        internal readonly ClrTypeRegistry _clrRegistry;

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
                Console.WriteLine($"OPCODE {code,-20} COUNT: {count,-20}   TICKS: {_opTicks[index],-20}");
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
            //var _callStack = exeContext._callStack;
            // 获取当前调用帧
            //var frame = _callStack.Peek();
            var opDispatch = _opDispatch;
            var ctx = new ExecuteFrameContext(this, exeContext);
            // 主执行循环，不断读取并执行指令，直到遇到返回指令或发生异常
            while (exeContext.Status == ExecuteStatus.Running)
            {
                // 从当前指令指针位置读取操作码
                var opCode = ctx.ReadOpCode();
                _opCounts[opCode]++;
                var start = Stopwatch.GetTimestamp();
                delegate*<ExecuteFrameContext, void> handler = opDispatch[opCode];
                handler(ctx);
                var end = Stopwatch.GetTimestamp();
                _opTicks[opCode] += (end - start);
            }
        }







        #region 辅助函数




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
                        return String.Equals(leftDatum.String.Value, rightDatum.String.Value, StringComparison.Ordinal);
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
            return leftObj.Equals(rightObj);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static StringValue ExtractPropertyKey(ref ScriptDatum keyDatum)
        {
            switch (keyDatum.Kind)
            {
                case ValueKind.String:
                    return keyDatum.String;
                case ValueKind.Number:
                    return StringValue.Of(keyDatum.Number.ToString(CultureInfo.InvariantCulture));
                case ValueKind.Boolean:
                    return keyDatum.Boolean ? StringValue.TRUE : StringValue.FALSE;
                case ValueKind.Null:
                    return StringValue.NULL;
                default:
                    return StringValue.OBJECT;
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

        #endregion



    }
}
