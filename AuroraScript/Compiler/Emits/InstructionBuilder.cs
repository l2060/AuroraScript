using AuroraScript.Core;
using System.Runtime.CompilerServices;


namespace AuroraScript.Compiler.Emits
{
    internal class InstructionBuilder
    {
        private readonly List<Instruction> _instructions = new List<Instruction>();

        public int _position { get; private set; }

        public Instruction LastInstruction
        {
            get
            {
                var count = _instructions.Count;
                return count > 0 ? _instructions[count - 1] : null;
            }
        }

        public Instruction FirstInstruction
        {
            get
            {
                return _instructions.Count > 0 ? _instructions[0] : null;
            }
        }




        private Byte[] _buffer = new byte[1024];

        public Instruction Emit(OpCode opCode)
        {
            return AppendInstruction(opCode, _position);
        }





        public Instruction Emit(OpCode opCode, params int[] param)
        {
            return AppendInstruction(opCode, _position, param);
        }

        private Instruction AppendInstruction(OpCode opCode, int offset, params int[] operands)
        {
            var instruction = new Instruction(opCode, _position, operands);
            var last = LastInstruction;
            if (last != null)
            {
                instruction.Previous = last;
                last.Next = instruction;
            }
            _instructions.Add(instruction);
            _position += instruction.Length;
            return instruction;
        }




        public Instruction PushConstInt(int operand)
        {
            if (operand >= 0 && operand <= 5)
            {
                return Emit((OpCode)((int)OpCode.PUSH_0 + operand));
            }
            return Emit(OpCode.PUSH_CONST_INT, operand);
        }

        
        public Instruction Position()
        {
            return new Instruction(OpCode.NOP, _position);
        }


        public Instruction Pop()
        {
            return Emit(OpCode.POP);
        }

        public Instruction SetElement()
        {
            return Emit(OpCode.SET_ELEMENT);
        }
        



        public Instruction PushNull()
        {
            return Emit(OpCode.PUSH_NULL);
        }

        public Instruction Duplicate()
        {
            return Emit(OpCode.DUP);
        }

        public Instruction NewArray( int count)
        {
            return Emit(OpCode.NEW_ARRAY, count);
        }



        public Instruction PushConstDouble(Double operand)
        {
            if (operand % 1 == 0 && operand < Int32.MaxValue)
            {
                return Emit(OpCode.PUSH_CONST_INT, (Int32)operand);
            }
            long bits = Unsafe.As<double, long>(ref operand);
            int high = (int)(bits >> 32);
            int low = (int)(bits & 0xFFFFFFFF);
            return Emit(OpCode.PUSH_CONST_DOUBLE, low, high);
        }

        public Instruction Jump()
        {
            return Emit(OpCode.JUMP, 0);
        }


        public Instruction JumpTo(Instruction position)
        {
            var jump = Emit(OpCode.JUMP, 0);
            var offset = _position - position.Offset;
            jump.Operands[0] = offset;
            return jump;
        }
        



        public Instruction JumpFalse()
        {
            return Emit(OpCode.JUMP_IF_FALSE, 0);
        }

        public void FixJump(Instruction jump, Instruction to)
        {
            var offset = to.Offset - jump.Offset;
            jump.Operands[0] = offset;
        }

        public void FixJump(Instruction jump)
        {
            var offset = _position - jump.Offset;
            jump.Operands[0] = offset;
        }

        public Instruction LoadArg(int index)
        {
            return Emit(OpCode.LOAD_ARG, index);
        }


        public Instruction StoreLocal(int index)
        {
            return Emit(OpCode.STORE_LOCAL, index);
        }


        public Instruction Return()
        {
            return Emit(OpCode.RETURN);
        }

    }
}
