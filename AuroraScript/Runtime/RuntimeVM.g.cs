using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
{
    internal unsafe partial class RuntimeVM
    {
        private static void NOP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {

        }

        private static void POP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PopDiscard();
        }

        private static void DUP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.Duplicate();
        }
        private static void SWAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.Swap();
        }

        private static void LOAD_ARG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var argIndex = vm._codeBuffer.ReadByte(frame);
            var argDatum = frame.GetArgumentDatum(argIndex);
            exeContext._operandStack.PushDatum(argDatum);
        }

        private static void TRY_LOAD_ARG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var propNameIndex = vm._codeBuffer.ReadByte(frame);
            if (frame.TryGetArgumentDatum(propNameIndex, out var tryArgDatum))
            {
                exeContext._operandStack.PopDiscard();
                exeContext._operandStack.PushDatum(tryArgDatum);
            }
        }




        private static void CREATE_CLOSURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _codeBuffer = vm._codeBuffer;
            var _operandStack = exeContext._operandStack;
            var closureOffset = _codeBuffer.ReadInt32(frame);
            var captureCount = _codeBuffer.ReadByte(frame);
            var entryPointer = frame.Pointer + closureOffset;
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

            var closure = new ClosureFunction(frame.Domain, moduleForClosure, entryPointer, capturedUpvalues);
            _operandStack.PushDatum(ScriptDatum.FromObject(closure));
        }

        private static void INIT_MODULE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var module = new ScriptModule(propName.Value);
            frame.Domain.Global.Define("@" + propName.Value, module, writeable: false, enumerable: true);
        }

        private static void NEW_MAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            _operandStack.PushDatum(ScriptDatum.FromObject(new ScriptObject()));
        }

        private static void NEW_ARRAY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var count = vm._codeBuffer.ReadInt32(frame);
            var _operandStack = exeContext._operandStack;
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




        private static void NEW_REGEX(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var _codeBuffer = vm._codeBuffer;
            var _pattern = _codeBuffer.ReadInt32(frame);
            var _flags = _codeBuffer.ReadInt32(frame);
            var flags = vm._stringConstants[_flags];
            var pattern = vm._stringConstants[_pattern];
            var regex = RegexManager.Resolve(pattern.Value, flags.Value);
            _operandStack.PushDatum(ScriptDatum.FromRegex(regex));
        }




        private static void DECONSTRUCT_ARRAY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var datumValue = _operandStack.PopDatum();
            if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array)
            {
                var deConstruct = new ScriptDeConstruct(array, ValueKind.Array);
                _operandStack.PushDatum(ScriptDatum.FromObject(deConstruct));
            }
        }


        private static void DECONSTRUCT_MAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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






        private static void GET_ITERATOR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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
        private static void ITERATOR_VALUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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
        private static void ITERATOR_HAS_VALUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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
        private static void ITERATOR_NEXT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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







        private static void TYPEOF(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            rightSlot = rightSlot.TypeOf();
            //datumRight = PopDatum();
            //PushDatum(datumRight.TypeOf());
        }


        private static void ADD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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






        private static void ALLOC_LOCALS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var localsRequested = vm._codeBuffer.ReadInt32(frame);
            frame.EnsureLocalStorage(localsRequested);
        }

        private static void LOAD_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var localIndex = vm._codeBuffer.ReadByte(frame);
            ref var value = ref frame.GetLocalRef(localIndex);
            exeContext._operandStack.PushRef(ref value);

        }

        private static void STORE_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var localIndex = vm._codeBuffer.ReadByte(frame);
            ref var slot = ref frame.GetLocalRef(localIndex);
            slot = exeContext._operandStack.PopDatum();
        }


        private static void LOAD_LOCAL_LONG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            exeContext._operandStack.PushDatum(frame.GetLocalDatum(localIndex));
        }

        private static void STORE_LOCAL_LONG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var value = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            frame.SetLocalDatum(localIndex, value);
        }



        private static void DELETE_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var key = _operandStack.PopDatum();
            var obj = _operandStack.PeekDatum();
            if (obj.TryGetAnyObject(out var scriptObject))
            {
                var propName = ExtractPropertyKey(ref key);
                scriptObject.DeletePropertyValue(propName);
            }
        }

        private static void GET_THIS_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = frame.Module.GetPropertyValue(propName);
            _operandStack.PushObject(value);
        }
        private static void SET_THIS_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = _operandStack.PopObject();
            frame.Module.SetPropertyValue(propName, value);
        }


        private static void GET_GLOBAL_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            if (vm._clrRegistry.TryGetClrType(propName.Value, out var clrType))
            {
                _operandStack.PushDatum(ScriptDatum.FromClrType(clrType));
            }
            else
            {
                var value = frame.Domain.Global.GetPropertyValue(propName);
                _operandStack.PushObject(value);
            }
        }
        private static void SET_GLOBAL_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = _operandStack.PopObject();
            frame.Domain.Global.SetPropertyValue(propName, value);
        }





        private static void GET_ELEMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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

        private static void SET_ELEMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
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




        private static void GET_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            ref var topDatum = ref exeContext._operandStack.PeekRef();
            if (topDatum.TryGetAnyObject(out var obj))
            {
                topDatum = ScriptDatum.FromObject(obj.GetPropertyValue(propName));
            }
            else if (topDatum.Kind == ValueKind.Number || topDatum.Kind == ValueKind.Boolean)
            {
                topDatum = ScriptDatum.FromObject(topDatum.ToObject().GetPropertyValue(propName));
            }
        }

        private static void SET_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = exeContext._operandStack.PopObject();
            if (exeContext._operandStack.PopDatum().TryGetAnyObject(out var obj))
            {
                obj.SetPropertyValue(propName, value);
            }
        }


        private static void CALL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {

            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;

            if (_callStack.Count > exeContext.ExecuteOptions.MaxCallStackDepth)
            {
                throw new AuroraVMException("The number of method call stacks exceeds the limit of " + exeContext.ExecuteOptions.MaxCallStackDepth);
            }

            // 函数调用指令
            // 从栈顶弹出可调用对象
            var callable = _operandStack.PopDatum();
            // 读取参数数量
            var argCount = vm._codeBuffer.ReadByte(frame);
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
                callFrame.Initialize(frame.Domain, closureFunc.Module, closureFunc.EntryPointer, closureFunc.CapturedUpvalues);
                // 将新帧压入调用栈
                _callStack.Push(callFrame);
                // 更新当前帧引用
                frame = callFrame;
                return;
            }
            else if (callable.TryGetClrBonding(out var callableFunc))
            {
                try
                {
                    ScriptDatum result = ScriptDatum.Null;
                    callableFunc.Invoke(exeContext, null, argDatums, ref result);
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
                    clrInvokable.Invoke(exeContext, callable.ToObject(), argDatums, ref result);
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

        private static void RETURN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;
            // 函数返回指令
            // 获取返回值（如果有）
            var datumValue = _operandStack.Count > 0 ? _operandStack.PopDatum() : ScriptDatum.Null;
            // 弹出当前调用帧
            var finishedFrame = _callStack.Pop();
            CallFramePool.Return(finishedFrame);
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (_callStack.Count == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                var value = datumValue.ToObject();
                exeContext.SetStatus(ExecuteStatus.Complete, value, null);
                return;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            _operandStack.PushRef(ref datumValue);
            // 切换到调用者的帧继续执行
            frame = _callStack.Peek();
        }



        private static void RETURN_NULL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var _operandStack = exeContext._operandStack;
            var _callStack = exeContext._callStack;

            // 弹出当前调用帧
            var finishedFrame = _callStack.Pop();
            CallFramePool.Return(finishedFrame);
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (_callStack.Count == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                exeContext.SetStatus(ExecuteStatus.Complete, ScriptObject.Null, null);
                return;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            _operandStack.PushDatum(ScriptDatum.Null);
            // 切换到调用者的帧继续执行
            frame = _callStack.Peek();

        }


        private static void CAPTURE_VAR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var slotIndex = vm._codeBuffer.ReadInt32(frame);
            var capturedUpvalue = frame.GetCapturedUpvalue(slotIndex) ?? frame.GetOrCreateUpvalue(slotIndex);
            capturedUpvalue.MarkAliasSlot(slotIndex);
            exeContext._operandStack.PushDatum(ScriptDatum.FromObject(capturedUpvalue));
        }
        private static void LOAD_CAPTURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var datumValue = frame.GetLocalDatum(localIndex);
            if (datumValue.Kind != ValueKind.Object || datumValue.Object is not Upvalue upvalueToRead)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            exeContext._operandStack.PushDatum(upvalueToRead.Get());
        }

        private static void STORE_CAPTURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var datumValue = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var upvalueDatum = frame.GetLocalDatum(localIndex);
            if (upvalueDatum.Kind != ValueKind.Object || upvalueDatum.Object is not Upvalue upvalueToWrite)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            upvalueToWrite.Set(datumValue);
        }

        private static void SUBTRACT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Subtract, double.NaN);
        }

        private static void MULTIPLY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Multiply, double.NaN);
        }

        private static void DIVIDE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Divide, double.NaN);
        }

        private static void MOD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Mod, double.NaN);

        }

        private static void NEGATE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Negate, double.NaN);

        }

        private static void INCREMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Increment, double.NaN);

        }

        private static void DECREMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Decrement, double.NaN);

        }

        private static void BIT_NOT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.BitNot, -1);
        }

        private static void BIT_SHIFT_LEFT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void BIT_SHIFT_RIGHT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void BIT_UNSIGNED_SHIFT_RIGHT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void BIT_AND(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void BIT_OR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void BIT_XOR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stack = exeContext._operandStack;
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

        private static void LOGIC_NOT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteLogicNot(exeContext);

        }

        private static void LOGIC_AND(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteLogicalBinary(exeContext, isAnd: true);

        }

        private static void LOGIC_OR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteLogicalBinary(exeContext, isAnd: false);

        }

        private static void EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteEquality(exeContext, negate: false);

        }

        private static void NOT_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteEquality(exeContext, negate: true);

        }

        private static void LESS_THAN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessThan);

        }

        private static void LESS_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessEqual);

        }

        private static void GREATER_THAN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterThan);

        }

        private static void GREATER_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterEqual);

        }

        private static void JUMP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            frame.Pointer += offset;

        }

        private static void JUMP_IF_FALSE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (!isTrue)
            {
                frame.Pointer += offset;
            }

        }

        private static void JUMP_IF_TRUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (isTrue)
            {
                frame.Pointer += offset;
            }

        }




        private static void YIELD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            // TODO
            if (exeContext.ExecuteOptions.EnabledYield)
            {
                exeContext.SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
            }

        }






        #region const

        private static void PUSH_0(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(0));

        }
        private static void PUSH_1(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(1));

        }
        private static void PUSH_2(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(2));

        }
        private static void PUSH_3(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(3));

        }
        private static void PUSH_4(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(4));

        }
        private static void PUSH_5(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(5));

        }
        private static void PUSH_6(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(6));

        }
        private static void PUSH_7(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(7));

        }
        private static void PUSH_8(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(8));

        }
        private static void PUSH_9(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(9));

        }
        private static void PUSH_NULL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.Null);

        }
        private static void PUSH_FALSE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromBoolean(false));

        }
        private static void PUSH_TRUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromBoolean(true));

        }
        private static void PUSH_THIS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromObject(frame.Module));
        }
        private static void PUSH_GLOBAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromObject(exeContext.Domain.Global));
        }
        private static void PUSH_CONTEXT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromObject(exeContext.UserState));

        }
        private static void PUSH_ARGUMENTS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var argDatum = ScriptDatum.FromArray(new ScriptArray(frame.Arguments.ViewSpan()));
            exeContext._operandStack.PushDatum(argDatum);

        }





        private static void PUSH_I8(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadSByte(frame)));

        }
        private static void PUSH_I16(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt16(frame)));

        }
        private static void PUSH_I32(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt32(frame)));

        }
        private static void PUSH_I64(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt64(frame)));

        }
        private static void PUSH_F32(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadFloat(frame)));

        }
        private static void PUSH_F64(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadDouble(frame)));

        }

        private static void PUSH_STRING(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            var stringIndex = vm._codeBuffer.ReadInt32(frame);
            exeContext._operandStack.PushDatum(ScriptDatum.FromString(vm._stringConstants[stringIndex]));

        }

        private static void INC_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: false, delta: 1d, isPostfix: false);
        }

        private static void INC_LOCAL_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: true, delta: 1d, isPostfix: false);
        }

        private static void DEC_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: false, delta: -1d, isPostfix: false);
        }

        private static void DEC_LOCAL_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: true, delta: -1d, isPostfix: false);
        }

        private static void INC_LOCAL_POST(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: false, delta: 1d, isPostfix: true);
        }

        private static void INC_LOCAL_POST_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: true, delta: 1d, isPostfix: true);
        }

        private static void DEC_LOCAL_POST(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: false, delta: -1d, isPostfix: true);
        }

        private static void DEC_LOCAL_POST_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalIncrement(vm, exeContext, ref frame, useLongIndex: true, delta: -1d, isPostfix: true);
        }

        private static void ADD_LOCAL_STACK(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: false, operation: LocalStackOp.Add);
        }

        private static void ADD_LOCAL_STACK_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: true, operation: LocalStackOp.Add);
        }

        private static void SUB_LOCAL_STACK(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: false, operation: LocalStackOp.Subtract);
        }

        private static void SUB_LOCAL_STACK_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: true, operation: LocalStackOp.Subtract);
        }

        private static void MUL_LOCAL_STACK(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: false, operation: LocalStackOp.Multiply);
        }

        private static void MUL_LOCAL_STACK_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: true, operation: LocalStackOp.Multiply);
        }

        private static void DIV_LOCAL_STACK(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: false, operation: LocalStackOp.Divide);
        }

        private static void DIV_LOCAL_STACK_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: true, operation: LocalStackOp.Divide);
        }

        private static void MOD_LOCAL_STACK(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: false, operation: LocalStackOp.Mod);
        }

        private static void MOD_LOCAL_STACK_L(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame)
        {
            HandleLocalBinaryFromStack(vm, exeContext, ref frame, useLongIndex: true, operation: LocalStackOp.Mod);
        }

        private enum LocalStackOp : byte
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Mod
        }

        private static void HandleLocalIncrement(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, bool useLongIndex, double delta, bool isPostfix)
        {
            var stack = exeContext._operandStack;
            var index = useLongIndex ? vm._codeBuffer.ReadInt32(frame) : vm._codeBuffer.ReadByte(frame);
            ref var slot = ref frame.GetLocalRef(index);
            var original = slot;
            if (!TryGetNumber(in slot, out var current))
            {
                current = double.NaN;
            }
            var newValue = current + delta;
            slot = ScriptDatum.FromNumber(newValue);
            var result = isPostfix ? original : slot;
            stack.PushDatum(result);
        }

        private static void HandleLocalBinaryFromStack(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, bool useLongIndex, LocalStackOp operation)
        {
            var stack = exeContext._operandStack;
            var index = useLongIndex ? vm._codeBuffer.ReadInt32(frame) : vm._codeBuffer.ReadByte(frame);
            ref var leftSlot = ref frame.GetLocalRef(index);
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
