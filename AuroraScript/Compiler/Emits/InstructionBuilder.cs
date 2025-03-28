using AuroraScript.Core;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;


namespace AuroraScript.Compiler.Emits
{
    internal class InstructionBuilder
    {
        private readonly List<string> _stringTable = new List<string>();
        public InstructionBuilder(List<string> stringTable)
        {
            _stringTable = stringTable;
        }



        private readonly List<Instruction> _instructions = new List<Instruction>();

        public int _position { get; private set; }

        public Instruction LastInstruction
        {
            get
            {
                for (int i = _instructions.Count - 1; i >= 0; i--)
                {
                    if (_instructions[i] is CommentInstruction) continue;
                    return _instructions[i];
                }
                return null;
            }
        }

        public Instruction FirstInstruction
        {
            get
            {
                return _instructions.Count > 0 ? _instructions[0] : null;
            }
        }

        public int GetOrAddStringTable(String str)
        {
            var index = _stringTable.IndexOf(str);
            if (index > -1) return index;
            _stringTable.Add(str);
            return _stringTable.Count - 1;
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



        public Instruction Position()
        {
            return new Instruction(OpCode.NOP, _position);
        }
        public Instruction PushConstantString(int index)
        {
            return Emit(OpCode.PUSH_STRING, index);
        }

        public Instruction PushConstantString(String str)
        {
            var strAddress = GetOrAddStringTable(str);
            return Emit(OpCode.PUSH_STRING, strAddress);
        }

        public Instruction PushConstantNumber(Double operand)
        {
            if (operand % 1 == 0 && operand >= SByte.MinValue && operand <= SByte.MaxValue)
            {
                if (operand >= 0 && operand <= 9)
                {
                    return Emit((OpCode)((Int32)OpCode.PUSH_0 + (Int32)operand));
                }
                return Emit(OpCode.PUSH_I8, (Int32)operand);
            }
            if (operand % 1 == 0 && operand >= Int16.MinValue && operand <= Int16.MaxValue)
            {
                return Emit(OpCode.PUSH_I16, (Int32)operand);
            }
            if (operand % 1 == 0 && operand >= Int32.MinValue && operand <= Int32.MaxValue)
            {
                return Emit(OpCode.PUSH_I32, (Int32)operand);
            }
            if (operand >= Single.MinValue && operand <= Single.MaxValue)
            {
                NumberUnion union = new NumberUnion((Single)operand);
                return Emit(OpCode.PUSH_F32, union.Int32Value1);
            }
            long bits = Unsafe.As<double, long>(ref operand);
            int high = (int)(bits >> 32);
            int low = (int)(bits & 0xFFFFFFFF);
            //NumberUnion union2 = new NumberUnion(operand);
            return Emit(OpCode.PUSH_F64, low, high);
        }






        public Instruction Pop()
        {
            return Emit(OpCode.POP);
        }

        public Instruction SetElement()
        {
            return Emit(OpCode.SET_ELEMENT);
        }
        public Instruction SetProperty()
        {
            return Emit(OpCode.SET_PROPERTY);
        }
        public Instruction GetProperty()
        {
            return Emit(OpCode.GET_PROPERTY);
        }
        public Instruction GetElement()
        {
            return Emit(OpCode.GET_ELEMENT);
        }


        public Instruction PushConstantBoolean(Boolean value)
        {
            return Emit(value ? OpCode.PUSH_TRUE : OpCode.PUSH_FALSE);
        }

        [Conditional("DEBUG")]
        public void Comment(String comment, int preEmptyLine = 0)
        {
            _instructions.Add(new CommentInstruction(comment, preEmptyLine));
        }


        public Instruction PushNull()
        {
            return Emit(OpCode.PUSH_NULL);
        }

        public Instruction Duplicate()
        {
            return Emit(OpCode.DUP);
        }

        public Instruction NewArray(int count)
        {
            return Emit(OpCode.NEW_ARRAY, count);
        }


        public Instruction NewMap(int count)
        {
            return Emit(OpCode.NEW_MAP, count);
        }


        public Instruction Jump()
        {
            return Emit(OpCode.JUMP, 0);
        }


        public Instruction JumpTo(Instruction position)
        {
            var jump = Emit(OpCode.JUMP, 0);
            var offset =  position.Offset - _position;
            jump.Operands[0] = offset + jump.Length;
            return jump;
        }

        public Instruction Call(int argsCount)
        {
            return Emit(OpCode.CALL, argsCount);
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

        public Instruction PushArg(int index)
        {
            return Emit(OpCode.LOAD_ARG, index);
        }

        public Instruction PushArgExist(int index)
        {
            return Emit(OpCode.LOAD_ARG2, index);
        }
        


        public Instruction PushLocal(int index)
        {
            return Emit(OpCode.PUSH_LOCAL, index);
        }

        public Instruction PushGlobal(String varName)
        {
            var strAddress = GetOrAddStringTable(varName);
            return Emit(OpCode.PUSH_GLOBAL, strAddress);
        }
        public Instruction PopLocal(int index)
        {
            return Emit(OpCode.POP_TO_LOCAL, index);
        }

        public Instruction PopGlobal(String varName)
        {
            var strAddress = GetOrAddStringTable(varName);
            return Emit(OpCode.POP_TO_GLOBAL, strAddress);
        }


        public Instruction Return()
        {
            return Emit(OpCode.RETURN);
        }









        public Byte[] Build()
        {
            using (var stream = new MemoryStream())
            {
                using (var write = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    foreach (var instruction in _instructions)
                    {
                        write.Write((Byte)instruction.OpCode);
                        if (instruction.Operands != null)
                        {
                            foreach (var operand in instruction.Operands)
                            {
                                write.Write(operand);
                            }
                        }
                    }
                }
                return stream.ToArray();
            }
        }

        public void DumpCode()
        {
            var index = 0;
            foreach (var instruction in _instructions)
            {
                if (instruction is CommentInstruction)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(instruction);
                    Console.ForegroundColor = color;
                }
                else
                {
                    Console.WriteLine($"[{instruction.Offset:0000}] {instruction}");
                }
            }

        }
    }

}