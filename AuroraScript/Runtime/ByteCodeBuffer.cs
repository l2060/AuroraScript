using AuroraScript.Core;
using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    internal class ByteCodeBuffer
    {
        private readonly Byte[] _byteCode;



        public ByteCodeBuffer(Byte[] bytes)
        {
            _byteCode = bytes;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Core.OpCode ReadOpCode(CallFrame frame)
        {
            if (frame.Pointer >= _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            frame.LastInstructionPointer = frame.Pointer;
            var opCode = (Core.OpCode)_byteCode[frame.Pointer++];
            return opCode;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SByte ReadSByte(CallFrame frame)
        {
            if (frame.Pointer >= _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            return (SByte)_byteCode[frame.Pointer++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte ReadByte(CallFrame frame)
        {
            if (frame.Pointer >= _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            return _byteCode[frame.Pointer++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int16 ReadInt16(CallFrame frame)
        {
            if (frame.Pointer + 2 > _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            short value = BitConverter.ToInt16(_byteCode, frame.Pointer);
            frame.Pointer += 2;
            return value;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Single ReadFloat(CallFrame frame)
        {
            if (frame.Pointer + 4 > _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            float value = BitConverter.ToSingle(_byteCode, frame.Pointer);
            frame.Pointer += 4;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Double ReadDouble(CallFrame frame)
        {
            if (frame.Pointer + 8 > _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            double value = BitConverter.ToDouble(_byteCode, frame.Pointer);
            frame.Pointer += 8;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int32 ReadInt32(CallFrame frame)
        {
            if (frame.Pointer + 4 > _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            int value = BitConverter.ToInt32(_byteCode, frame.Pointer);
            frame.Pointer += 4;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int64 ReadInt64(CallFrame frame)
        {
            if (frame.Pointer + 8 > _byteCode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            Int64 value = BitConverter.ToInt64(_byteCode, frame.Pointer);
            frame.Pointer += 8;
            return value;
        }


    }
}
