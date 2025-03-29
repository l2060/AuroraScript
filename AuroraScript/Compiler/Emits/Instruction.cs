using AuroraScript.Core;


namespace AuroraScript.Compiler.Emits
{
    public abstract class Instruction
    {
        /// <summary>
        /// The operation code.
        /// </summary>
        public OpCode OpCode { get; private set; }

        public readonly int Offset;
        public abstract int Length { get; }

        public Instruction Previous;

        public Instruction Next;

        public void Change(OpCode opCode)
        {
            OpCode = opCode;
        }


        public Instruction(OpCode opCode, int offset)
        {
            OpCode = opCode;
            Offset = offset;
        }

        public abstract void WriteTo(BinaryWriter writer);

    }

    public class Instruction1 : Instruction
    {
        public override int Length => 1;

        public Instruction1(OpCode opCode, int offset) : base(opCode, offset)
        {
        }

        public override string ToString()
        {
            return OpCode.ToString();
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write((Byte)OpCode);
        }
    }

    public class Instruction5 : Instruction
    {
        public int Value;

        public override int Length => 5;

        public Instruction5(OpCode opCode, int offset, int value) : base(opCode, offset)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            if (OpCode == OpCode.PUSH_F32)
            {
                NumberUnion union = new NumberUnion(Value, 0);
                return OpCode + " " + union.FloatValue1;
            }
            return OpCode.ToString() + " " + Value;
        }
        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write((Byte)OpCode);
            writer.Write(Value);
        }
    }

    public class Instruction9 : Instruction
    {

        public NumberUnion Value;

        public override int Length => 9;

        public Instruction9(OpCode opCode, int offset, Double value) : base(opCode, offset)
        {
            this.Value = new NumberUnion(value);
        }

        public override string ToString()
        {
            return OpCode + " " + Value.DoubleValue;
        }

        public override void WriteTo(BinaryWriter writer)
        {
            writer.Write((Byte)OpCode);
            writer.Write(Value.Int32Value1);
            writer.Write(Value.Int32Value2);
        }
        //private void a()
        //{
        //    long bits = Unsafe.As<double, long>(ref Value);
        //    int high = (int)(bits >> 32);
        //    int low = (int)(bits & 0xFFFFFFFF);
        //    // low, high
        //    //NumberUnion union2 = new NumberUnion(operand);
        //}
    }

    public class JumpInstruction : Instruction5
    {
        internal JumpInstruction(OpCode opCode, int offset, int addOffset = 0) : base(opCode, offset, addOffset)
        {
        }

        public override string ToString()
        {
            return $"{OpCode} [{Offset + Length + Value:0000}]";
        }
    }




    public class PositionInstruction : Instruction
    {
        internal PositionInstruction(int offset) : base(OpCode.NOP, offset)
        {
        }

        public override int Length => 0;

        public override void WriteTo(BinaryWriter writer)
        {
        }
    }


    public class CommentInstruction : Instruction
    {
        public override int Length => 0;

        public String Comment;
        private int PreEmptyLine;
        internal CommentInstruction(String comment, int preEmptyLine) : base(OpCode.NOP, 0)
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
        public override void WriteTo(BinaryWriter writer)
        {

        }
    }


}
