using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
{
    internal unsafe partial class RuntimeVM
    {
        private static bool NOP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            return true;
        }

        private static bool POP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PopDiscard();
            return true;
        }

        private static bool DUP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.Duplicate();
            return true;
        }
        private static bool SWAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.Swap();
            return true;
        }

        private static bool LOAD_ARG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var argIndex = vm._codeBuffer.ReadByte(frame);
            var argDatum = frame.GetArgumentDatum(argIndex);
            exeContext._operandStack.PushDatum(argDatum);
            return true;
        }

        private static bool TRY_LOAD_ARG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var propNameIndex = vm._codeBuffer.ReadByte(frame);
            if (frame.TryGetArgumentDatum(propNameIndex, out var tryArgDatum))
            {
                exeContext._operandStack.PopDiscard();
                exeContext._operandStack.PushDatum(tryArgDatum);
            }
            return true;
        }




        private static bool CREATE_CLOSURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _codeBuffer = vm._codeBuffer;
            var _operandStack = exeContext._operandStack;
            var closureOffset = _codeBuffer.ReadInt32(frame);
            var captureCount = _codeBuffer.ReadByte(frame);
            var entryPointer = frame.Pointer + closureOffset;

            var moduleObject = _operandStack.PopObject();
            var moduleForClosure = moduleObject as ScriptModule;

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
                    var upvalueObj = _operandStack.PopObject();
                    if (upvalueObj is not Upvalue upvalue)
                    {
                        throw new AuroraVMException("Invalid captured upvalue");
                    }
                    var aliasSlot = upvalue.ConsumeAliasSlot();
                    capturedUpvalues[i] = new ClosureUpvalue(aliasSlot, upvalue);
                }
            }

            var closure = new ClosureFunction(frame.Domain, moduleForClosure, entryPointer, capturedUpvalues);
            _operandStack.PushObject(closure);
            return true;
        }

        private static bool INIT_MODULE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var module = new ScriptModule(propName.Value);
            frame.Domain.Global.Define("@" + propName.Value, module, writeable: false, enumerable: true);
            return true;
        }

        private static bool NEW_MAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            _operandStack.PushObject(new ScriptObject());
            return true;
        }

        private static bool NEW_ARRAY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
                if (datumBuffer[i].Kind.Include(ValueKind.Object) && datumBuffer[i].Object is ScriptDeConstruct deConstruct)
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
            _operandStack.PushObject(newArray);
            return true;
        }




        private static bool NEW_REGEX(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var _codeBuffer = vm._codeBuffer;
            var _pattern = _codeBuffer.ReadInt32(frame);
            var _flags = _codeBuffer.ReadInt32(frame);
            var flags = vm._stringConstants[_flags];
            var pattern = vm._stringConstants[_pattern];
            var regex = RegexManager.Resolve(pattern.Value, flags.Value);
            _operandStack.PushDatum(ScriptDatum.FromRegex(regex));
            return true;
        }




        private static bool DECONSTRUCT_ARRAY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datumValue = _operandStack.PopDatum();
            if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array)
            {
                var deConstruct = new ScriptDeConstruct(array, ValueKind.Array);
                _operandStack.PushObject(deConstruct);
            }
            return true;
        }


        private static bool DECONSTRUCT_MAP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datumValue = _operandStack.PopDatum();
            var value = _operandStack.PopObject();
            if (datumValue.Kind.Include(ValueKind.Object))
            {
                value.CopyPropertysFrom(datumValue.Object, true);
            }
            else if (datumValue.Kind == ValueKind.Array && datumValue.Object is ScriptArray array1)
            {
                for (int i = 0; i < array1.Length; i++)
                {
                    var ele = array1.Get(i);
                    value.SetPropertyValue(i.ToString(), ele.ToObject());
                }
            }
            return true;
        }






        private static bool GET_ITERATOR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datum = _operandStack.PopDatum();
            if (datum.TryGetAnyObject(out var obj) && obj is IEnumerator iterable)
            {
                _operandStack.PushObject(iterable.GetIterator());
            }
            else
            {
                throw new AuroraVMException($"Object {obj} does not support iterators.");
            }
            return true;
        }
        private static bool ITERATOR_VALUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datum = _operandStack.PopDatum();
            if (datum.TryGetObject(out var obj) && obj is ItemIterator iterator)
            {
                _operandStack.PushDatum(iterator.Value());
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
            return true;
        }
        private static bool ITERATOR_HAS_VALUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datum = _operandStack.PopDatum();
            if (datum.TryGetObject(out var obj) && obj is ItemIterator iterator)
            {
                _operandStack.PushDatum(ScriptDatum.FromBoolean(iterator.HasValue()));
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
            return true;
        }
        private static bool ITERATOR_NEXT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var datum = _operandStack.PopDatum();
            if (datum.TryGetObject(out var obj) && obj is ItemIterator iterator)
            {
                iterator.Next();
            }
            else
            {
                throw new AuroraVMException($"Object {obj} not iterator.");
            }
            return true;
        }







        private static bool TYPEOF(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stack = exeContext._operandStack;
            ref var rightSlot = ref stack.PeekRef();
            rightSlot = rightSlot.TypeOf();
            //datumRight = PopDatum();
            //PushDatum(datumRight.TypeOf());
            return true;
        }


        private static bool ADD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }






        private static bool ALLOC_LOCALS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localsRequested = vm._codeBuffer.ReadInt32(frame);
            frame.EnsureLocalStorage(localsRequested);
            return true;
        }

        private static bool LOAD_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadByte(frame);
            //ref var value = ref frame.GetLocalRef(localIndex);
            //exeContext._operandStack.PushRef( ref value);
            exeContext._operandStack.PushDatum(frame.GetLocalDatum(localIndex));
            return true;
        }

        private static bool STORE_LOCAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadByte(frame);
            ref var slot = ref frame.GetLocalRef(localIndex);
            slot = exeContext._operandStack.PopDatum();
            //frame.SetLocalDatum(localIndex, value);
            return true;
        }


        private static bool LOAD_LOCAL_LONG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            exeContext._operandStack.PushDatum(frame.GetLocalDatum(localIndex));
            return true;
        }

        private static bool STORE_LOCAL_LONG(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var value = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            frame.SetLocalDatum(localIndex, value);
            return true;
        }

        private static bool SET_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = exeContext._operandStack.PopObject();
            if (exeContext._operandStack.PopDatum().TryGetAnyObject(out var obj))
            {
                obj.SetPropertyValue(propName, value);
            }
            return true;
        }


        private static bool GET_ELEMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;
            var datumValue = _operandStack.PopDatum();
            var datumObjValue = _operandStack.PopDatum();
            if (datumObjValue.TryGetArray(out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                _operandStack.PushDatum(scriptArray.Get((Int32)datumValue.Number));
            }
            else if (datumObjValue.Kind.Include(ValueKind.Object))
            {
                var key = ExtractPropertyKey(ref datumValue);
                _operandStack.PushObject(datumObjValue.Object.GetPropertyValue(key));
            }
            else
            {
                _operandStack.PushDatum(ScriptDatum.FromNull());
            }
            return true;
        }

        private static bool DELETE_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var key = _operandStack.PopDatum();
            var obj = _operandStack.PeekDatum();
            if (obj.TryGetAnyObject(out var scriptObject))
            {
                scriptObject.DeletePropertyValue(ExtractPropertyKey(ref key));
            }
            return true;
        }

        private static bool GET_THIS_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = frame.Module.GetPropertyValue(propName.Value);
            _operandStack.PushObject(value);
            return true;
        }
        private static bool SET_THIS_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = _operandStack.PopObject();
            frame.Module.SetPropertyValue(propName, value);
            return true;
        }


        private static bool GET_GLOBAL_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            if (vm._clrRegistry.TryGetClrType(propName.Value, out var clrType))
            {
                _operandStack.PushObject(clrType);
            }
            else
            {
                var value = exeContext.Domain.Global.GetPropertyValue(propName.Value);
                _operandStack.PushObject(value);
            }

            return true;
        }
        private static bool SET_GLOBAL_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            var value = _operandStack.PopObject();
            exeContext.Domain.Global.SetPropertyValue(propName.Value, value);
            return true;
        }





        private static bool SET_ELEMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {

            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;
            var datumTargetObj = _operandStack.PopDatum();
            var datumValue = _operandStack.PopDatum();
            var datumAssignedValue = _operandStack.PopDatum();
            if (datumTargetObj.TryGetArray(out var scriptArray) && datumValue.Kind == ValueKind.Number)
            {
                scriptArray.Set((Int32)datumValue.Number, datumAssignedValue);
            }
            else if (datumTargetObj.Kind.Include(ValueKind.Object))
            {
                var key = ExtractPropertyKey(ref datumValue);
                datumTargetObj.Object.SetPropertyValue(key, datumAssignedValue.ToObject());
            }
            return true;
        }




        private static bool GET_PROPERTY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var propNameIndex = vm._codeBuffer.ReadInt32(frame);
            var propName = vm._stringConstants[propNameIndex];
            ref var topDatum = ref exeContext._operandStack.PeekRef();
            if (topDatum.TryGetAnyObject(out var obj))
            {
                topDatum = ScriptDatum.FromObject(obj.GetPropertyValue(propName.Value));
            }
            else if (topDatum.Kind == ValueKind.Number || topDatum.Kind == ValueKind.Boolean)
            {
                topDatum = ScriptDatum.FromObject(topDatum.ToObject().GetPropertyValue(propName.Value));
            }
            //var datum = PopDatum();
            //if (datum.TryGetAnyObject(out obj))
            //{
            //    PushObject(obj.GetPropertyValue(propName.Value));
            //}
            //else if (datum.Kind == ValueKind.Number || datum.Kind == ValueKind.Boolean)
            //{
            //    PushObject(datum.ToObject().GetPropertyValue(propName.Value));
            //}
            return true;
        }


        private static bool CALL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            var argDatums = new ScriptDatum[argCount];
            // 从栈中弹出参数，注意参数顺序是从右到左
            for (int i = argCount - 1; i >= 0; i--)
            {
                argDatums[i] = _operandStack.PopDatum();
            }

            if (callable.TryGetFunction(out var closureFunc))
            {
                // 如果是脚本中定义的闭包函数
                // 创建新的调用帧，包含环境、全局对象、模块和入口点
                var callFrame = CallFramePool.Rent(frame.Domain, closureFunc.Module, closureFunc.EntryPointer, argDatums, closureFunc.CapturedUpvalues);
                // 将新帧压入调用栈
                _callStack.Push(callFrame);
                // 更新当前帧引用
                frame = callFrame;
                return true;
            }
            else if (callable.TryGetClrBonding(out var callableFunc))
            {
                try
                {
                    var callResult = callableFunc.Invoke(exeContext, null, argDatums);
                    _operandStack.PushObject(callResult);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    //ArrayPool<ScriptDatum>.Shared.Return(argDatums);
                }
            }
            else if (callable.TryGetClrInvokable(out var clrInvokable))
            {
                try
                {
                    var callResult = clrInvokable.Invoke(exeContext, callable.ToObject(), argDatums);
                    _operandStack.PushDatum(callResult);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    //ArrayPool<ScriptDatum>.Shared.Return(argDatums);
                }
            }
            else
            {
                // 如果不是可调用对象，抛出异常
                throw new InvalidOperationException($"Cannot call {callable.Kind}");
            }


            return true;
        }

        private static bool RETURN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _callStack = exeContext._callStack;
            var _operandStack = exeContext._operandStack;
            // 函数返回指令
            // 获取返回值（如果有）
            var datumValue = _operandStack.Count > 0 ? _operandStack.PopDatum() : ScriptDatum.FromNull();
            var value = datumValue.ToObject();
            // 弹出当前调用帧
            var finishedFrame = _callStack.Pop();
            CallFramePool.Return(finishedFrame);


            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (_callStack.Count == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                exeContext.SetStatus(ExecuteStatus.Complete, value, null);
                return true;
            }

            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            _operandStack.PushDatum(datumValue);
            // 切换到调用者的帧继续执行
            frame = _callStack.Peek();
            return true;
        }





        private static bool CAPTURE_VAR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var slotIndex = vm._codeBuffer.ReadInt32(frame);
            var capturedUpvalue = frame.GetCapturedUpvalue(slotIndex) ?? frame.GetOrCreateUpvalue(slotIndex);
            capturedUpvalue.MarkAliasSlot(slotIndex);
            exeContext._operandStack.PushObject(capturedUpvalue);
            return true;
        }
        private static bool LOAD_CAPTURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var datumValue = frame.GetLocalDatum(localIndex);
            if (datumValue.Kind != ValueKind.Object || datumValue.Object is not Upvalue upvalueToRead)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            exeContext._operandStack.PushDatum(upvalueToRead.Get());
            return true;
        }

        private static bool STORE_CAPTURE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var datumValue = exeContext._operandStack.PopDatum();
            var localIndex = vm._codeBuffer.ReadInt32(frame);
            var upvalueDatum = frame.GetLocalDatum(localIndex);
            if (upvalueDatum.Kind != ValueKind.Object || upvalueDatum.Object is not Upvalue upvalueToWrite)
            {
                throw new AuroraVMException("Invalid captured upvalue");
            }
            upvalueToWrite.Set(datumValue);
            return true;
        }

        private static bool SUBTRACT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Subtract, double.NaN);
            return true;
        }

        private static bool MULTIPLY(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Multiply, double.NaN);
            return true;
        }

        private static bool DIVIDE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Divide, double.NaN);
            return true;
        }

        private static bool MOD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryNumberOp(exeContext, BinaryNumberOp.Mod, double.NaN);
            return true;
        }

        private static bool NEGATE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Negate, double.NaN);
            return true;
        }

        private static bool INCREMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Increment, double.NaN);
            return true;
        }

        private static bool DECREMENT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.Decrement, double.NaN);
            return true;
        }

        private static bool BIT_NOT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteUnaryNumberOp(exeContext, UnaryNumberOp.BitNot, -1);
            return true;
        }

        private static bool BIT_SHIFT_LEFT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool BIT_SHIFT_RIGHT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool BIT_UNSIGNED_SHIFT_RIGHT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool BIT_AND(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool BIT_OR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool BIT_XOR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
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
            return true;
        }

        private static bool LOGIC_NOT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicNot(exeContext);
            return true;
        }

        private static bool LOGIC_AND(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicalBinary(exeContext, isAnd: true);
            return true;
        }

        private static bool LOGIC_OR(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteLogicalBinary(exeContext, isAnd: false);
            return true;
        }

        private static bool EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteEquality(exeContext, negate: false);
            return true;
        }

        private static bool NOT_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteEquality(exeContext, negate: true);
            return true;
        }

        private static bool LESS_THAN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessThan);
            return true;
        }

        private static bool LESS_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.LessEqual);
            return true;
        }

        private static bool GREATER_THAN(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterThan);
            return true;
        }

        private static bool GREATER_EQUAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            ExecuteBinaryPredicate(exeContext, BinaryPredicateOp.GreaterEqual);
            return true;
        }

        private static bool JUMP(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            frame.Pointer += offset;
            return true;
        }

        private static bool JUMP_IF_FALSE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (!isTrue)
            {
                frame.Pointer += offset;
            }
            return true;
        }

        private static bool JUMP_IF_TRUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var offset = vm._codeBuffer.ReadInt32(frame);
            var stack = exeContext._operandStack;
            var isTrue = stack.PeekRef().IsTrue();
            stack.PopDiscard();
            if (isTrue)
            {
                frame.Pointer += offset;
            }
            return true;
        }


        private static bool RETURN_NULL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var _operandStack = exeContext._operandStack;
            var _callStack = exeContext._callStack;
            // 弹出当前调用帧
            CallFramePool.Return(_callStack.Pop());
            // 如果调用栈为空，说明已经执行到最外层，整个脚本执行完毕
            if (_callStack.Count == 0)
            {
                // 设置执行状态为完成，并返回最终结果
                exeContext.SetStatus(ExecuteStatus.Complete, ScriptObject.Null, null);
                return true;
            }
            // 如果调用栈不为空，说明是从子函数返回到调用者
            // 将返回值压入操作数栈，供调用者使用
            _operandStack.PushDatum(ScriptDatum.FromNull());
            // 切换到调用者的帧继续执行
            frame = _callStack.Peek();
            return true;
        }


        private static bool YIELD(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            // TODO
            if (exeContext.ExecuteOptions.EnabledYield)
            {
                exeContext.SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
            }
            return true;
        }






        #region const

        private static bool PUSH_0(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(0));
            return true;
        }
        private static bool PUSH_1(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(1));
            return true;
        }
        private static bool PUSH_2(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(2));
            return true;
        }
        private static bool PUSH_3(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(3));
            return true;
        }
        private static bool PUSH_4(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(4));
            return true;
        }
        private static bool PUSH_5(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(5));
            return true;
        }
        private static bool PUSH_6(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(6));
            return true;
        }
        private static bool PUSH_7(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(7));
            return true;
        }
        private static bool PUSH_8(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(8));
            return true;
        }
        private static bool PUSH_9(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(9));
            return true;
        }
        private static bool PUSH_NULL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNull());
            return true;
        }
        private static bool PUSH_FALSE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromBoolean(false));
            return true;
        }
        private static bool PUSH_TRUE(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromBoolean(true));
            return true;
        }
        private static bool PUSH_THIS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushObject(frame.Module);
            return true;
        }
        private static bool PUSH_GLOBAL(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushObject(exeContext.Domain.Global);
            return true;
        }
        private static bool PUSH_CONTEXT(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromObject(exeContext.UserState));
            return true;
        }
        private static bool PUSH_ARGUMENTS(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var argDatum = ScriptDatum.FromArray(new ScriptArray(frame.Arguments));
            exeContext._operandStack.PushDatum(argDatum);
            return true;
        }





        private static bool PUSH_I8(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadSByte(frame)));
            return true;
        }
        private static bool PUSH_I16(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt16(frame)));
            return true;
        }
        private static bool PUSH_I32(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt32(frame)));
            return true;
        }
        private static bool PUSH_I64(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadInt64(frame)));
            return true;
        }
        private static bool PUSH_F32(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadFloat(frame)));
            return true;
        }
        private static bool PUSH_F64(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            exeContext._operandStack.PushDatum(ScriptDatum.FromNumber(vm._codeBuffer.ReadDouble(frame)));
            return true;
        }

        private static bool PUSH_STRING(RuntimeVM vm, ExecuteContext exeContext, ref CallFrame frame, OpCode opCode)
        {
            var stringIndex = vm._codeBuffer.ReadInt32(frame);
            exeContext._operandStack.PushDatum(ScriptDatum.FromString(vm._stringConstants[stringIndex]));
            return true;
        }




        #endregion








    }
}
