using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// AuroraScript 运行时虚拟机，负责执行字节码并管理运行时环境
    /// </summary>
    internal class RuntimeVM
    {
        // 操作数栈，用于存储执行过程中的临时值
        private readonly Stack<ScriptObject> _operandStack;

        // 调用栈，用于管理函数调用
        private readonly Stack<CallFrame> _callStack;

        // 全局环境，存储全局变量和函数
        private readonly Environment _globalEnv;

        // 字符串常量池
        private readonly ImmutableArray<StringValue> _stringConstants;

        // 当前执行的字节码
        private byte[] _currentBytecode;



        /// <summary>
        /// 初始化运行时虚拟机
        /// </summary>
        public RuntimeVM()
        {
            _operandStack = new Stack<ScriptObject>();
            _callStack = new Stack<CallFrame>();
            _globalEnv = new Environment(null);
            _stringConstants = [];
        }

        /// <summary>
        /// 使用指定的字节码和字符串常量池初始化虚拟机
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="stringConstants">字符串常量池</param>
        public RuntimeVM(byte[] bytecode, ImmutableArray<String> stringConstants)
        {
            _operandStack = new Stack<ScriptObject>();
            _callStack = new Stack<CallFrame>();
            _globalEnv = new Environment(null);
            _currentBytecode = bytecode;
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
        }

        /// <summary>
        /// 加载字节码和字符串常量池
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="stringConstants">字符串常量池</param>
        public void Load(byte[] bytecode, string[] stringConstants)
        {
            _currentBytecode = bytecode;
            _operandStack.Clear();
            _callStack.Clear();
        }

        /// <summary>
        /// 执行已加载的字节码
        /// </summary>
        /// <returns>执行结果</returns>
        public object Execute()
        {
            if (_currentBytecode == null || _currentBytecode.Length == 0)
            {
                throw new InvalidOperationException("No bytecode loaded");
            }

            // 创建主调用帧
            var mainFrame = new CallFrame(_currentBytecode, 0, _globalEnv);
            _callStack.Push(mainFrame);

            return ExecuteFrame();
        }

        /// <summary>
        /// 执行二元操作
        /// </summary>
        /// <param name="operation">要执行的操作</param>
        private void PerformBinaryOperation(Func<ScriptObject, ScriptObject, ScriptObject> operation)
        {
            var right = _operandStack.Pop();
            var left = _operandStack.Pop();
            var result = operation(left, right);
            _operandStack.Push(result);
        }


        public void Execute2()
        {
            // 获取当前调用帧
            var frame = _callStack.Peek();



            //while (true)
            //{



            //}

        }







        /// <summary>
        /// 执行当前调用帧
        /// </summary>
        /// <returns>执行结果</returns>
        private object ExecuteFrame()
        {

            // 获取当前调用帧
            var frame = _callStack.Peek();

            // 执行循环
            while (true)
            {
                // 读取操作码
                var opCode = frame.ReadOpCode();
                // 根据操作码执行相应的操作
                switch (opCode)
                {
                    // 基本栈操作
                    case OpCode.NOP:
                        // 空操作
                        break;

                    case OpCode.POP:
                        // 弹出栈顶元素
                        _operandStack.Pop();
                        break;

                    case OpCode.DUP:
                        // 复制栈顶元素
                        var topValue = _operandStack.Peek();
                        _operandStack.Push(topValue);
                        break;

                    case OpCode.SWAP:
                        // 交换栈顶两个元素
                        var a = _operandStack.Pop();
                        var b = _operandStack.Pop();
                        _operandStack.Push(a);
                        _operandStack.Push(b);
                        break;

                    // 常量操作
                    case OpCode.PUSH_NULL:
                        _operandStack.Push(ScriptObject.Null);
                        break;

                    case OpCode.PUSH_FALSE:
                        _operandStack.Push(BooleanValue.False);
                        break;

                    case OpCode.PUSH_TRUE:
                        _operandStack.Push(BooleanValue.True);
                        break;

                    case OpCode.PUSH_THIS:
                        _operandStack.Push(frame.Environment);
                        break;

                    case OpCode.PUSH_I8:
                        _operandStack.Push(NumberValue.Of(frame.ReadSByte()));
                        break;

                    case OpCode.PUSH_I16:
                        _operandStack.Push(NumberValue.Of(frame.ReadInt16()));
                        break;

                    case OpCode.PUSH_I32:
                        _operandStack.Push(NumberValue.Of(frame.ReadInt32()));
                        break;

                    case OpCode.PUSH_F32:
                        _operandStack.Push(NumberValue.Of(frame.ReadFloat()));
                        break;

                    case OpCode.PUSH_F64:
                        _operandStack.Push(NumberValue.Of(frame.ReadDouble()));
                        break;

                    case OpCode.PUSH_STRING:
                        var stringIndex = frame.ReadInt32();
                        _operandStack.Push(_stringConstants[stringIndex]);
                        break;

                    // 局部变量操作
                    case OpCode.PUSH_LOCAL:
                        var localIndex = frame.ReadInt32();
                        _operandStack.Push(frame.Locals[localIndex]);
                        break;

                    case OpCode.POP_TO_LOCAL:
                        localIndex = frame.ReadInt32();
                        frame.Locals[localIndex] = _operandStack.Pop();
                        break;

                    // 全局变量操作
                    case OpCode.PUSH_GLOBAL:
                        _operandStack.Push(_globalEnv);
                        break;

                    // 属性操作
                    case OpCode.GET_PROPERTY:
                        var propNameIndex = frame.ReadInt32();
                        var propName = _stringConstants[propNameIndex];
                        var obj = _operandStack.Pop();

                        if (obj is Environment env)
                        {
                            var propValue = env.GetPropertyValue(propName.Value);
                            _operandStack.Push(propValue);
                        }
                        else if (obj is ScriptObject scriptObj)
                        {
                            _operandStack.Push(scriptObj.GetPropertyValue(propName.Value));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot get property '{propName}' from {obj}");
                        }
                        break;

                    case OpCode.SET_PROPERTY:
                        propNameIndex = frame.ReadInt32();
                        propName = _stringConstants[propNameIndex];
                        var value = _operandStack.Pop();
                        obj = _operandStack.Pop();

                        if (obj is Environment targetEnv)
                        {
                            targetEnv.Define(propName.Value, value);
                        }
                        else if (obj is Base.ScriptObject targetScriptObj)
                        {
                            targetScriptObj.SetPropertyValue(propName.Value, (Base.ScriptObject)value);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot set property '{propName}' on {obj}");
                        }
                        break;

                    // 数组和映射操作
                    case OpCode.NEW_ARRAY:
                        var count = frame.ReadInt32();
                        var buffer = new ScriptObject[count];
                        for (int i = count - 1; i >= 0; i--)
                        {
                            buffer[i] = _operandStack.Pop();
                        }
                        var array = new ScriptArray(buffer);
                        _operandStack.Push(array);
                        break;

                    // 算术操作
                    case OpCode.ADD:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av + bv;
                            }
                            else if (a is StringValue strA || b is StringValue strB)
                            {
                                return a + b;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.SUBTRACT:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av - bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.MULTIPLY:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av * bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.DIVIDE:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av / bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    // 位运算操作
                    case OpCode.BIT_SHIFT_L:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av << bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.BIT_SHIFT_R:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av >> bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.BIT_AND:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av & bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.BIT_OR:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av | bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.BIT_XOR:
                        PerformBinaryOperation((a, b) =>
                        {
                            if (a is NumberValue av && b is NumberValue bv)
                            {
                                return av ^ bv;
                            }
                            return ScriptObject.Null;
                        });
                        break;

                    case OpCode.BIT_NOT:
                        var operand = _operandStack.Pop();
                        if (operand is NumberValue number)
                        {
                            _operandStack.Push(~number);
                        }
                        else
                        {
                            _operandStack.Push(ScriptObject.Null);
                        }
                        break;

                    // 跳转操作
                    case OpCode.JUMP:
                        var offset = frame.ReadInt32();
                        frame.Pointer += offset;
                        break;

                    case OpCode.JUMP_IF_FALSE:
                        offset = frame.ReadInt32();
                        if (!Convert.ToBoolean(_operandStack.Pop()))
                        {
                            frame.Pointer += offset;
                        }
                        break;

                    case OpCode.JUMP_IF_TRUE:
                        offset = frame.ReadInt32();
                        if (Convert.ToBoolean(_operandStack.Pop()))
                        {
                            frame.Pointer += offset;
                        }
                        break;

                    // 函数调用
                    case OpCode.CALL:
                        var argCount = frame.ReadByte();
                        var args = new ScriptObject[argCount];
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            args[i] = _operandStack.Pop();
                        }

                        var callable = _operandStack.Pop();
                        if (callable is Closure closure)
                        {
                            // 创建新的环境
                            var callEnv = new Environment(closure.CapturedEnv);

                            // 创建新的调用帧
                            var callFrame = new CallFrame(closure.Bytecode, 0, callEnv);

                            // 设置参数
                            for (int i = 0; i < args.Length; i++)
                            {
                                callFrame.Locals[i] = args[i];
                            }
                            // 切换到新帧
                            _callStack.Push(callFrame);
                            frame = callFrame;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot call {callable}");
                        }
                        break;

                    // 返回操作
                    case OpCode.RETURN:
                        // 函数返回
                        var returnValue = _operandStack.Count > 0 ? _operandStack.Pop() : null;
                        _callStack.Pop(); // 弹出当前调用帧

                        if (_callStack.Count == 0)
                        {
                            // 如果调用栈为空，则返回最终结果
                            return returnValue;
                        }

                        // 否则，将返回值压入操作数栈，并继续执行调用者的帧
                        _operandStack.Push(returnValue);
                        frame = _callStack.Peek();
                        break;

                    default:
                        throw new NotImplementedException($"OpCode {opCode} not implemented");
                }
            }

            // 如果执行到这里，说明字节码执行完毕但没有明确的返回值
            return null;
        }
    }
}
