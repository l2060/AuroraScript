using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace AuroraScript.Runtime
{
    internal class PatchVM
    {
        /// </summary>
        private readonly ByteCodeBuffer _codeBuffer;

        public PatchVM(ByteCodeBuffer buffer)
        {
            _codeBuffer = buffer;
        }


        /// <summary>
        /// 执行当前调用帧中的指令
        /// 这是虚拟机的核心方法，实现了字节码的解释执行
        /// </summary>
        /// <param name="exeContext">执行上下文，包含操作数栈、调用栈和全局环境</param>
        public void Patch(ExecuteContext exeContext)
        {
            var _callStack = exeContext._callStack;
            var frame = _callStack.Peek();
            ScriptGlobal domainGlobal = frame.Global;
            var _operandStack = exeContext._operandStack;


            var popStack = _operandStack.Pop;
            var pushStack = _operandStack.Push;

            var opCode = _codeBuffer.ReadOpCode(frame);
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
                    break;

                case OpCode.STORE_LOCAL:
                    break;

                case OpCode.LOAD_CAPTURE:
                    break;

                case OpCode.STORE_CAPTURE:
                    break;

                case OpCode.CREATE_CLOSURE:
                    break;

                case OpCode.CAPTURE_VAR:
                    break;

                case OpCode.NEW_MODULE:
                    break;

                case OpCode.DEFINE_MODULE:
                    break;

                case OpCode.NEW_MAP:
                    break;

                case OpCode.NEW_ARRAY:
                    break;

                case OpCode.GET_ITERATOR:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.ITERATOR_VALUE:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.ITERATOR_HAS_VALUE:
                    pushStack(BooleanValue.False);
                    break;

                case OpCode.ITERATOR_NEXT:
                    break;
                case OpCode.GET_PROPERTY:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.SET_PROPERTY:
                    break;

                case OpCode.DELETE_PROPERTY:
                    break;
                case OpCode.GET_THIS_PROPERTY:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.SET_THIS_PROPERTY:
                    break;

                case OpCode.GET_GLOBAL_PROPERTY:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.SET_GLOBAL_PROPERTY:
                    break;

                case OpCode.GET_ELEMENT:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.SET_ELEMENT:
                    break;

                case OpCode.LOGIC_NOT:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.LOGIC_AND:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.LOGIC_OR:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.EQUAL:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.NOT_EQUAL:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.LESS_THAN:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.LESS_EQUAL:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.GREATER_THAN:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.GREATER_EQUAL:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.ADD:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.SUBTRACT:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.MULTIPLY:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.DIVIDE:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.MOD:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.NEGATE:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.INCREMENT:
                    pushStack(ScriptObject.Null);
                    break;
                case OpCode.DECREMENT:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_SHIFT_L:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_SHIFT_R:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_USHIFT_R:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_AND:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_OR:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_XOR:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.BIT_NOT:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.JUMP:
                    break;

                case OpCode.JUMP_IF_FALSE:
                    break;
                case OpCode.JUMP_IF_TRUE:
                    break;
                case OpCode.CALL:
                    pushStack(ScriptObject.Null);
                    break;

                case OpCode.RETURN:
                    frame?.Dispose();
                    pushStack(ScriptObject.Null);
                    // 切换到调用者的帧继续执行
                    frame = _callStack.Peek();
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
            }

        }
    }
}
