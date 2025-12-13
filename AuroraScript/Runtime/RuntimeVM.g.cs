using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Pool;
using AuroraScript.Runtime.Types;
using AuroraScript.Runtime.Types.Internal;
using AuroraScript.Runtime.Util;
using System;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuroraScript.Runtime
{
    internal unsafe partial class RuntimeVM
    {
        private static void NOP(ExecuteFrameContext ctx)
        {
            // vm._codeBuffer
            // ctx.Strings

            // ctx.OperandStack
            // exeContext._callStack
            // exeContext.Domain
            // exeContext.SetStatus
            // exeContext.ExecuteOptions


            // frame.GetArgumentDatum
            // frame.GetLocalDatum
        }

        private static void POP(ExecuteFrameContext ctx)
        {
            //ctx.OperandStack.PopDiscard();

            var stack = ctx.OperandStack;
            int newSize = stack._size - 1;
            if ((uint)newSize >= (uint)stack._size) ThrowHelper.ThrowEmptyStack();
            stack._size = newSize;
            stack._buffer[newSize] = default;

        }


        private static void LOAD_LOCAL(ExecuteFrameContext ctx)
        {
            //var localIndex = ctx.ReadByte();
            //ctx.PushLocal(localIndex);
            ref var frame = ref ctx.CurrentFrame;
            byte index = *(ctx.CodeBasePointer + frame.Pointer++);
            ref ScriptDatum local = ref ctx.Locals[index];
            var stack = ctx.OperandStack;
            int s = stack._size++;
            var buf = stack._buffer;
            buf[s] = local;
        }


        private static void LESS_THAN(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref readonly var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            if (leftSlot.Kind == ValueKind.Number && rightSlot.Kind == ValueKind.Number)
            {
                ScriptDatum.BooleanOf(leftSlot.Number < rightSlot.Number, out leftSlot);
                stack.PopDiscard();
            }
            else
            {
                ScriptDatum.BooleanOf(false, out leftSlot);
                stack.PopDiscard();
            }
        }

        private static void INC_LOCAL_POST(ExecuteFrameContext ctx)
        {
            //var index = ctx.ReadByte();
            //ctx.IncLocal(index);

            ref var frame = ref ctx.CurrentFrame;
            byte index = *(ctx.CodeBasePointer + frame.Pointer++);
            ref var local = ref ctx.Locals[index];
            var origin = ctx.Locals[index];
            local.Number += 1;


            var stack = ctx.OperandStack;
            int s = stack._size;
            var buf = stack._buffer;
            if ((uint)s >= (uint)buf.Length) stack.Grow();
            buf[s] = origin;
            stack._size = s + 1;

        }


        private static void JUMP(ExecuteFrameContext ctx)
        {
            var frame = ctx.CurrentFrame;
            int offset = *(int*)(ctx.CodeBasePointer + frame.Pointer);
            frame.Pointer += (offset + 4);
        }








        private static void DUP(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.Duplicate();
        }
        private static void SWAP(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.Swap();
        }

        private static void LOAD_ARG(ExecuteFrameContext ctx)
        {
            var argIndex = ctx.ReadByte();
            var argDatum = ctx.CurrentFrame.GetArgumentRef(argIndex);
            ctx.OperandStack.PushRef(ref argDatum);
        }

        private static void TRY_LOAD_ARG(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadByte();
            if (ctx.CurrentFrame.TryGetArgumentDatum(propNameIndex, out var tryArgDatum))
            {
                ctx.OperandStack.PopDiscard();
                ctx.OperandStack.PushRef(ref tryArgDatum);
            }
        }




        private static void CREATE_CLOSURE(ExecuteFrameContext ctx)
        {
            //var _codeBuffer = vm._codeBuffer;
            var _operandStack = ctx.OperandStack;
            var closureOffset = ctx.ReadInt32();
            var captureCount = ctx.ReadByte();
            var entryPointer = ctx.CurrentFrame.Pointer + closureOffset;
            ref readonly var moduleObject = ref _operandStack.PopRef();
            var moduleForClosure = moduleObject.Object as ScriptModule;

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
                    ref readonly var upvalueObj = ref _operandStack.PopRef();
                    if (upvalueObj.Object is not Upvalue upvalue)
                    {
                        throw new AuroraVMException("Invalid captured upvalue");
                    }
                    var aliasSlot = upvalue.ConsumeAliasSlot();
                    capturedUpvalues[i] = new ClosureUpvalue(aliasSlot, upvalue);
                }
            }

            var closure = new ClosureFunction(ctx.Domain, moduleForClosure, entryPointer, capturedUpvalues);
            _operandStack.PushDatum(ScriptDatum.FromObject(closure));
        }

        private static void INIT_MODULE(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var module = new ScriptModule(propName.Value);
            ctx.Global.Define("@" + propName.Value, module, writeable: false, enumerable: true);
        }

        private static void NEW_MAP(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            _operandStack.PushDatum(ScriptDatum.FromObject(new ScriptObject()));
        }

        private static void NEW_ARRAY(ExecuteFrameContext ctx)
        {
            var count = ctx.ReadInt32();
            var _operandStack = ctx.OperandStack;
            var newArray = new ScriptArray(count);
            var datumBuffer = new ScriptDatum[count];
            for (int i = count - 1; i >= 0; i--)
            {
                datumBuffer[i] = _operandStack.PopDatum();
            }
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if (ScriptDatum.TryGetObject(in datumBuffer[i], out var obj) && obj is ScriptDeConstruct deConstruct)
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
            _operandStack.PushDatum(ScriptDatum.FromArray(newArray));
        }




        private static void NEW_REGEX(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var _pattern = ctx.ReadInt32();
            var _flags = ctx.ReadInt32();
            var flags = ctx.Strings[_flags];
            var pattern = ctx.Strings[_pattern];
            var regex = RegexManager.Resolve(pattern.Value, flags.Value);
            _operandStack.PushDatum(ScriptDatum.FromRegex(regex));
        }




        private static void DECONSTRUCT_ARRAY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datumValue = ref _operandStack.PopRef();
            if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array)
            {
                var deConstruct = new ScriptDeConstruct(array, ValueKind.Array);
                _operandStack.PushDatum(ScriptDatum.FromObject(deConstruct));
            }
        }


        private static void DECONSTRUCT_MAP(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var source = ref _operandStack.PopRef();
            ref readonly var target = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetObject(in target, out var targetObject))
            {
                if (ScriptDatum.TryGetObject(in source, out var sourceObject))
                {
                    RuntimeHelper.CopyProperties(sourceObject, targetObject, true);
                }
                else if (ScriptDatum.TryGetArray(in source, out var array))
                {
                    var n = array.Length;
                    for (int i = 0; i < n; i++)
                    {
                        var ele = array.Get(i);
                        targetObject.SetPropertyValue(i.ToString(), ScriptDatum.ToObject(in ele));
                    }
                }
            }
        }






        private static void GET_ITERATOR(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datum = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in datum, out var obj) && obj is IEnumerator iterable)
            {
                _operandStack.PushDatum(ScriptDatum.FromObject(iterable.GetIterator()));
            }
            else
            {
                throw new AuroraVMException($"Object {obj} does not support iterators.");
            }
        }
        private static void ITERATOR_VALUE(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datum = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in datum, out var obj) && obj is ItemIterator iterator)
            {
                _operandStack.PushDatum(iterator.Value());
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
        }
        private static void ITERATOR_HAS_VALUE(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datum = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in datum, out var obj) && obj is ItemIterator iterator)
            {
                _operandStack.PushDatum(ScriptDatum.FromBoolean(iterator.HasValue()));
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
        }
        private static void ITERATOR_NEXT(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datum = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in datum, out var obj) && obj is ItemIterator iterator)
            {
                iterator.Next();
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
        }







        private static void TYPEOF(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ScriptDatum.TypeOf(in rightSlot, out rightSlot);
        }


        private static void ADD(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            if (leftSlot.Kind == ValueKind.Number && rightSlot.Kind == ValueKind.Number)
            {
                leftSlot.Number = leftSlot.Number + rightSlot.Number;
                stack.PopDiscard();
            }
            else
            {
                leftSlot = ScriptDatum.FromString(ScriptDatum.ToString(in leftSlot) + ScriptDatum.ToString(in rightSlot));
                stack.PopDiscard();
            }
        }






        private static void ALLOC_LOCALS(ExecuteFrameContext ctx)
        {
            var localsRequested = ctx.ReadInt32();
            ctx.EnsureLocalStorage(localsRequested);
        }



        private static void STORE_LOCAL(ExecuteFrameContext ctx)
        {
            var localIndex = ctx.ReadByte();
            ctx.PopToLocal(localIndex);
            //ref var slot = ref ctx.CurrentFrame.GetLocalRef(localIndex);
            //slot = ctx.OperandStack.PopDatum();
        }


        private static void LOAD_LOCAL_LONG(ExecuteFrameContext ctx)
        {
            var localIndex = ctx.ReadInt32();
            ctx.PushLocal(localIndex);
        }

        private static void STORE_LOCAL_LONG(ExecuteFrameContext ctx)
        {
            var localIndex = ctx.ReadInt32();
            ctx.PopToLocal(localIndex);
        }



        private static void DELETE_PROPERTY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var key = ref _operandStack.PopRef();
            ref readonly var obj = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in obj, out var scriptObject))
            {
                var propName = ExtractPropertyKey(in key);
                scriptObject.DeletePropertyValue(propName);
            }
        }

        private static void GET_THIS_PROPERTY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var value = ctx.Module.GetPropertyValue(propName);
            _operandStack.PushObject(value);
        }
        private static void SET_THIS_PROPERTY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var value = _operandStack.PopObject();
            ctx.Module.SetPropertyValue(propName, value);
        }


        private static void GET_GLOBAL_PROPERTY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            if (ctx.ClrRegistry.TryGetClrType(propName.Value, out var clrType))
            {
                _operandStack.PushDatum(ScriptDatum.FromClrType(clrType));
            }
            else
            {
                var value = ctx.Global.GetPropertyValue(propName);
                _operandStack.PushObject(value);
            }
        }
        private static void SET_GLOBAL_PROPERTY(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var value = _operandStack.PopObject();
            ctx.Global.SetPropertyValue(propName, value);
        }





        private static void GET_ELEMENT(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datumValue = ref _operandStack.PopRef();
            ref readonly var datumObjValue = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetArray(in datumObjValue, out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                scriptArray.Get((Int32)datumValue.Number, out var value);
                _operandStack.PushRef(ref value);
            }
            else if (ScriptDatum.TryGetAnyObject(in datumObjValue, out var datumObj))
            {
                var key = ExtractPropertyKey(in datumValue);
                _operandStack.PushObject(datumObj.GetPropertyValue(key));
            }
            else
            {
                _operandStack.PushNull();
            }
        }

        private static void SET_ELEMENT(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            ref readonly var datumTargetObj = ref _operandStack.PopRef();
            ref readonly var datumValue = ref _operandStack.PopRef();
            ref readonly var datumAssignedValue = ref _operandStack.PopRef();
            if (ScriptDatum.TryGetArray(in datumTargetObj, out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                scriptArray.Set((Int32)datumValue.Number, datumAssignedValue);
            }
            else if (ScriptDatum.TryGetAnyObject(in datumTargetObj, out var datumObj))
            {
                var key = ExtractPropertyKey(in datumValue);
                datumObj.SetPropertyValue(key, ScriptDatum.ToObject(in datumAssignedValue));
            }
        }




        private static void GET_PROPERTY(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            ref var topDatum = ref ctx.OperandStack.PeekRef();
            if (ScriptDatum.TryGetAnyObject(in topDatum, out var obj))
            {
                topDatum = ScriptDatum.FromObject(obj.GetPropertyValue(propName));
            }
            else if (topDatum.Kind == ValueKind.Number || topDatum.Kind == ValueKind.Boolean)
            {
                topDatum = ScriptDatum.FromObject(ScriptDatum.ToObject(in topDatum).GetPropertyValue(propName));
            }
        }

        private static void SET_PROPERTY(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var value = ctx.OperandStack.PopObject();
            ref readonly var datum = ref ctx.OperandStack.PopRef();
            if (ScriptDatum.TryGetAnyObject(in datum, out var obj))
            {
                obj.SetPropertyValue(propName, value);
            }
        }


        private static void CALL(ExecuteFrameContext ctx)
        {
            //var _callStack = exeContext._callStack;
            var _operandStack = ctx.OperandStack;
            ctx.EnsureCallDepth();
            // 函数调用指令
            // 从栈顶弹出可调用对象
            ref readonly var callable = ref _operandStack.PopRef();
            // 读取参数数量
            var argCount = ctx.ReadByte();
            // 创建参数数组
            var callFrame = CallFramePool.Rent();

            var argDatums = callFrame.Arguments.ViewSpan(argCount);
            // 从栈中弹出参数，注意参数顺序是从右到左
            for (int i = argCount - 1; i >= 0; i--)
            {
                _operandStack.PopToRef(ref argDatums[i]);
            }

            if (ScriptDatum.TryGetFunction(in callable, out var closureFunc))
            {
                // 如果是脚本中定义的闭包函数
                // 创建新的调用帧，包含环境、全局对象、模块和入口点
                callFrame.Initialize(ctx.Domain, closureFunc.Module, closureFunc.EntryPointer, closureFunc.CapturedUpvalues);
                // 将新帧压入调用栈
                //_callStack.Push(callFrame);
                // 更新当前帧引用
                ctx.PushCallStack(callFrame);
                //frame = callFrame;
                return;
            }
            else if (ScriptDatum.TryGetClrBonding(in callable, out var callableFunc))
            {
                try
                {
                    ScriptDatum result = ScriptDatum.Null;
                    callableFunc.Invoke(ctx.ExecuteContext, null, argDatums, ref result);
                    _operandStack.PushRef(ref result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CallFramePool.Return(callFrame);
                }
            }
            else if (ScriptDatum.TryGetClrInvokable(in callable, out var clrInvokable))
            {
                try
                {
                    ScriptDatum result = ScriptDatum.Null;
                    clrInvokable.Invoke(ctx.ExecuteContext, ScriptDatum.ToObject(in callable), argDatums, ref result);
                    _operandStack.PushDatum(result);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CallFramePool.Return(callFrame);
                }
            }
            else
            {
                // 如果不是可调用对象，抛出异常
                throw new InvalidOperationException($"Cannot call {callable.Kind}");
            }
        }

        private static void RETURN(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            // 函数返回指令
            var datumValue = _operandStack.Count > 0 ? _operandStack.PopDatum() : ScriptDatum.Null;
            // 弹出当前调用帧
            var callDeep = ctx.PopCallStack();
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (callDeep == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                var value = ScriptDatum.ToObject(in datumValue);
                ctx.SetStatus(ExecuteStatus.Complete, value, null);
                return;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            ctx.OperandStack.PushRef(ref datumValue);
        }



        private static void RETURN_NULL(ExecuteFrameContext ctx)
        {
            //var _operandStack = ctx.OperandStack;
            //var _callStack = exeContext._callStack;

            // 弹出当前调用帧
            var callDeep = ctx.PopCallStack();
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (callDeep == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                ctx.SetStatus(ExecuteStatus.Complete, ScriptObject.Null, null);
                return;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            ctx.OperandStack.PushNull();
            // 切换到调用者的帧继续执行
            //ctx.SwitchFrame();
            //frame = _callStack.Peek();

        }


        private static void CAPTURE_VAR(ExecuteFrameContext ctx)
        {
            var slotIndex = ctx.ReadInt32();
            var capturedUpvalue = ctx.CurrentFrame.GetCapturedUpvalue(slotIndex) ?? ctx.CurrentFrame.GetOrCreateUpvalue(slotIndex);
            capturedUpvalue.MarkAliasSlot(slotIndex);
            ctx.OperandStack.PushDatum(ScriptDatum.FromObject(capturedUpvalue));
        }
        private static void LOAD_CAPTURE(ExecuteFrameContext ctx)
        {
            var localIndex = ctx.ReadInt32();
            var datumValue = ctx.CurrentFrame.GetLocalDatum(localIndex);
            if (datumValue.Kind != ValueKind.Object || datumValue.Object is not Upvalue upvalueToRead)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            ctx.OperandStack.PushDatum(upvalueToRead.Get());
        }

        private static void STORE_CAPTURE(ExecuteFrameContext ctx)
        {
            var datumValue = ctx.OperandStack.PopDatum();
            var localIndex = ctx.ReadInt32();
            var upvalueDatum = ctx.CurrentFrame.GetLocalDatum(localIndex);
            if (upvalueDatum.Kind != ValueKind.Object || upvalueDatum.Object is not Upvalue upvalueToWrite)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            upvalueToWrite.Set(datumValue);
        }

        private static void SUBTRACT(ExecuteFrameContext ctx)
        {

            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                ScriptDatum.NumberOf(leftNumber - rightNumber, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;

        }

        private static void MULTIPLY(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                ScriptDatum.NumberOf(leftNumber * rightNumber, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void DIVIDE(ExecuteFrameContext ctx)
        {

            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                ScriptDatum.NumberOf(leftNumber / rightNumber, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void MOD(ExecuteFrameContext ctx)
        {

            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                ScriptDatum.NumberOf(leftNumber % rightNumber, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void NEGATE(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (ScriptDatum.TryToNumber(in slot, out var value))
            {
                ScriptDatum.NumberOf(-value, out slot);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out slot);
            }
        }

        private static void INCREMENT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (ScriptDatum.TryToNumber(in slot, out var value))
            {
                ScriptDatum.NumberOf(value + 1, out slot);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out slot);
            }
        }

        private static void DECREMENT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (ScriptDatum.TryToNumber(in slot, out var value))
            {
                ScriptDatum.NumberOf(value - 1, out slot);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out slot);
            }
        }

        private static void BIT_NOT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (ScriptDatum.TryToInteger(in slot, out var value))
            {
                ScriptDatum.NumberOf(~value, out slot);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out slot);
            }
        }

        private static void BIT_SHIFT_LEFT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var value = (double)((int)leftNumber << (int)rightNumber);
                ScriptDatum.NumberOf(value, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void BIT_SHIFT_RIGHT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var value = (double)((int)leftNumber >> (int)rightNumber);
                ScriptDatum.NumberOf(value, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void BIT_UNSIGNED_SHIFT_RIGHT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var value = (double)((int)leftNumber >>> (int)rightNumber);
                ScriptDatum.NumberOf(value, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void BIT_AND(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var v = unchecked((Int32)(Int64)leftNumber) & unchecked((Int32)(Int64)rightNumber);
                ScriptDatum.NumberOf(v, out result);
            }
            else if (leftSlot.Kind == ValueKind.Null || rightSlot.Kind == ValueKind.Null)
            {
                ScriptDatum.NumberOf(0, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void BIT_OR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var v = unchecked((Int32)(Int64)leftNumber) | unchecked((Int32)(Int64)rightNumber);
                ScriptDatum.NumberOf(v, out result);
            }
            else if (leftSlot.Kind == ValueKind.Null)
            {
                result = rightSlot;
            }
            else
            {
                result = leftSlot;
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void BIT_XOR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            ScriptDatum result;
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                var v = unchecked((Int32)(Int64)leftNumber) ^ unchecked((Int32)(Int64)rightNumber);
                ScriptDatum.NumberOf(v, out result);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out result);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void LOGIC_NOT(ExecuteFrameContext ctx)
        {
            ref var slot = ref ctx.OperandStack.PeekRef();
            ScriptDatum.BooleanOf(!ScriptDatum.IsTrue(in slot), out slot);
        }

        private static void LOGIC_AND(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = (ScriptDatum.IsTrue(in leftSlot) && ScriptDatum.IsTrue(in rightSlot));
            stack.PopDiscard();
            ScriptDatum.BooleanOf(result, out leftSlot);
        }

        private static void LOGIC_OR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = (ScriptDatum.IsTrue(in leftSlot) || ScriptDatum.IsTrue(in rightSlot));
            stack.PopDiscard();
            ScriptDatum.BooleanOf(result, out leftSlot);
        }

        private static void EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var equals = DatumEquals(leftSlot, rightSlot);
            stack.PopDiscard();
            ScriptDatum.BooleanOf(equals, out leftSlot);

        }

        private static void NOT_EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var equals = DatumEquals(leftSlot, rightSlot);
            stack.PopDiscard();
            ScriptDatum.BooleanOf(!equals, out leftSlot);
        }



        private static void LESS_EQUAL(ExecuteFrameContext ctx)
        {
            //var stack = ctx.OperandStack;
            //ref var rightSlot = ref stack.PeekRef();
            //ref var leftSlot = ref stack.PeekRef(1);
            //if (leftSlot.Kind == rightSlot.Kind && leftSlot.Kind == ValueKind.Number)
            //{
            //    stack.PopDiscard();
            //    leftSlot = ScriptDatum.FromBoolean(leftSlot.Number <= rightSlot.Number);
            //}
            //else
            //{
            //    stack.PopDiscard();
            //    leftSlot = ScriptDatum.False;
            //}


            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber)
                ? leftNumber <= rightNumber
                : false;
            stack.PopDiscard();
            ScriptDatum.BooleanOf(result, out leftSlot);
        }

        private static void GREATER_THAN(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber)
                ? leftNumber > rightNumber
                : false;
            stack.PopDiscard();
            ScriptDatum.BooleanOf(result, out leftSlot);
        }

        private static void GREATER_EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber)
                ? leftNumber >= rightNumber
                : false;
            stack.PopDiscard();
            ScriptDatum.BooleanOf(result, out leftSlot);

        }


        private static void JUMP_IF_FALSE(ExecuteFrameContext ctx)
        {
            var offset = ctx.ReadInt32();
            var isTrue = ctx.OperandStack.PopTopDatumIsTrue();
            if (!isTrue)
            {
                ctx.CurrentFrame.Pointer += offset;
            }
        }

        private static void JUMP_IF_TRUE(ExecuteFrameContext ctx)
        {
            var offset = ctx.ReadInt32();
            var stack = ctx.OperandStack;
            var isTrue = stack.PeekIsTrue();
            stack.PopDiscard();
            if (isTrue)
            {
                ctx.CurrentFrame.Pointer += offset;
            }

        }




        private static void YIELD(ExecuteFrameContext ctx)
        {
            // TODO
            if (ctx.ExecuteOptions.EnabledYield)
            {
                ctx.SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
            }
        }





        #region const

        private static void PUSH_0(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(0);

        }
        private static void PUSH_1(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(1);

        }
        private static void PUSH_2(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(2);

        }
        private static void PUSH_3(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(3);

        }
        private static void PUSH_4(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(4);

        }
        private static void PUSH_5(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(5);

        }
        private static void PUSH_6(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(6);

        }
        private static void PUSH_7(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(7);

        }
        private static void PUSH_8(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(8);

        }
        private static void PUSH_9(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNumber(9);

        }
        private static void PUSH_NULL(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushNull();

        }
        private static void PUSH_FALSE(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushFalse();

        }
        private static void PUSH_TRUE(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushTrue();

        }
        private static void PUSH_THIS(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromObject(ctx.Module));
        }
        private static void PUSH_GLOBAL(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromObject(ctx.Global));
        }
        private static void PUSH_CONTEXT(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromObject(ctx.UserState));

        }
        private static void PUSH_ARGUMENTS(ExecuteFrameContext ctx)
        {
            var argDatum = ScriptDatum.FromArray(new ScriptArray(ctx.CurrentFrame.Arguments.ViewSpan()));
            ctx.OperandStack.PushRef(ref argDatum);

        }





        private static void PUSH_I8(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadSByte(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }
        private static void PUSH_I16(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadInt16(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }
        private static void PUSH_I32(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadInt32(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }
        private static void PUSH_I64(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadInt64(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }
        private static void PUSH_F32(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadFloat(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }
        private static void PUSH_F64(ExecuteFrameContext ctx)
        {
            ScriptDatum.NumberOf(ctx.ReadDouble(), out var num);
            ctx.OperandStack.PushRef(ref num);
        }

        private static void PUSH_STRING(ExecuteFrameContext ctx)
        {
            var stringIndex = ctx.ReadInt32();
            ScriptDatum num = new ScriptDatum() { Kind = ValueKind.String, String = ctx.Strings[stringIndex] };
            ctx.OperandStack.PushRef(ref num);
        }

        private static void INC_LOCAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            var index = ctx.ReadByte();
            ref var slot = ref ctx.CurrentFrame.GetLocalRef(index);
            var original = slot;
            if (!ScriptDatum.TryToNumber(in slot, out var current))
            {
                current = double.NaN;
            }
            var newValue = current + 1;
            stack.PushNumber(newValue);
        }

        private static void INC_LOCAL_L(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: true, delta: 1d, isPostfix: false);
        }

        private static void DEC_LOCAL(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: false, delta: -1d, isPostfix: false);
        }

        private static void DEC_LOCAL_L(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: true, delta: -1d, isPostfix: false);
        }



        private static void INC_LOCAL_POST_L(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: true, delta: 1d, isPostfix: true);
        }

        private static void DEC_LOCAL_POST(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: false, delta: -1d, isPostfix: true);
        }

        private static void DEC_LOCAL_POST_L(ExecuteFrameContext ctx)
        {
            HandleLocalIncrement(ctx, useLongIndex: true, delta: -1d, isPostfix: true);
        }

        private static void ADD_LOCAL_STACK(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: false, operation: LocalStackOp.Add);
        }

        private static void ADD_LOCAL_STACK_L(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: true, operation: LocalStackOp.Add);
        }

        private static void SUB_LOCAL_STACK(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: false, operation: LocalStackOp.Subtract);
        }

        private static void SUB_LOCAL_STACK_L(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: true, operation: LocalStackOp.Subtract);
        }

        private static void MUL_LOCAL_STACK(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: false, operation: LocalStackOp.Multiply);
        }

        private static void MUL_LOCAL_STACK_L(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: true, operation: LocalStackOp.Multiply);
        }

        private static void DIV_LOCAL_STACK(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: false, operation: LocalStackOp.Divide);
        }

        private static void DIV_LOCAL_STACK_L(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: true, operation: LocalStackOp.Divide);
        }

        private static void MOD_LOCAL_STACK(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: false, operation: LocalStackOp.Mod);
        }

        private static void MOD_LOCAL_STACK_L(ExecuteFrameContext ctx)
        {
            HandleLocalBinaryFromStack(ctx, useLongIndex: true, operation: LocalStackOp.Mod);
        }

        private enum LocalStackOp : byte
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Mod
        }

        private static void HandleLocalIncrement(ExecuteFrameContext ctx, bool useLongIndex, double delta, bool isPostfix)
        {
            var stack = ctx.OperandStack;
            var index = useLongIndex ? ctx.ReadInt32() : ctx.ReadByte();
            ref var slot = ref ctx.CurrentFrame.GetLocalRef(index);
            var original = slot;
            if (!ScriptDatum.TryToNumber(in slot, out var current))
            {
                current = double.NaN;
            }
            var newValue = current + delta;
            ScriptDatum.NumberOf(newValue, out slot);
            var result = isPostfix ? original : slot;
            stack.PushDatum(result);
        }

        private static void HandleLocalBinaryFromStack(ExecuteFrameContext ctx, bool useLongIndex, LocalStackOp operation)
        {
            var stack = ctx.OperandStack;
            var index = useLongIndex ? ctx.ReadInt32() : ctx.ReadByte();
            ref var leftSlot = ref ctx.CurrentFrame.GetLocalRef(index);
            ref readonly var rightSlot = ref stack.PopRef();
            switch (operation)
            {
                case LocalStackOp.Add:
                    if (leftSlot.Kind == ValueKind.Number && rightSlot.Kind == ValueKind.Number)
                    {
                        leftSlot.Number = leftSlot.Number + rightSlot.Number;
                    }
                    else
                    {
                        leftSlot = ScriptDatum.FromString(ScriptDatum.ToString(in leftSlot) + ScriptDatum.ToString(in rightSlot));
                    }
                    break;
                case LocalStackOp.Subtract:
                    ApplyBinaryToLocal(ref leftSlot, in rightSlot, BinaryNumberOp.Subtract);
                    break;
                case LocalStackOp.Multiply:
                    ApplyBinaryToLocal(ref leftSlot, in rightSlot, BinaryNumberOp.Multiply);
                    break;
                case LocalStackOp.Divide:
                    ApplyBinaryToLocal(ref leftSlot, in rightSlot, BinaryNumberOp.Divide);
                    break;
                case LocalStackOp.Mod:
                    ApplyBinaryToLocal(ref leftSlot, in rightSlot, BinaryNumberOp.Mod);
                    break;
            }
        }

        private static void ApplyBinaryToLocal(ref ScriptDatum leftSlot, in ScriptDatum rightSlot, BinaryNumberOp operation)
        {
            if (TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber))
            {
                ScriptDatum.NumberOf(ApplyBinaryOp(operation, leftNumber, rightNumber), out leftSlot);
            }
            else
            {
                ScriptDatum.NumberOf(double.NaN, out leftSlot);
            }
        }

        #endregion








    }
}
