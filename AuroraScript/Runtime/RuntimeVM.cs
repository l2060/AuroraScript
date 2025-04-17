using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// AuroraScript 运行时虚拟机，负责执行字节码并管理运行时环境
    /// </summary>
    /// 
    internal class RuntimeVM
    {
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
            _codeBuffer = new ByteCodeBuffer(bytecode);
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
        }





        /// <summary>
        /// 执行已加载的字节码
        /// </summary>
        /// <returns>执行结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptGlobal CreateDomain(ExecuteContext exeContext)
        {
            var domainGlobal = new ScriptGlobal() { _prototype = exeContext.Global };
            var mainFrame = new CallFrame(null, domainGlobal, null, 0);
            exeContext._callStack.Push(mainFrame);
            return ExecuteFrame(exeContext) as ScriptGlobal;
        }

        /// <summary>
        /// 执行已加载的字节码
        /// </summary>
        /// <returns>执行结果</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptObject Execute(ClosureFunction closure, params ScriptObject[] arguments)
        {
            ExecuteContext exeContext = new ExecuteContext(closure.Global);
            var mainFrame = new CallFrame(closure.Environment, closure.Global, closure.Module, closure.EntryPointer, arguments);
            exeContext._callStack.Push(mainFrame);
            return ExecuteFrame(exeContext);
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
            Int32 localIndex = 0;

            StringValue propName = null;
            ScriptObject temp = null;
            ScriptObject value = null;
            ScriptObject obj = null;
            ScriptObject left = null;
            ScriptObject right = null;
            // 获取当前调用帧
            var frame = _callStack.Peek();
            ScriptGlobal domainGlobal = frame.Global;


            var NumberUnaryOperation = (ScriptObject defaultValue, Func<NumberValue, ScriptObject> operation) =>
            {
                var value = _operandStack.Pop();
                if (value is NumberValue number)
                {
                    _operandStack.Push(operation(number));
                }
                else
                {
                    _operandStack.Push(defaultValue);
                }
            };



            var NumberBinaryOperation = (ScriptObject defaultValue, Func<NumberValue, NumberValue, ScriptObject> operation) =>
             {
                 var right = _operandStack.Pop();
                 var left = _operandStack.Pop();

                 if (left is NumberValue leftNumber && right is NumberValue rightNumber)
                 {
                     var result = operation(leftNumber, rightNumber);
                     _operandStack.Push(result);
                 }
                 else
                 {
                     _operandStack.Push(defaultValue);
                 }
             };



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

                    case OpCode.LOAD_ARG:
                        // 
                        var argIndex = _codeBuffer.ReadByte(frame);
                        var arg = frame.GetArgument(argIndex);
                        _operandStack.Push(arg);
                        break;

                    case OpCode.TRY_LOAD_ARG:
                        propNameIndex = _codeBuffer.ReadByte(frame);
                        if (frame.TryGetArgument(propNameIndex, out arg))
                        {
                            _operandStack.Pop();
                            _operandStack.Push(arg);
                        }
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


                    case OpCode.LOAD_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        _operandStack.Push(frame.Locals[localIndex]);
                        break;

                    case OpCode.STORE_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        frame.Locals[localIndex] = _operandStack.Pop();
                        break;


                    case OpCode.CREATE_CLOSURE:
                        // 创建闭包
                        var thsiModule = _operandStack.Pop() as ScriptModule;
                        var closureIndex = _codeBuffer.ReadInt32(frame);
                        var closure = new ClosureFunction(frame, domainGlobal, thsiModule, frame.Pointer + closureIndex);
                        _operandStack.Push(closure);
                        break;


                    case OpCode.CAPTURE_VAR:
                        // 捕获变量
                        var varIndex = _codeBuffer.ReadInt32(frame);
                        var cv = new CapturedVariablee(frame.Environment, varIndex);
                        _operandStack.Push(cv);
                        break;

                    case OpCode.LOAD_CAPTURE:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        var capturedVar = frame.Locals[localIndex] as CapturedVariablee;
                        _operandStack.Push(capturedVar.Read());
                        break;

                    case OpCode.NEW_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        _operandStack.Push(new ScriptModule(propName.Value));
                        break;

                    case OpCode.NEW_MAP:
                        _operandStack.Push(new ScriptObject());
                        break;

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


                    case OpCode.GET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        obj = _operandStack.Pop();

                        if (obj is ScriptObject scriptObj)
                        {
                            _operandStack.Push(scriptObj.GetPropertyValue(propName.Value, scriptObj));
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

                        if (obj is ScriptObject targetScriptObj)
                        {
                            targetScriptObj.SetPropertyValue(propName.Value, (Base.ScriptObject)value);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot set property '{propName}' on {obj}");
                        }
                        break;


                    case OpCode.GET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = frame.Module.GetPropertyValue(propName.Value, frame.Module);
                        _operandStack.Push(value);
                        break;

                    case OpCode.SET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _operandStack.Pop();
                        frame.Module.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = domainGlobal.GetPropertyValue(propName.Value, domainGlobal);
                        _operandStack.Push(value);
                        break;

                    case OpCode.SET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = _operandStack.Pop();
                        domainGlobal.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_ELEMENT:
                        // TODO
                        temp = _operandStack.Pop();
                        obj = _operandStack.Pop();
                        if (obj is ScriptArray scriptArray && temp is NumberValue numberValue)
                        {
                            _operandStack.Push(scriptArray.GetElement(numberValue));
                        }
                        else
                        {
                            _operandStack.Push(obj.GetPropertyValue(temp.ToString(), obj));
                        }
                        break;

                    case OpCode.SET_ELEMENT:
                        // TODO
                        obj = _operandStack.Pop();
                        temp = _operandStack.Pop();
                        value = _operandStack.Pop();
                        if (obj is ScriptArray scriptArray2)
                        {
                            if (temp is NumberValue numberValue2) scriptArray2.SetElement(numberValue2, value);
                        }
                        else
                        {
                            obj.SetPropertyValue(temp.ToString(), value);
                        }
                        break;

                    // 算术操作
                    case OpCode.ADD:
                        right = _operandStack.Pop();
                        left = _operandStack.Pop();
                        var result = left + right;
                        _operandStack.Push(result);
                        break;
                    case OpCode.SUBTRACT:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l - r);
                        break;
                    case OpCode.MULTIPLY:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l * r);
                        break;
                    case OpCode.DIVIDE:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l / r);
                        break;
                    case OpCode.MOD:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l % r);
                        break;
                    case OpCode.NEGATE:
                        NumberUnaryOperation(NumberValue.NaN, (v) => -v);
                        break;
                    case OpCode.INCREMENT:
                        NumberUnaryOperation(NumberValue.NaN, (v) => v + 1);
                        break;
                    case OpCode.DECREMENT:
                        NumberUnaryOperation(NumberValue.NaN, (v) => v - 1);
                        break;
                    case OpCode.LOGIC_NOT:
                        value = _operandStack.Pop();
                        _operandStack.Push(BooleanValue.Of(!value.IsTrue()));
                        break;
                    case OpCode.LOGIC_AND:
                        right = _operandStack.Pop();
                        left = _operandStack.Pop();
                        _operandStack.Push(BooleanValue.Of(left.IsTrue() && right.IsTrue()));
                        break;
                    case OpCode.LOGIC_OR:
                        right = _operandStack.Pop();
                        left = _operandStack.Pop();
                        _operandStack.Push(BooleanValue.Of(left.IsTrue() || right.IsTrue()));
                        break;

                    case OpCode.EQUAL:
                        right = _operandStack.Pop();
                        left = _operandStack.Pop();
                        // TODO 
                        _operandStack.Push(BooleanValue.Of(left == right));
                        break;

                    case OpCode.NOT_EQUAL:
                        right = _operandStack.Pop();
                        left = _operandStack.Pop();
                        // TODO 
                        _operandStack.Push(BooleanValue.Of(left != right));
                        break;

                    case OpCode.LESS_THAN:
                        NumberBinaryOperation(BooleanValue.False, (l, r) => l < r);
                        break;

                    case OpCode.LESS_EQUAL:
                        NumberBinaryOperation(BooleanValue.False, (l, r) => l <= r);
                        break;

                    case OpCode.GREATER_THAN:
                        NumberBinaryOperation(BooleanValue.False, (l, r) => l > r);
                        break;
                    case OpCode.GREATER_EQUAL:
                        NumberBinaryOperation(BooleanValue.False, (l, r) => l >= r);
                        break;

                    case OpCode.BIT_SHIFT_L:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l << r);
                        break;

                    case OpCode.BIT_SHIFT_R:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l >> r);
                        break;

                    case OpCode.BIT_AND:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l & r);
                        break;

                    case OpCode.BIT_OR:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l | r);
                        break;

                    case OpCode.BIT_XOR:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l ^ r);
                        break;

                    case OpCode.BIT_NOT:
                        NumberUnaryOperation(NumberValue.NaN, (v) => ~v);
                        break;


                    case OpCode.JUMP:
                        var offset = _codeBuffer.ReadInt32(frame);
                        frame.Pointer += offset;
                        break;

                    case OpCode.JUMP_IF_FALSE:
                        offset = _codeBuffer.ReadInt32(frame);
                        value = _operandStack.Pop();
                        if (!value.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;
                    case OpCode.JUMP_IF_TRUE:
                        offset = _codeBuffer.ReadInt32(frame);
                        value = _operandStack.Pop();
                        if (value.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;

                    case OpCode.CALL:
                        var callable = _operandStack.Pop();
                        var argCount = _codeBuffer.ReadByte(frame);
                        var args = new ScriptObject[argCount];
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            args[i] = _operandStack.Pop();
                        }
                        if (callable is ClosureFunction closureFunc)
                        {
                            // 创建新的调用帧
                            var callFrame = new CallFrame(closureFunc.Environment, closureFunc.Global, closureFunc.Module, closureFunc.EntryPointer, args);
                            // 切换到新帧
                            _callStack.Push(callFrame);
                            frame = callFrame;
                        }
                        else if (callable is BoundFunction clrFunction)
                        {
                            var callResult = clrFunction.Invoke(null, null, args);
                            _operandStack.Push(callResult);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot call {callable}");
                        }
                        break;

                    case OpCode.RETURN:
                        // 函数返回
                        value = _operandStack.Count > 0 ? _operandStack.Pop() : null;
                        _callStack.Pop(); // 弹出当前调用帧
                        // 如果调用栈为空，则返回最终结果
                        if (_callStack.Count == 0) return value;
                        // 否则，将返回值压入操作数栈，并继续执行调用者的帧
                        _operandStack.Push(value);
                        frame = _callStack.Peek();
                        break;

                    case OpCode.YIELD:
                        // TODO


                        break;

                    case OpCode.PUSH_0:
                        _operandStack.Push(NumberValue.Zero);
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
                    case OpCode.PUSH_5:
                        _operandStack.Push(NumberValue.Num5);
                        break;
                    case OpCode.PUSH_6:
                        _operandStack.Push(NumberValue.Num6);
                        break;
                    case OpCode.PUSH_7:
                        _operandStack.Push(NumberValue.Num7);
                        break;
                    case OpCode.PUSH_8:
                        _operandStack.Push(NumberValue.Num8);
                        break;
                    case OpCode.PUSH_9:
                        _operandStack.Push(NumberValue.Num9);
                        break;
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
                        _operandStack.Push(frame.Module);
                        break;
                    case OpCode.PUSH_GLOBAL:
                        _operandStack.Push(domainGlobal);
                        break;




                }



            }

            // 如果执行到这里，说明字节码执行完毕但没有明确的返回值
            return null;
        }
    }
}
