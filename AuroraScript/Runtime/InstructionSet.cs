using System;
using AuroraScript.Core;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 指令执行上下文，封装当前指令缓冲区与指令指针。
    /// </summary>
    internal ref struct InstructionStream
    {
        public Instruction[] Buffer;
        public int IP;
        public int NextIP;

        public InstructionStream(Instruction[] buffer, int ip)
        {
            Buffer = buffer;
            IP = ip;
            NextIP = ip;
        }

        public ref Instruction Current => ref Buffer[IP];

        public void Advance()
        {
            IP = NextIP;
        }
    }

    /// <summary>
    /// 解析后的指令。Operand 字段根据不同的 opcode 复用。
    /// </summary>
    internal readonly struct Instruction
    {
        public readonly OpCode OpCode;
        public readonly int SourceOffset;
        public readonly int Operand0;
        public readonly long Operand1;
        public readonly double OperandNumber;
        public readonly IntPtr HandlerPtr;

        public Instruction(OpCode opCode, int sourceOffset, int operand0, long operand1, double operandNumber, IntPtr handlerPtr)
        {
            OpCode = opCode;
            SourceOffset = sourceOffset;
            Operand0 = operand0;
            Operand1 = operand1;
            OperandNumber = operandNumber;
            HandlerPtr = handlerPtr;
        }
    }
}

