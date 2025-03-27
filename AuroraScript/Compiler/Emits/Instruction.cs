using AuroraScript.Core;

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


    public class CommentInstruction : Instruction
    {
        public String Comment;
        private int PreEmptyLine;
        public CommentInstruction(String comment, int preEmptyLine) : base(OpCode.NOP, 0)
        {
            this.Comment = comment;
            this.PreEmptyLine = preEmptyLine;
        }

        public override string ToString()
        {
            if (this.PreEmptyLine > 0)
            {
                return $"{"".PadLeft(PreEmptyLine, '\n')}{Comment}";
            }
            return Comment;
        }

    }


}
