using AuroraScript.Core;
using System.Diagnostics;
using System.Text;


namespace AuroraScript.Compiler.Emits
{
    internal class InstructionBuilder
    {
        public int _position { get; private set; }
        private readonly List<string> _stringTable = new List<string>();
        private readonly List<Instruction> _instructions = new List<Instruction>();


        public InstructionBuilder(List<string> stringTable)
        {
            _stringTable = stringTable;
        }

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


        public Boolean IsChanged(Instruction position)
        {
            return position.Offset != _position;
        }


        public int GetOrAddStringTable(String str)
        {
            var index = _stringTable.IndexOf(str);
            if (index > -1) return index;
            _stringTable.Add(str);
            return _stringTable.Count - 1;
        }


        public Instruction Emit(OpCode opCode)
        {
            var instruction = new Instruction1(opCode, _position);
            return AppendInstruction(instruction);
        }

        public Instruction Emit(OpCode opCode, int param)
        {
            var instruction = new Instruction5(opCode, _position, param);
            return AppendInstruction(instruction);
        }

        public Instruction Emit(OpCode opCode, Double param)
        {
            var instruction = new Instruction9(opCode, _position, param);
            return AppendInstruction(instruction);
        }

        private Instruction AppendInstruction(Instruction instruction)
        {
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



        public PositionInstruction Position()
        {
            return new PositionInstruction(_position);
        }

        public JumpInstruction Jump()
        {
            var instruction = new JumpInstruction(OpCode.JUMP, _position);
            AppendInstruction(instruction);
            return instruction;
        }

        public JumpInstruction JumpFalse()
        {
            var instruction = new JumpInstruction(OpCode.JUMP_IF_FALSE, _position);
            AppendInstruction(instruction);
            return instruction;
        }

        public void JumpTo(PositionInstruction position)
        {
            var offset = position.Offset - (_position + 5);
            var instruction = new JumpInstruction(OpCode.JUMP, _position, offset);
            AppendInstruction(instruction);
            //var jump = Emit(OpCode.JUMP, 0);
            //jump.Operands[0] = offset;
        }


        public void PushConstantString(int index)
        {
            Emit(OpCode.PUSH_STRING, index);
        }

        public void PushConstantString(String str)
        {
            var strAddress = GetOrAddStringTable(str);
            Emit(OpCode.PUSH_STRING, strAddress);
        }

        public void PushConstantNumber(Double operand)
        {
            if (operand % 1 == 0 && operand >= SByte.MinValue && operand <= SByte.MaxValue)
            {
                if (operand >= 0 && operand <= 9)
                {
                    Emit((OpCode)((Int32)OpCode.PUSH_0 + (Int32)operand));
                    return;
                }
                Emit(OpCode.PUSH_I8, (Int32)operand);
                return;
            }
            if (operand % 1 == 0 && operand >= Int16.MinValue && operand <= Int16.MaxValue)
            {
                Emit(OpCode.PUSH_I16, (Int32)operand);
                return;
            }
            if (operand % 1 == 0 && operand >= Int32.MinValue && operand <= Int32.MaxValue)
            {
                Emit(OpCode.PUSH_I32, (Int32)operand);
                return;
            }
            if (operand >= Single.MinValue && operand <= Single.MaxValue && (Single)operand == operand)
            {
                NumberUnion union = new NumberUnion((Single)operand);
                Emit(OpCode.PUSH_F32, union.Int32Value1);
                return;
            }
            Emit(OpCode.PUSH_F64, operand);
        }






        public void Pop()
        {
            Emit(OpCode.POP);
        }

        public void SetElement()
        {
            Emit(OpCode.SET_ELEMENT);
        }
        public void SetProperty()
        {
            Emit(OpCode.SET_PROPERTY);
        }
        public void GetProperty()
        {
            Emit(OpCode.GET_PROPERTY);
        }
        public void GetElement()
        {
            Emit(OpCode.GET_ELEMENT);
        }


        public void PushConstantBoolean(Boolean value)
        {
            Emit(value ? OpCode.PUSH_TRUE : OpCode.PUSH_FALSE);
        }

        [Conditional("DEBUG")]
        public void Comment(String comment, int preEmptyLine = 0)
        {
            _instructions.Add(new CommentInstruction(comment, preEmptyLine));
        }


        public void PushNull()
        {
            Emit(OpCode.PUSH_NULL);
        }

        public void Duplicate()
        {
            Emit(OpCode.DUP);
        }

        public void NewArray(int count)
        {
            Emit(OpCode.NEW_ARRAY, count);
        }


        public void NewMap(int count)
        {
            Emit(OpCode.NEW_MAP, count);
        }

        public void Call(int argsCount)
        {
            Emit(OpCode.CALL, argsCount);
        }



        public void FixJump(JumpInstruction jump, Instruction to)
        {
            var offset = to.Offset - (jump.Offset + jump.Length);
            jump.Value = offset;
        }

        public void FixJumpToHere(JumpInstruction jump)
        {
            var offset = _position - (jump.Offset + jump.Length);
            jump.Value = offset;
        }

        public void LoadArg(int index)
        {
            Emit(OpCode.LOAD_ARG, index);
        }

        public void LoadArgIsExist(int index)
        {
            Emit(OpCode.LOAD_ARG2, index);
        }

        public void PushLocal(int index)
        {
            //var last = LastInstruction;
            //if (last != null)
            //{
            //    if (last.OpCode == OpCode.POP_TO_LOCAL && index == last.Operands[0])
            //    {
            //        last.Change(OpCode.MOV_TO_LOCAL);
            //        return;
            //    }
            //}
            Emit(OpCode.PUSH_LOCAL, index);
        }
        public void PushGlobal(String varName)
        {
            var strAddress = GetOrAddStringTable(varName);
            //var last = LastInstruction;
            //if (last.OpCode == OpCode.POP_TO_GLOBAL && strAddress == last.Operands[0])
            //{
            //    last.Change(OpCode.MOV_TO_GLOBAL);
            //    return;
            //}
            Emit(OpCode.PUSH_GLOBAL, strAddress);
        }

        public void PopLocal(int index)
        {
            Emit(OpCode.POP_TO_LOCAL, index);
        }

        public void PopGlobal(String varName)
        {
            var strAddress = GetOrAddStringTable(varName);
            Emit(OpCode.POP_TO_GLOBAL, strAddress);
        }


        public void Return()
        {
            Emit(OpCode.RETURN);
        }

        public Byte[] Build()
        {
            using (var stream = new MemoryStream())
            {
                using (var write = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    foreach (var instruction in _instructions)
                    {
                        instruction.WriteTo(write);
                    }
                }
                return stream.ToArray();
            }
        }

        public void DumpCode()
        {
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