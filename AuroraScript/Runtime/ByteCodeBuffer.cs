using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraScript.Runtime
{
    internal class ByteCodeBuffer
    {
        private readonly Byte[] _byteCode;
        public IntPtr UnmanagedPtr;
        private int _length;

        public ByteCodeBuffer(Byte[] bytes)
        {
            _byteCode = bytes;
            _length = bytes.Length;

            // 1. 在非托管堆分配内存（大小 = 数组长度）
            UnmanagedPtr = Marshal.AllocHGlobal(_length);
            // 2. 将托管数组内容拷贝到非托管堆
            Marshal.Copy(bytes, 0, UnmanagedPtr, _length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Core.OpCode ReadOpCode(CallFrame frame)
        {
            frame.LastInstructionPointer = frame.Pointer;
            var opCode = (Core.OpCode)_byteCode[frame.Pointer++];
            return opCode;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SByte ReadSByte(CallFrame frame)
        {
            return (SByte)_byteCode[frame.Pointer++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Byte ReadByte(CallFrame frame)
        {
            return _byteCode[frame.Pointer++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int16 ReadInt16(CallFrame frame)
        {
            short value = Unsafe.ReadUnaligned<Int16>(ref _byteCode[frame.Pointer]);
            frame.Pointer += 2;
            return value;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Single ReadFloat(CallFrame frame)
        {
            float value = Unsafe.ReadUnaligned<Single>(ref _byteCode[frame.Pointer]);
            frame.Pointer += 4;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Double ReadDouble(CallFrame frame)
        {
            double value = Unsafe.ReadUnaligned<Double>(ref _byteCode[frame.Pointer]);
            frame.Pointer += 8;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int32 ReadInt32(CallFrame frame)
        {
            int value = Unsafe.ReadUnaligned<int>(ref _byteCode[frame.Pointer]);
            frame.Pointer += 4;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Int64 ReadInt64(CallFrame frame)
        {
            Int64 value = Unsafe.ReadUnaligned<Int64>(ref _byteCode[frame.Pointer]);
            frame.Pointer += 8;
            return value;
        }




        // 手动释放非托管内存（必须实现 IDisposable）
        public void Dispose()
        {
            if (UnmanagedPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(UnmanagedPtr); // 释放非托管内存
                UnmanagedPtr = IntPtr.Zero;
                _length = 0;
            }
            GC.SuppressFinalize(this);
        }

        // 析构函数：防止忘记手动 Dispose 导致内存泄漏
        ~ByteCodeBuffer()
        {
            Dispose();
        }

    }
}
