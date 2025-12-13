using AuroraScript.Core;
using System;

namespace AuroraScript.Runtime
{
    internal class PatchVM
    {
        /// </summary>
        private readonly IntPtr ByteCodePtr;

        public PatchVM(IntPtr byteCodePtr)
        {
            ByteCodePtr = byteCodePtr;
        }


        /// <summary>
        /// 执行当前调用帧中的指令
        /// 这是虚拟机的核心方法，实现了字节码的解释执行
        /// </summary>
        /// <param name="exeContext">执行上下文，包含操作数栈、调用栈和全局环境</param>
        public unsafe void Patch(ExecuteContext exeContext, int offset)
        {

            var opCode = (OpCode)(*(Byte*)(ByteCodePtr + offset));

            var stack = exeContext._operandStack;

            switch (opCode)
            {
                // 基本栈操作
                case OpCode.NOP:
                    break;

                case OpCode.POP:
                    break;

                case OpCode.DUP:
                    break;

                case OpCode.SWAP:
                    break;

                case OpCode.LOAD_ARG:
                    break;

                case OpCode.TRY_LOAD_ARG:
                    break;

                case OpCode.PUSH_I8:
                    break;

                case OpCode.PUSH_I16:
                    break;

                case OpCode.PUSH_I32:
                    break;

                case OpCode.PUSH_I64:
                    break;
                case OpCode.PUSH_F32:
                    break;

                case OpCode.PUSH_F64:
                    break;

                case OpCode.PUSH_STRING:
                    break;

                case OpCode.LOAD_LOCAL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.STORE_LOCAL:
                    break;

                case OpCode.LOAD_CAPTURE:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.STORE_CAPTURE:
                    break;


                case OpCode.PUSH_CONTEXT:
                    break;

                case OpCode.PUSH_ARGUMENTS:
                    break;

                case OpCode.CREATE_CLOSURE:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.CAPTURE_VAR:
                    break;

                case OpCode.INIT_MODULE:
                    break;

                case OpCode.NEW_MAP:
                    break;

                case OpCode.NEW_ARRAY:
                    break;

                case OpCode.GET_ITERATOR:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.ITERATOR_VALUE:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.ITERATOR_HAS_VALUE:
                    stack.PushDatum(ScriptDatum.FromBoolean(false));
                    break;

                case OpCode.ITERATOR_NEXT:
                    break;
                case OpCode.GET_PROPERTY:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.SET_PROPERTY:
                    break;

                case OpCode.DELETE_PROPERTY:
                    break;
                case OpCode.GET_THIS_PROPERTY:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.SET_THIS_PROPERTY:
                    break;

                case OpCode.GET_GLOBAL_PROPERTY:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.SET_GLOBAL_PROPERTY:
                    break;

                case OpCode.GET_ELEMENT:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.SET_ELEMENT:
                    break;

                case OpCode.LOGIC_NOT:
                    stack.PushDatum(ScriptDatum.Null);
                    break;
                case OpCode.LOGIC_AND:
                    stack.PushDatum(ScriptDatum.Null);
                    break;
                case OpCode.LOGIC_OR:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.EQUAL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.NOT_EQUAL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.LESS_THAN:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.LESS_EQUAL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.GREATER_THAN:
                    stack.PushDatum(ScriptDatum.Null);
                    break;
                case OpCode.GREATER_EQUAL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.ADD:
                    stack.PushDatum(ScriptDatum.Null);
                    break;
                case OpCode.SUBTRACT:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.MULTIPLY:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.DIVIDE:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.MOD:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.NEGATE:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.INCREMENT:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;
                case OpCode.DECREMENT:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_SHIFT_L:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_SHIFT_R:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_USHIFT_R:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_AND:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_OR:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_XOR:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.BIT_NOT:
                    stack.PushDatum(ScriptDatum.NaN);
                    break;

                case OpCode.JUMP:
                    break;

                case OpCode.JUMP_IF_FALSE:
                    break;
                case OpCode.JUMP_IF_TRUE:
                    break;
                case OpCode.CALL:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.RETURN:
                    stack.PushDatum(ScriptDatum.Null);
                    break;

                case OpCode.YIELD:
                    break;
                case OpCode.PUSH_0:
                    break;
                case OpCode.PUSH_1:
                    break;
                case OpCode.PUSH_2:
                    break;
                case OpCode.PUSH_3:

                    break;
                case OpCode.PUSH_4:
                    break;
                case OpCode.PUSH_5:
                    break;
                case OpCode.PUSH_6:
                    break;
                case OpCode.PUSH_7:
                    break;
                case OpCode.PUSH_8:
                    break;
                case OpCode.PUSH_9:
                    break;
                case OpCode.PUSH_NULL:
                    break;
                case OpCode.PUSH_FALSE:
                    break;
                case OpCode.PUSH_TRUE:
                    break;
                case OpCode.PUSH_THIS:
                    break;
                case OpCode.PUSH_GLOBAL:
                    break;
                case OpCode.NEW_REGEX:
                    stack.PushDatum(ScriptDatum.Null);
                    break;
            }

        }
    }
}
