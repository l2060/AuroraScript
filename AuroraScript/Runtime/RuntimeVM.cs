using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Immutable;
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
        /// 使用指定的字节码和字符串常量池初始化虚拟机
        /// </summary>
        /// <param name="bytecode">要执行的字节码，由编译器生成的二进制指令序列</param>
        /// <param name="stringConstants">字符串常量池，包含脚本中所有的字符串字面量</param>
        public RuntimeVM(byte[] bytecode, ImmutableArray<String> stringConstants)
        {
            // 创建字节码缓冲区，用于读取和解析字节码指令
            _codeBuffer = new ByteCodeBuffer(bytecode);
            // 将字符串常量转换为StringValue对象并存储在不可变数组中
            _stringConstants = stringConstants.Select(e => StringValue.Of(e)).ToImmutableArray();
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
            ScriptObject temp = null;
            ScriptObject value = null;
            ScriptObject obj = null;
            ScriptObject left = null;
            ScriptObject right = null;

            // 获取当前调用帧
            var frame = _callStack.Peek();

            // 获取当前域的全局对象
            ScriptGlobal domainGlobal = frame.Global;

            var popStack = _operandStack.Pop;
            var pushStack = _operandStack.Push;


            // 数值一元操作的lambda函数，用于简化代码
            // 对栈顶的数值执行一元操作，如果不是数值则返回默认值
            var NumberUnaryOperation = (ScriptObject defaultValue, Func<NumberValue, ScriptObject> operation) =>
            {
                var value = popStack();
                if (value is NumberValue number)
                {
                    // 如果是数值类型，执行操作并将结果压入栈
                    pushStack(operation(number));
                }
                else
                {
                    // 如果不是数值类型，将默认值压入栈
                    pushStack(defaultValue);
                }
            };







            // 数值二元操作的lambda函数，用于简化代码
            // 对栈顶的两个数值执行二元操作，如果不是数值则返回默认值
            var NumberBinaryOperation = (ScriptObject defaultValue, Func<NumberValue, NumberValue, ScriptObject> operation) =>
             {
                 // 弹出栈顶的两个值作为操作数
                 var right = popStack();
                 var left = popStack();
                 if (left is NumberValue leftNumber && right is NumberValue rightNumber)
                 {
                     // 如果两个操作数都是数值类型，执行操作并将结果压入栈
                     var result = operation(leftNumber, rightNumber);
                     if (result == null) throw new Exception();
                     pushStack(result);
                 }
                 else
                 {
                     pushStack(ScriptObject.Null);
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
                        popStack();
                        break;

                    case OpCode.DUP:
                        // 复制栈顶元素
                        var topValue = _operandStack.Peek();
                        pushStack(topValue);
                        break;

                    case OpCode.SWAP:
                        // 交换栈顶两个元素
                        var a = popStack();
                        var b = popStack();
                        pushStack(a);
                        pushStack(b);
                        break;

                    case OpCode.LOAD_ARG:
                        //
                        var argIndex = _codeBuffer.ReadByte(frame);
                        var arg = frame.GetArgument(argIndex);
                        pushStack(arg);
                        break;

                    case OpCode.TRY_LOAD_ARG:
                        propNameIndex = _codeBuffer.ReadByte(frame);
                        if (frame.TryGetArgument(propNameIndex, out arg))
                        {
                            popStack();
                            pushStack(arg);
                        }
                        break;

                    case OpCode.PUSH_I8:
                        pushStack(NumberValue.Of(_codeBuffer.ReadSByte(frame)));
                        break;

                    case OpCode.PUSH_I16:
                        pushStack(NumberValue.Of(_codeBuffer.ReadInt16(frame)));
                        break;

                    case OpCode.PUSH_I32:
                        pushStack(NumberValue.Of(_codeBuffer.ReadInt32(frame)));
                        break;

                    case OpCode.PUSH_I64:
                        pushStack(NumberValue.Of(_codeBuffer.ReadInt64(frame)));
                        break;
                    case OpCode.PUSH_F32:
                        pushStack(NumberValue.Of(_codeBuffer.ReadFloat(frame)));
                        break;

                    case OpCode.PUSH_F64:
                        pushStack(NumberValue.Of(_codeBuffer.ReadDouble(frame)));
                        break;

                    case OpCode.PUSH_STRING:
                        var stringIndex = _codeBuffer.ReadInt32(frame);
                        pushStack(_stringConstants[stringIndex]);
                        break;


                    case OpCode.LOAD_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        pushStack(frame.Locals[localIndex]);
                        break;

                    case OpCode.STORE_LOCAL:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        frame.Locals[localIndex] = popStack();
                        break;

                    case OpCode.LOAD_CAPTURE:
                        localIndex = _codeBuffer.ReadInt32(frame);
                        var capturedVar = frame.Locals[localIndex] as CapturedVariablee;
                        pushStack(capturedVar.Read());
                        break;

                    case OpCode.STORE_CAPTURE:
                        value = popStack();
                        localIndex = _codeBuffer.ReadInt32(frame);
                        capturedVar = frame.Locals[localIndex] as CapturedVariablee;
                        capturedVar.Write(value);
                        break;

                    case OpCode.CREATE_CLOSURE:
                        var thisModule = popStack() as ScriptModule;
                        var closureIndex = _codeBuffer.ReadInt32(frame);
                        var closure = new ClosureFunction(frame, thisModule, frame.Pointer + closureIndex);
                        pushStack(closure);
                        break;

                    case OpCode.CAPTURE_VAR:
                        var varIndex = _codeBuffer.ReadInt32(frame);
                        var cv = new CapturedVariablee(frame.Environment, varIndex);
                        pushStack(cv);
                        break;

                    case OpCode.NEW_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        pushStack(new ScriptModule(propName.Value));
                        break;

                    case OpCode.DEFINE_MODULE:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = popStack();
                        domainGlobal.Define(propName.Value, value, writeable: false);
                        break;

                    case OpCode.NEW_MAP:
                        pushStack(new ScriptObject());
                        break;

                    case OpCode.NEW_ARRAY:
                        var count = _codeBuffer.ReadInt32(frame);
                        var buffer = new ScriptObject[count];
                        for (int i = count - 1; i >= 0; i--)
                        {
                            buffer[i] = popStack();
                        }
                        var array = new ScriptArray(buffer);
                        pushStack(array);
                        break;


                    case OpCode.GET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        obj = popStack();

                        if (obj is ScriptObject scriptObj)
                        {
                            pushStack(scriptObj.GetPropertyValue(propName.Value));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot get property '{propName}' from {obj}");
                        }
                        break;

                    case OpCode.SET_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = popStack();
                        obj = popStack();

                        if (obj is ScriptObject targetScriptObj)
                        {
                            targetScriptObj.SetPropertyValue(propName.Value, (Base.ScriptObject)value);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Cannot set property '{propName}' on {obj}");
                        }
                        break;

                    case OpCode.DELETE_PROPERTY:
                        value = popStack();
                        obj = popStack();
                        obj.DeletePropertyValue(value.ToString());
                        break;
                    case OpCode.GET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = frame.Module.GetPropertyValue(propName.Value);
                        pushStack(value);
                        break;

                    case OpCode.SET_THIS_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = popStack();
                        frame.Module.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = domainGlobal.GetPropertyValue(propName.Value);
                        pushStack(value);
                        break;

                    case OpCode.SET_GLOBAL_PROPERTY:
                        propNameIndex = _codeBuffer.ReadInt32(frame);
                        propName = _stringConstants[propNameIndex];
                        value = popStack();
                        domainGlobal.SetPropertyValue(propName.Value, value);
                        break;

                    case OpCode.GET_ELEMENT:
                        // TODO
                        temp = popStack();
                        obj = popStack();
                        if (obj is ScriptArray scriptArray && temp is NumberValue numberValue)
                        {
                            pushStack(scriptArray.GetElement(numberValue));
                        }
                        else
                        {
                            pushStack(obj.GetPropertyValue(temp.ToString()));
                        }
                        break;

                    case OpCode.SET_ELEMENT:
                        // TODO
                        obj = popStack();
                        temp = popStack();
                        value = popStack();
                        if (obj is ScriptArray scriptArray2)
                        {
                            if (temp is NumberValue numberValue2) scriptArray2.SetElement(numberValue2, value);
                        }
                        else
                        {
                            obj.SetPropertyValue(temp.ToString(), value);
                        }
                        break;
              
                    case OpCode.LOGIC_NOT:
                        value = popStack();
                        pushStack(BooleanValue.Of(!value.IsTrue()));
                        break;
                    case OpCode.LOGIC_AND:
                        right = popStack();
                        left = popStack();
                        pushStack(BooleanValue.Of(left.IsTrue() && right.IsTrue()));
                        break;
                    case OpCode.LOGIC_OR:
                        right = popStack();
                        left = popStack();
                        pushStack(BooleanValue.Of(left.IsTrue() || right.IsTrue()));
                        break;

                    case OpCode.EQUAL:
                        right = popStack();
                        left = popStack();
                        pushStack(BooleanValue.Of(left.Equals(right)));
                        break;

                    case OpCode.NOT_EQUAL:
                        right = popStack();
                        left = popStack();
                        pushStack(BooleanValue.Of(!left.Equals(right)));
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





                    case OpCode.ADD:
                        right = popStack();
                        left = popStack();
                        if (left is NumberValue number1 && right is NumberValue number2)
                        {
                            pushStack(number1 + number2);
                        }
                        else
                        {
                            var result = left + right;
                            pushStack(result);
                        }
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





                    case OpCode.BIT_SHIFT_L:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l << r);
                        break;

                    case OpCode.BIT_SHIFT_R:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l >> r);
                        break;

                    case OpCode.BIT_USHIFT_R:
                        NumberBinaryOperation(NumberValue.NaN, (l, r) => l >>> r);
                        break;

                    case OpCode.BIT_AND:

                        right = popStack();
                        left = popStack();
                        if (left is NumberValue leftOrNumber && right is NumberValue rightOrNumber)
                        {
                            pushStack(leftOrNumber & rightOrNumber);
                        }
                        else if (left == ScriptObject.Null || right == ScriptObject.Null)
                        {
                            pushStack(NumberValue.Zero);
                        }
                        break;

                    case OpCode.BIT_OR:
                        right = popStack();
                        left = popStack();
                        if (left is NumberValue leftNumber && right is NumberValue rightNumber)
                        {
                            pushStack(leftNumber | rightNumber);
                        }
                        else if (left == ScriptObject.Null)
                        {
                            pushStack(right);
                        }
                        else
                        {
                            pushStack(left);
                        }
                        break;

                    case OpCode.BIT_XOR:
                        right = popStack();
                        left = popStack();
                        if (left is NumberValue leftXorNumber && right is NumberValue rightXorNumber)
                        {
                            pushStack(leftXorNumber ^ rightXorNumber);
                        }
                        else if (left == ScriptObject.Null)
                        {
                            pushStack(right);
                        }
                        else
                        {
                            pushStack(left);
                        }
                        break;

                    case OpCode.BIT_NOT:
                        NumberUnaryOperation(NumberValue.Negative1, (v) => ~v);
                        break;





                    case OpCode.JUMP:
                        var offset = _codeBuffer.ReadInt32(frame);
                        frame.Pointer += offset;
                        break;

                    case OpCode.JUMP_IF_FALSE:
                        offset = _codeBuffer.ReadInt32(frame);
                        value = popStack();
                        if (!value.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;
                    case OpCode.JUMP_IF_TRUE:
                        offset = _codeBuffer.ReadInt32(frame);
                        value = popStack();
                        if (value.IsTrue())
                        {
                            frame.Pointer += offset;
                        }
                        break;
                    case OpCode.CALL:
                        // 函数调用指令
                        // 从栈顶弹出可调用对象
                        var callable = popStack();
                        // 读取参数数量
                        var argCount = _codeBuffer.ReadByte(frame);
                        // 创建参数数组
                        var args = new ScriptObject[argCount];
                        // 从栈中弹出参数，注意参数顺序是从右到左
                        for (int i = argCount - 1; i >= 0; i--)
                        {
                            args[i] = popStack();
                        }
                        if (callable is ClosureFunction closureFunc)
                        {
                            if (_callStack.Count > exeContext.ExecuteOptions.MaxCallStackDepth)
                            {
                                throw new RuntimeException("The number of method call stacks exceeds the limit of " + exeContext.ExecuteOptions.MaxCallStackDepth);
                            }
                            // 如果是脚本中定义的闭包函数
                            // 创建新的调用帧，包含环境、全局对象、模块和入口点
                            var callFrame = new CallFrame(closureFunc.Environment, domainGlobal, closureFunc.Module, closureFunc.EntryPointer, args);
                            // 将新帧压入调用栈
                            _callStack.Push(callFrame);
                            // 更新当前帧引用
                            frame = callFrame;
                        }
                        else if (callable is BoundFunction clrFunction)
                        {
                            // 如果是绑定的CLR函数（本地函数）
                            // 直接调用函数并将结果压入栈
                            var callResult = clrFunction.Invoke(null, null, args);
                            pushStack(callResult);
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
                        value = _operandStack.Count > 0 ? popStack() : ScriptObject.Null;
                        // 弹出当前调用帧
                        _callStack.Pop();

                        // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
                        if (_callStack.Count == 0)
                        {
                            // 设置执行状态为完成，并返回最终结果
                            exeContext.SetStatus(ExecuteStatus.Complete, value, null);
                            return;
                        }

                        // 如果调用栈不为空，说明是从子函数返回到调用者
                        // 将返回值压入操作数栈，供调用者使用
                        pushStack(value);
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
                        pushStack(NumberValue.Zero);
                        break;
                    case OpCode.PUSH_1:
                        pushStack(NumberValue.Num1);
                        break;
                    case OpCode.PUSH_2:
                        pushStack(NumberValue.Num2);
                        break;
                    case OpCode.PUSH_3:
                        pushStack(NumberValue.Num3);
                        break;
                    case OpCode.PUSH_4:
                        pushStack(NumberValue.Num4);
                        break;
                    case OpCode.PUSH_5:
                        pushStack(NumberValue.Num5);
                        break;
                    case OpCode.PUSH_6:
                        pushStack(NumberValue.Num6);
                        break;
                    case OpCode.PUSH_7:
                        pushStack(NumberValue.Num7);
                        break;
                    case OpCode.PUSH_8:
                        pushStack(NumberValue.Num8);
                        break;
                    case OpCode.PUSH_9:
                        pushStack(NumberValue.Num9);
                        break;
                    case OpCode.PUSH_NULL:
                        pushStack(ScriptObject.Null);
                        break;
                    case OpCode.PUSH_FALSE:
                        pushStack(BooleanValue.False);
                        break;
                    case OpCode.PUSH_TRUE:
                        pushStack(BooleanValue.True);
                        break;
                    case OpCode.PUSH_THIS:
                        pushStack(frame.Module);
                        break;
                    case OpCode.PUSH_GLOBAL:
                        pushStack(domainGlobal);
                        break;
                }
            }
        }
    }
}
