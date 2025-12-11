using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            if ((uint)newSize >= (uint)stack._size) stack.ThrowEmpty();
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
            int s = stack._size;
            var buf = stack._buffer;
            if ((uint)s >= (uint)buf.Length) stack.Grow();
            buf[s] = local;
            stack._size = s + 1;
        }


        private static void LESS_THAN(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            if (leftSlot.Kind == ValueKind.Number && rightSlot.Kind == ValueKind.Number)
            {
                leftSlot = ScriptDatum.FromBoolean(leftSlot.Number < rightSlot.Number);
                stack.PopDiscard();
            }
            else
            {
                leftSlot = ScriptDatum.FromBoolean(false);
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
            //var offset = ctx.ReadInt32();
            //ctx.CurrentFrame.Pointer += offset;

            ref var frame = ref ctx.CurrentFrame;
            int offset = *(int*)(ctx.CodeBasePointer + frame.Pointer);
            frame.Pointer += 4;  // 读取 4 字节
            frame.Pointer += offset;
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
            var argDatum = ctx.CurrentFrame.GetArgumentDatum(argIndex);
            ctx.OperandStack.PushDatum(argDatum);
        }

        private static void TRY_LOAD_ARG(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadByte();
            if (ctx.CurrentFrame.TryGetArgumentDatum(propNameIndex, out var tryArgDatum))
            {
                ctx.OperandStack.PopDiscard();
                ctx.OperandStack.PushDatum(tryArgDatum);
            }
        }




        private static void CREATE_CLOSURE(ExecuteFrameContext ctx)
        {
            //var _codeBuffer = vm._codeBuffer;
            var _operandStack = ctx.OperandStack;
            var closureOffset = ctx.ReadInt32();
            var captureCount = ctx.ReadByte();
            var entryPointer = ctx.CurrentFrame.Pointer + closureOffset;
            var moduleObject = _operandStack.PopDatum();
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
                    var upvalueObj = _operandStack.PopDatum();
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
                if (datumBuffer[i].TryGetObject(out var obj) && obj is ScriptDeConstruct deConstruct)
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
            var datumValue = _operandStack.PopDatum();
            if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array)
            {
                var deConstruct = new ScriptDeConstruct(array, ValueKind.Array);
                _operandStack.PushDatum(ScriptDatum.FromObject(deConstruct));
            }
        }


        private static void DECONSTRUCT_MAP(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var datumValue = _operandStack.PopDatum();
            var obj = _operandStack.PopDatum();

            if (obj.TryGetObject(out var value))
            {
                if (datumValue.TryGetObject(out var scriptObject))
                {
                    value.CopyPropertysFrom(scriptObject, true);
                }
                else if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array1)
                {
                    for (int i = 0; i < array1.Length; i++)
                    {
                        var ele = array1.Get(i);
                        value.SetPropertyValue(i.ToString(), ele.ToObject());
                    }
                }
            }
        }






        private static void GET_ITERATOR(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var datum = _operandStack.PopDatum();
            if (datum.TryGetAnyObject(out var obj) && obj is IEnumerator iterable)
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
            var datum = _operandStack.PopDatum();
            if (datum.TryGetAnyObject(out var obj) && obj is ItemIterator iterator)
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
            var datum = _operandStack.PopDatum();
            if (datum.TryGetAnyObject(out var obj) && obj is ItemIterator iterator)
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
            var datum = _operandStack.PopDatum();
            if (datum.TryGetAnyObject(out var obj) && obj is ItemIterator iterator)
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
            rightSlot = rightSlot.TypeOf();
            //datumRight = PopDatum();
            //PushDatum(datumRight.TypeOf());
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
                leftSlot = ScriptDatum.FromString(leftSlot.ToString() + rightSlot.ToString());
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
            var key = _operandStack.PopDatum();
            var obj = _operandStack.PeekDatum();
            if (obj.TryGetAnyObject(out var scriptObject))
            {
                var propName = ExtractPropertyKey(ref key);
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
            var datumValue = _operandStack.PopDatum();
            var datumObjValue = _operandStack.PopDatum();
            if (datumObjValue.TryGetArray(out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                _operandStack.PushDatum(scriptArray.Get((Int32)datumValue.Number));
            }
            else if (datumObjValue.TryGetAnyObject(out var datumObj))
            {
                var key = ExtractPropertyKey(ref datumValue);
                _operandStack.PushObject(datumObj.GetPropertyValue(key));
            }
            else
            {
                _operandStack.PushDatum(ScriptDatum.Null);
            }
        }

        private static void SET_ELEMENT(ExecuteFrameContext ctx)
        {
            var _operandStack = ctx.OperandStack;
            var datumTargetObj = _operandStack.PopDatum();
            var datumValue = _operandStack.PopDatum();
            var datumAssignedValue = _operandStack.PopDatum();
            if (datumTargetObj.TryGetArray(out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                scriptArray.Set((Int32)datumValue.Number, datumAssignedValue);
            }
            else if (datumTargetObj.TryGetAnyObject(out var datumObj))
            {
                var key = ExtractPropertyKey(ref datumValue);
                datumObj.SetPropertyValue(key, datumAssignedValue.ToObject());
            }
        }




        private static void GET_PROPERTY(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            ref var topDatum = ref ctx.OperandStack.PeekRef();
            if (topDatum.TryGetAnyObject(out var obj))
            {
                topDatum = ScriptDatum.FromObject(obj.GetPropertyValue(propName));
            }
            else if (topDatum.Kind == ValueKind.Number || topDatum.Kind == ValueKind.Boolean)
            {
                topDatum = ScriptDatum.FromObject(topDatum.ToObject().GetPropertyValue(propName));
            }
        }

        private static void SET_PROPERTY(ExecuteFrameContext ctx)
        {
            var propNameIndex = ctx.ReadInt32();
            var propName = ctx.Strings[propNameIndex];
            var value = ctx.OperandStack.PopObject();
            if (ctx.OperandStack.PopDatum().TryGetAnyObject(out var obj))
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
            var callable = _operandStack.PopDatum();
            // 读取参数数量
            var argCount = ctx.ReadByte();
            // 创建参数数组
            var callFrame = CallFramePool.Rent();

            var argDatums = callFrame.Arguments.ViewSpan(argCount);
            // 从栈中弹出参数，注意参数顺序是从右到左
            for (int i = argCount - 1; i >= 0; i--)
            {
                argDatums[i] = _operandStack.PopDatum();
            }

            if (callable.TryGetFunction(out var closureFunc))
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
            else if (callable.TryGetClrBonding(out var callableFunc))
            {
                try
                {
                    ScriptDatum result = ScriptDatum.Null;
                    callableFunc.Invoke(ctx.ExecuteContext, null, argDatums, ref result);
                    _operandStack.PushDatum(result);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    CallFramePool.Return(callFrame);
                    //ArrayPool<ScriptDatum>.Shared.Return(argDatums);
                }
            }
            else if (callable.TryGetClrInvokable(out var clrInvokable))
            {
                try
                {
                    ScriptDatum result = ScriptDatum.Null;
                    clrInvokable.Invoke(ctx.ExecuteContext, callable.ToObject(), argDatums, ref result);
                    _operandStack.PushDatum(result);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    CallFramePool.Return(callFrame);
                    //ArrayPool<ScriptDatum>.Shared.Return(argDatums);
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
            //var _callStack = ctx.OperandStack;
            var _operandStack = ctx.OperandStack;
            // 函数返回指令
            // 获取返回值（如果有）
            var datumValue = _operandStack.Count > 0 ? _operandStack.PopDatum() : ScriptDatum.Null;
            // 弹出当前调用帧
            var callDeep = ctx.PopCallStack();

            //var finishedFrame = _callStack.Pop();
            //CallFramePool.Return(finishedFrame);
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (callDeep == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                var value = datumValue.ToObject();
                ctx.SetStatus(ExecuteStatus.Complete, value, null);
                return;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            ctx.OperandStack.PushRef(ref datumValue);
            // 切换到调用者的帧继续执行
            //frame = _callStack.Peek();
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
            ctx.OperandStack.PushDatum(ScriptDatum.Null);
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
                result = ScriptDatum.FromNumber(leftNumber - rightNumber);
            }
            else
            {
                result = ScriptDatum.FromNumber(double.NaN);
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
                result = ScriptDatum.FromNumber(leftNumber * rightNumber);
            }
            else
            {
                result = ScriptDatum.FromNumber(double.NaN);
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
                result = ScriptDatum.FromNumber(leftNumber / rightNumber);
            }
            else
            {
                result = ScriptDatum.FromNumber(double.NaN);
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
                result = ScriptDatum.FromNumber(leftNumber % rightNumber);
            }
            else
            {
                result = ScriptDatum.FromNumber(double.NaN);
            }
            stack.PopDiscard();
            leftSlot = result;
        }

        private static void NEGATE(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (slot.TryGetNumber(out var value))
            {
                slot = ScriptDatum.FromNumber(ApplyUnaryOp(UnaryNumberOp.Negate, value));
            }
            else
            {
                slot = ScriptDatum.FromNumber(double.NaN);
            }
        }

        private static void INCREMENT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (slot.TryGetNumber(out var value))
            {
                slot = ScriptDatum.FromNumber(ApplyUnaryOp(UnaryNumberOp.Increment, value));
            }
            else
            {
                slot = ScriptDatum.FromNumber(double.NaN);
            }
        }

        private static void DECREMENT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (slot.TryGetNumber(out var value))
            {
                slot = ScriptDatum.FromNumber(ApplyUnaryOp(UnaryNumberOp.Decrement, value));
            }
            else
            {
                slot = ScriptDatum.FromNumber(double.NaN);
            }
        }

        private static void BIT_NOT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var slot = ref stack.PeekRef();
            if (slot.TryGetInteger(out var value))
            {
                slot = ScriptDatum.FromNumber(~value);
            }
            else
            {
                slot = ScriptDatum.FromNumber(double.NaN);
            }
        }

        private static void BIT_SHIFT_LEFT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void BIT_SHIFT_RIGHT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void BIT_UNSIGNED_SHIFT_RIGHT(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void BIT_AND(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void BIT_OR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void BIT_XOR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
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

        }

        private static void LOGIC_NOT(ExecuteFrameContext ctx)
        {
            ref var slot = ref ctx.OperandStack.PeekRef();
            slot = ScriptDatum.FromBoolean(!slot.IsTrue());
        }

        private static void LOGIC_AND(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = (leftSlot.IsTrue() && rightSlot.IsTrue());
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(result);

        }

        private static void LOGIC_OR(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = (leftSlot.IsTrue() || rightSlot.IsTrue());
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(result);

        }

        private static void EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var equals = DatumEquals(leftSlot, rightSlot);
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(equals);

        }

        private static void NOT_EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var equals = DatumEquals(leftSlot, rightSlot);
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(!equals);

        }



        private static void LESS_EQUAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            ref var rightSlot = ref stack.PeekRef();
            ref var leftSlot = ref stack.PeekRef(1);
            var result = TryGetBinaryNumbers(in leftSlot, in rightSlot, out var leftNumber, out var rightNumber)
                ? leftNumber <= rightNumber
                : false;
            stack.PopDiscard();
            leftSlot = ScriptDatum.FromBoolean(result);
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
            leftSlot = ScriptDatum.FromBoolean(result);
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
            leftSlot = ScriptDatum.FromBoolean(result);

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
            var isTrue = stack.PeekRef().IsTrue();
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
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(0));

        }
        private static void PUSH_1(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(1));

        }
        private static void PUSH_2(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(2));

        }
        private static void PUSH_3(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(3));

        }
        private static void PUSH_4(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(4));

        }
        private static void PUSH_5(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(5));

        }
        private static void PUSH_6(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(6));

        }
        private static void PUSH_7(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(7));

        }
        private static void PUSH_8(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(8));

        }
        private static void PUSH_9(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(9));

        }
        private static void PUSH_NULL(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.Null);

        }
        private static void PUSH_FALSE(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromBoolean(false));

        }
        private static void PUSH_TRUE(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromBoolean(true));

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
            ctx.OperandStack.PushDatum(argDatum);

        }





        private static void PUSH_I8(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadSByte()));

        }
        private static void PUSH_I16(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadInt16()));

        }
        private static void PUSH_I32(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadInt32()));

        }
        private static void PUSH_I64(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadInt64()));

        }
        private static void PUSH_F32(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadFloat()));

        }
        private static void PUSH_F64(ExecuteFrameContext ctx)
        {
            ctx.OperandStack.PushDatum(ScriptDatum.FromNumber(ctx.ReadDouble()));

        }

        private static void PUSH_STRING(ExecuteFrameContext ctx)
        {
            var stringIndex = ctx.ReadInt32();
            ctx.OperandStack.PushDatum(ScriptDatum.FromString(ctx.Strings[stringIndex]));

        }

        private static void INC_LOCAL(ExecuteFrameContext ctx)
        {
            var stack = ctx.OperandStack;
            var index =  ctx.ReadByte();
            ref var slot = ref ctx.CurrentFrame.GetLocalRef(index);
            var original = slot;
            if (!slot.TryGetNumber(out var current))
            {
                current = double.NaN;
            }
            var newValue = current + 1;
            stack.PushDatum(ScriptDatum.FromNumber(newValue));
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
            if (!slot.TryGetNumber(out var current))
            {
                current = double.NaN;
            }
            var newValue = current + delta;
            slot = ScriptDatum.FromNumber(newValue);
            var result = isPostfix ? original : slot;
            stack.PushDatum(result);
        }

        private static void HandleLocalBinaryFromStack(ExecuteFrameContext ctx, bool useLongIndex, LocalStackOp operation)
        {
            var stack = ctx.OperandStack;
            var index = useLongIndex ? ctx.ReadInt32() : ctx.ReadByte();
            ref var leftSlot = ref ctx.CurrentFrame.GetLocalRef(index);
            var rightSlot = stack.PopDatum();
            switch (operation)
            {
                case LocalStackOp.Add:
                    if (leftSlot.Kind == ValueKind.Number && rightSlot.Kind == ValueKind.Number)
                    {
                        leftSlot.Number = leftSlot.Number + rightSlot.Number;
                    }
                    else
                    {
                        leftSlot = ScriptDatum.FromString(leftSlot.ToString() + rightSlot.ToString());
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
                leftSlot = ScriptDatum.FromNumber(ApplyBinaryOp(operation, leftNumber, rightNumber));
            }
            else
            {
                leftSlot = ScriptDatum.FromNumber(double.NaN);
            }
        }

        #endregion








    }
}
