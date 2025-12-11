using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;


namespace AuroraScript.Runtime
{
    internal unsafe sealed class ExecuteFrameContext : IDisposable
    {
        /// <summary>
        /// 虚拟机代码段基址指针
        /// </summary>
        internal readonly Byte* CodeBasePointer;
        internal ScriptDatum[] Locals;
        internal CallFrame CurrentFrame;
        internal readonly ScriptDatumStack OperandStack;
        internal readonly Stack<CallFrame> CallStack;
        internal readonly ImmutableArray<StringValue> Strings;



        internal readonly ExecuteOptions ExecuteOptions;
        internal readonly ClrTypeRegistry ClrRegistry;
        internal readonly ExecuteContext ExecuteContext;
        internal readonly ScriptObject UserState;
        internal readonly ScriptDomain Domain;
        internal readonly ScriptGlobal Global;
        internal ScriptModule Module;
        private readonly ByteCodeBuffer _codeBuffer;


        public ExecuteFrameContext(RuntimeVM vm, ExecuteContext executeContext)
        {
            _codeBuffer = vm._codeBuffer;
            ExecuteContext = executeContext;
            Strings = vm._stringConstants;
            OperandStack = executeContext._operandStack;
            CallStack = executeContext._callStack;
            UserState = executeContext.UserState;
            CodeBasePointer = (byte*)_codeBuffer.UnmanagedPtr;
            ClrRegistry = vm._clrRegistry;
            ExecuteOptions = executeContext.ExecuteOptions;
            Domain = executeContext.Domain;
            Global = Domain.Global;
            CurrentFrame = CallStack.Peek();
            Module = CurrentFrame.Module;
            Locals = CurrentFrame.Locals;
        }


        internal void SetStatus(ExecuteStatus status, ScriptObject result, Exception exception)
        {
            ExecuteContext.SetStatus(status, result, exception);
        }


        public void EnsureLocalStorage(int length)
        {
            CurrentFrame.EnsureLocalStorage(length);
            Locals = CurrentFrame.Locals;
        }



        public int PopCallStack()
        {
            var finishedFrame = CallStack.Pop();
            CallFramePool.Return(finishedFrame);
            var length = CallStack.Count;
            if (length > 0)
            {
                CurrentFrame = CallStack.Peek();
                Module = CurrentFrame.Module;
                Locals = CurrentFrame.Locals;
            }
            return length;
        }

        public void PushCallStack(CallFrame frame)
        {
            CallStack.Push(frame);
            CurrentFrame = frame;
            Module = CurrentFrame.Module;
            Locals = CurrentFrame.Locals;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushLocal(int localIndex)
        {
            ref ScriptDatum local = ref Locals[localIndex];
            OperandStack.PushRef(ref local);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopToLocal(int localIndex)
        {
            ref ScriptDatum local = ref Locals[localIndex];
            OperandStack.PopToRef(ref local);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IncLocal(int localIndex)
        {
            var origin = Locals[localIndex];
            Locals[localIndex] = ScriptDatum.FromNumber(origin.Number + 1);
            OperandStack.PushRef(ref origin);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte ReadOpCode()
        {
            ref var frame = ref CurrentFrame;
            frame.LastInstructionPointer = frame.Pointer;
            return (*(Byte*)(CodeBasePointer + frame.Pointer++));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte ReadByte()
        {
            ref var frame = ref CurrentFrame;
            return (*(Byte*)(CodeBasePointer + frame.Pointer++));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int32 ReadInt32()
        {
            ref var frame = ref CurrentFrame;
            var value = (*(Int32*)(CodeBasePointer + frame.Pointer));
            frame.Pointer += 4;
            return value;
        }

        public SByte ReadSByte()
        {
            ref var frame = ref CurrentFrame;
            return (*(SByte*)(CodeBasePointer + frame.Pointer++));
        }
        public Int16 ReadInt16()
        {
            ref var frame = ref CurrentFrame;
            var value = (*(Int16*)(CodeBasePointer + frame.Pointer));
            frame.Pointer += 2;
            return value;
        }

        public Int64 ReadInt64()
        {
            ref var frame = ref CurrentFrame;
            var value = (*(Int64*)(CodeBasePointer + frame.Pointer));
            frame.Pointer += 8;
            return value;
        }

        public Single ReadFloat()
        {
            ref var frame = ref CurrentFrame;
            var value = (*(Single*)(CodeBasePointer + frame.Pointer));
            frame.Pointer += 4;
            return value;
        }

        public Double ReadDouble()
        {
            ref var frame = ref CurrentFrame;
            var value = (*(Double*)(CodeBasePointer + frame.Pointer));
            frame.Pointer += 8;
            return value;
        }

        public void Dispose()
        {

        }

        public void EnsureCallDepth()
        {
            if (CallStack.Count > ExecuteContext.ExecuteOptions.MaxCallStackDepth)
            {
                throw new AuroraVMException("The number of method call stacks exceeds the limit of " + ExecuteContext.ExecuteOptions.MaxCallStackDepth);
            }
        }





    }
}
