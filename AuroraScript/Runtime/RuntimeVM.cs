using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// AuroraScript 运行时虚拟机，负责执行字节码并管理运行时环境
    /// </summary>
    /// 
    internal class RuntimeVM
    {

        // 全局环境，存储全局变量和函数
        private readonly Environment _globalEnv;

        // 字符串常量池
        private readonly ImmutableArray<StringValue> _stringConstants;

        // 当前执行的字节码
        private readonly ByteCodeBuffer _codeBuffer;




        /// <summary>
        /// 使用指定的字节码和字符串常量池初始化虚拟机
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="stringConstants">字符串常量池</param>
        public RuntimeVM(byte[] bytecode, ImmutableArray<String> stringConstants)
        {
            _globalEnv = new Environment(null);
            _codeBuffer = new ByteCodeBuffer(bytecode);
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
        }


        /// <summary>
        /// 执行已加载的字节码
        /// </summary>
        /// <returns>执行结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptObject Execute(ExecuteContext exeContext)
        {
            var mainFrame = new CallFrame(0, _globalEnv, _globalEnv);
            exeContext._callStack.Push(mainFrame);
            return ExecuteFrame(exeContext);
        }

        /// <summary>
        /// 执行二元操作
        /// </summary>
        /// <param name="operation">要执行的操作</param>
        private void PerformBinaryOperation(Func<ScriptObject, ScriptObject, ScriptObject> operation)
        {
            //var right = _operandStack.Pop();
            //var left = _operandStack.Pop();
            //var result = operation(left, right);
            //_operandStack.Push(result);
        }





        /// <summary>
        /// 执行当前调用帧
        /// </summary>
        /// <returns>执行结果</returns>
        private ScriptObject ExecuteFrame(ExecuteContext exeContext)
        {
            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;
            Int32 propNameIndex = 0;
            NumberValue numValue = null;
            StringValue propName = null;
            ScriptObject value = null;
            ScriptObject obj = null;

            // 获取当前调用帧
            var frame = _callStack.Peek();

            // 执行循环
            while (true)
            {
                // 读取操作码
                var opCode = _codeBuffer.ReadOpCode(frame);
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

                    case OpCode.NEW_MAP:
                        // 交换栈顶两个元素
                        _operandStack.Push(new ScriptObject());
                        break;
                    case OpCode.CREATE_CLOSURE:
                        // 创建闭包
                        var thsiModule = _operandStack.Pop();
                        var closureIndex = _codeBuffer.ReadInt32(frame);
                        var closure = new Closure(frame.Environment, thsiModule, frame.Pointer + closureIndex);
                        _operandStack.Push(closure);
                        break;

                    case OpCode.LOAD_ARG:
                        // 
                        var argIndex = _codeBuffer.ReadByte(frame);
                        var arg = frame.Arguments[argIndex];
                        _operandStack.Push(arg);

                        //var thsiModule = _operandStack.Pop();
                    
                        //var closure = new Closure(frame.Environment, thsiModule, frame.Pointer + closureIndex);
                        //
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
                        _operandStack.Push(frame.ThisModule);
                        break;

                    case OpCode.PUSH_I8:
                        _operandStack.Push(NumberValue.Of(_codeBuffer.ReadSByte(frame)));
                        break;

                    case OpCode.PUSH_I16:
                        _operandStack.Push(NumberValue.Of(_codeBuffer.ReadInt16(frame)));
                        break;

                    case OpCode.PUSH_I32:
                        _operandStack.Push(NumberValue.Of(_codeBuffer.ReadInt32(frame)));
                        break;

                    case OpCode.PUSH_F32:
                        _operandStack.Push(NumberValue.Of(_codeBuffer.ReadFloat(frame)));
                        break;

                    case OpCode.PUSH_F64:
                        _operandStack.Push(NumberValue.Of(_codeBuffer.ReadDouble(frame)));
                        break;

                    case OpCode.PUSH_STRING:
                        var stringIndex = _codeBuffer.ReadInt32(frame);
                        _operandStack.Push(_stringConstants[stringIndex]);
                        break;

                    // 局部变量操作
                    case OpCode.PUSH_LOCAL:
                        var localIndex = _codeBuffer.ReadInt32(frame);
                        _operandStack.Push(frame.Locals[localIndex]);
                        break;

                    case OpCode.POP_TO_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        frame.Locals[localIndex] = _operandStack.Pop();
                        break;

                    // 全局变量操作
                    case OpCode.PUSH_GLOBAL:
                        _operandStack.Push(_globalEnv);
                        break;

                    case OpCode.GET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _globalEnv.GetPropertyValue(propName.Value);
                        _operandStack.Push(value);
                        break;

                    case OpCode.SET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _operandStack.Pop();
                        _globalEnv.SetPropertyValue(propName.Value, value);
                        break;


                    case OpCode.SET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _operandStack.Pop();
                        frame.ThisModule.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = frame.ThisModule.GetPropertyValue(propName.Value);
                        _operandStack.Push(value);
                        break;

                    // 属性操作
                    case OpCode.GET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        obj = _operandStack.Pop();

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
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _operandStack.Pop();
                        obj = _operandStack.Pop();

                        if (obj is Environment targetEnv)
                        {
                            targetEnv.Define(propName.Value, value);
                        }
                        else if (obj is ScriptObject targetScriptObj)
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
                        var count = _codeBuffer.ReadInt32(frame);
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

                    case OpCode.INCREMENT:
                        value = _operandStack.Pop();
                        if (value is NumberValue num)
                        {
                            _operandStack.Push(num + 1);
                        }
                        else
                        {
                            _operandStack.Push(ScriptObject.Null);
                        }
                        break;


                    case OpCode.DECREMENT:
                        value = _operandStack.Pop();
                        if (value is NumberValue num2)
                        {
                            _operandStack.Push(num2 - 1);
                        }
                        else
                        {
                            _operandStack.Push(ScriptObject.Null);
                        }
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
                        var offset = _codeBuffer.ReadInt32(frame);
                        frame.Pointer += offset;
                        break;

                    case OpCode.JUMP_IF_FALSE:
                        offset = _codeBuffer.ReadInt32(frame);
                        if (!Convert.ToBoolean(_operandStack.Pop()))
                        {
                            frame.Pointer += offset;
                        }
                        break;

                    case OpCode.JUMP_IF_TRUE:
                        offset = _codeBuffer.ReadInt32(frame);
                        if (Convert.ToBoolean(_operandStack.Pop()))
                        {
                            frame.Pointer += offset;
                        }
                        break;

                    // 函数调用
                    case OpCode.CALL:

                        var callable = _operandStack.Pop();

                        var argCount = _codeBuffer.ReadByte(frame);
                        var args = new ScriptObject[argCount];
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            args[i] = _operandStack.Pop();
                        }

  
                        if (callable is Closure closureFunc)
                        {
                            // 创建新的环境
                            var callEnv = new Environment(closureFunc.CapturedEnv);

                            // 创建新的调用帧
                            var callFrame = new CallFrame(closureFunc.EntryPointer, callEnv, closureFunc.ThisModule);


                            // 设置参数
                            callFrame.Arguments = args;
                            //for (int i = 0; i < args.Length; i++)
                            //{
                            //    callFrame.Locals[i] = args[i];
                            //}
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

                    case OpCode.PUSH_0:
                        _operandStack.Push(NumberValue.Num8);
                        break;

                    case OpCode.PUSH_1:
                        _operandStack.Push(NumberValue.Num1);
                        break;
                    case OpCode.PUSH_2:
                        _operandStack.Push(NumberValue.Num2);
                        break;
                    case OpCode.PUSH_3:
                        _operandStack.Push(NumberValue.Num3);
                        break;
                    case OpCode.PUSH_4:
                        _operandStack.Push(NumberValue.Num4);
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
