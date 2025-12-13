using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraScript.Runtime.Util
{

    internal unsafe static class CpuTimer
    {
        private static readonly delegate*<ulong> rdtscPtr;
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, int flNewProtect, out int lpflOldProtect);

        static CpuTimer()
        {
            // 分配可执行内存
            byte* code = (byte*)NativeMemory.Alloc((nuint)16, 16);
            // RDTSC 指令 + ret
            code[0] = 0x0F;
            code[1] = 0x31;
            code[2] = 0x48;
            code[3] = 0xC1;
            code[4] = 0xE2;
            code[5] = 0x20;
            code[6] = 0x48;
            code[7] = 0x09;
            code[8] = 0xD0;
            code[9] = 0xC3;


            rdtscPtr = (delegate*<ulong>)code;

            int outMemProtect;
            if (!VirtualProtect((IntPtr)code, 16, 64, out outMemProtect))
            {
                Console.WriteLine();
            }
        }









        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Ticks()
        {
            return rdtscPtr();
        }
    }
}
