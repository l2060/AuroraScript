using AuroraScript.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Emits
{
    public class Instruction

    {
        /// <summary>
        /// The operation code.
        /// </summary>
        public readonly OpCode OpCode;

        /// <summary>
        /// The instruction operands, if any.
        /// </summary>
        public readonly int[] Operands;

        public readonly int Offset;

        public readonly int Length;

        public Instruction Previous;

        public Instruction Next;

        public Instruction(OpCode opCode, int offset, params int[] operands)
        {
            OpCode = opCode;
            Operands = operands;
            Offset = offset;
            Length = operands.Length * sizeof(int) + 1;
        }


        public override string ToString()
        {
            if (Operands.Length == 0) return OpCode.ToString();
            return $"{OpCode} {String.Join(", ", Operands)}";
        }

    }
}
