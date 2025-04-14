using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// </summary>
    internal class CallFrame
    {
        /// <summary>
        /// 当前执行的字节码
        /// </summary>
        public byte[] Bytecode { get; }

        /// <summary>
        /// 指令指针，指向当前执行的指令
        /// </summary>
        public int Pointer { get; set; }

        /// <summary>
        /// 局部变量环境
        /// </summary>
        public Environment Environment { get; }

        /// <summary>
        /// 局部变量数组
        /// </summary>
        public ScriptObject[] Locals { get; }

        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="bytecode">要执行的字节码</param>
        /// <param name="ip">初始指令指针位置</param>
        /// <param name="environment">执行环境</param>
        /// <param name="localCount">局部变量数量</param>
        public CallFrame(byte[] bytecode, int ip, Environment environment, int localCount = 16)
        {
            Bytecode = bytecode;
            Pointer = ip;
            Environment = environment;
            Locals = new ScriptObject[localCount];
        }





        public OpCode ReadOpCode()
        {
            if (Pointer >= Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            return (OpCode)Bytecode[Pointer++];
        }

        public SByte ReadSByte()
        {
            if (Pointer >= Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            return (SByte)Bytecode[Pointer++];
        }

        public Byte ReadByte()
        {
            if (Pointer >= Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            return Bytecode[Pointer++];
        }


        public Int16 ReadInt16()
        {
            if (Pointer + 2 > Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            short value = BitConverter.ToInt16(Bytecode, Pointer);
            Pointer += 2;
            return value;
        }


        public Single ReadFloat()
        {
            if (Pointer + 4 > Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            float value = BitConverter.ToSingle(Bytecode, Pointer);
            Pointer += 4;
            return value;
        }

        public Double ReadDouble()
        {
            if (Pointer + 8 > Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            double value = BitConverter.ToDouble(Bytecode, Pointer);
            Pointer += 8;
            return value;
        }



        public Int32 ReadInt32()
        {
            if (Pointer + 4 > Bytecode.Length)
                throw new InvalidOperationException("指令指针超出范围");
            int value = BitConverter.ToInt32(Bytecode, Pointer);
            Pointer += 4;
            return value;
        }


    }
}
