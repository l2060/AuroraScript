using AuroraScript.Ast;
using AuroraScript.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace AuroraScript.Compiler.Emits
{
    public class InstructionBuilder
    {
        public int _position { get; private set; }
        private readonly List<Instruction> _instructions = new List<Instruction>();
        private readonly StringList _stringSet;

        public readonly Dictionary<String, Int32> ModuleEntrys = new Dictionary<string, int>();

        public InstructionBuilder(StringList stringSet)
        {
            this._stringSet = stringSet;

        }

        public Instruction LastInstruction
        {
            get
            {
                return _instructions.Count > 0 ? _instructions[_instructions.Count - 1] : null;
            }
        }


        public Boolean IsChanged(Instruction position)
        {
            return position.Offset != _position;
        }


        public Instruction Emit(OpCode opCode)
        {
            var instruction = new Instruction1(opCode, _position);
            return AppendInstruction(instruction);
        }



        public Instruction EmitStr(OpCode opCode, int strAddr)
        {
            var instruction = new InstructionStr(opCode, _position, strAddr);
            instruction.String = _stringSet.List[strAddr];
            return AppendInstruction(instruction);
        }

        public Instruction EmitStr2(OpCode opCode, String str1, String str2)
        {
            var v1 = _stringSet.GetSlot(str1);
            var v2 = _stringSet.GetSlot(str2);
            var instruction = new InstructionStr2(opCode, _position, v1, v2);
            instruction.String1 = _stringSet.List[v1];
            instruction.String2 = _stringSet.List[v2];
            return AppendInstruction(instruction);
        }



        public Instruction Emit(OpCode opCode, int param)
        {
            var instruction = new Instruction5(opCode, _position, param);
            return AppendInstruction(instruction);
        }


        public Instruction Emit(OpCode opCode, SByte param)
        {
            var instruction = new Instruction2S(opCode, _position, param);
            return AppendInstruction(instruction);
        }

        public Instruction Emit(OpCode opCode, Byte param)
        {
            var instruction = new Instruction2U(opCode, _position, param);
            return AppendInstruction(instruction);
        }


        public Instruction Emit(OpCode opCode, Int16 param)
        {
            var instruction = new Instruction3(opCode, _position, param);
            return AppendInstruction(instruction);
        }


        public Instruction Emit(OpCode opCode, Double param)
        {
            var instruction = new InstructionDouble(opCode, _position, param);
            return AppendInstruction(instruction);
        }

        public Instruction Emit(OpCode opCode, Int64 param)
        {
            var instruction = new InstructionInt64(opCode, _position, param);
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
        }



        /// <summary>
        /// 创建闭包指令
        /// </summary>
        /// <param name="localVars">捕获的当前域变量</param>
        /// <returns></returns>
        public ClosureInstruction NewClosure(Int32 captureCount)
        {
            if (captureCount < 0 || captureCount > Byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(captureCount), "Capture count must fit in a byte.");
            }
            var instruction = new ClosureInstruction(_position, (Byte)captureCount);
            AppendInstruction(instruction);
            return instruction;
        }

        public ClosureInstruction NewClosure()
        {
            return NewClosure(0);
        }

        /// <summary>
        /// 申请本地变量栈
        /// </summary>
        /// <returns></returns>
        public AllocLocalsInstruction AllocLocals()
        {
            var instruction = new AllocLocalsInstruction(_position);
            AppendInstruction(instruction);
            return instruction;
        }


        public void FixAllocLocals(AllocLocalsInstruction allocLocals, int num)
        {
            allocLocals.StackSize = num;
        }


        /// <summary>
        /// 将闭包指令指向此处
        /// </summary>
        /// <param name="closure"></param>
        public void FixClosure(ClosureInstruction closure)
        {
            var offset = _position - (closure.Offset + closure.Length);
            closure.Address = offset;
        }

        /// <summary>
        /// 捕获变量，将变量值存储到闭包环境中
        /// </summary>
        /// <param name="index">捕获变量的索引</param>
        public void CaptureVariable(int index)
        {
            Emit(OpCode.CAPTURE_VAR, index);
        }

        /// <summary>
        /// 加载捕获的变量，从闭包环境中加载变量值
        /// </summary>
        /// <param name="index">捕获变量的索引</param>
        public void LoadCapturedVariable(int index)
        {
            Emit(OpCode.LOAD_CAPTURE, index);
        }



        public void StoreCapturedVariable(int index)
        {
            Emit(OpCode.STORE_CAPTURE, index);
        }


        /// <summary>
        /// 获取对象迭代器
        /// </summary>
        public void GetIterator()
        {
            Emit(OpCode.GET_ITERATOR);
        }

        /// <summary>
        /// 获取迭代器当前值
        /// </summary>
        public void IteratorValue()
        {
            Emit(OpCode.ITERATOR_VALUE);
        }

        /// <summary>
        /// 获取迭代器是否有值
        /// </summary>
        public void IteratorHasValue()
        {
            Emit(OpCode.ITERATOR_HAS_VALUE);
        }

        /// <summary>
        /// 迭代器迭代下一个
        /// </summary>
        public void IteratorNext()
        {
            Emit(OpCode.ITERATOR_NEXT);
        }












        public void PushConstantString(int index)
        {
            Emit(OpCode.PUSH_STRING, index);
        }

        public void PushConstantString(String str)
        {
            var strAddress = _stringSet.GetSlot(str);
            EmitStr(OpCode.PUSH_STRING, strAddress);
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
                Emit(OpCode.PUSH_I8, (SByte)operand);
                return;
            }
            if (operand % 1 == 0 && operand >= Int16.MinValue && operand <= Int16.MaxValue)
            {
                Emit(OpCode.PUSH_I16, (Int16)operand);
                return;
            }
            if (operand % 1 == 0 && operand >= Int32.MinValue && operand <= Int32.MaxValue)
            {
                Emit(OpCode.PUSH_I32, (Int32)operand);
                return;
            }
            if (operand % 1 == 0 && operand >= Int64.MinValue && operand <= Int64.MaxValue)
            {
                Emit(OpCode.PUSH_I64, (Int64)operand);
                return;
            }

            if (operand >= Single.MinValue && operand <= Single.MaxValue && (Single)operand == operand)
            {
                UnionNumber union = new UnionNumber((Single)operand, 0.0f);
                Emit(OpCode.PUSH_F32, union.Int32ValueH);
                return;
            }
            Emit(OpCode.PUSH_F64, operand);
        }




        public void Yield()
        {
            Emit(OpCode.YIELD);
        }


        public void Pop()
        {
            Emit(OpCode.POP);
        }

        public void SetElement()
        {
            Emit(OpCode.SET_ELEMENT);
        }

        public void SetProperty(String propertyName)
        {
            var strAddress = _stringSet.GetSlot(propertyName);
            EmitStr(OpCode.SET_PROPERTY, strAddress);
        }

        public void GetProperty(String propertyName)
        {
            var strAddress = _stringSet.GetSlot(propertyName);
            EmitStr(OpCode.GET_PROPERTY, strAddress);
        }

        public void DeleteProperty()
        {
            Emit(OpCode.DELETE_PROPERTY);
        }

        public void GetThisProperty(Int32 nameSlot)
        {
            EmitStr(OpCode.GET_THIS_PROPERTY, nameSlot);
        }

        public void GetThisProperty(String propertyName)
        {
            var strAddress = _stringSet.GetSlot(propertyName);
            EmitStr(OpCode.GET_THIS_PROPERTY, strAddress);
        }

        public void SetThisProperty(String propertyName)
        {
            var strAddress = _stringSet.GetSlot(propertyName);
            EmitStr(OpCode.SET_THIS_PROPERTY, strAddress);
        }
        public void SetThisProperty(Int32 nameSlot)
        {
            EmitStr(OpCode.SET_THIS_PROPERTY, nameSlot);
        }

        public void GetGlobalProperty(String varName)
        {
            var strAddress = _stringSet.GetSlot(varName);
            EmitStr(OpCode.GET_GLOBAL_PROPERTY, strAddress);
        }

        public void SetGlobalProperty(String varName)
        {
            var strAddress = _stringSet.GetSlot(varName);
            EmitStr(OpCode.SET_GLOBAL_PROPERTY, strAddress);
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
            var last = LastInstruction;
            if (last == null) return;
            last.AddComment(comment, preEmptyLine);
        }


        public void DefineModule(ModuleDeclaration node)
        {
            var pos = _position;
            ModuleEntrys.Add(node.ModuleName, pos);
        }




        public void PushThis()
        {
            Emit(OpCode.PUSH_THIS);
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
        public void InitModule(String moduleName)
        {
            var strAddress = _stringSet.GetSlot(moduleName);
            EmitStr(OpCode.INIT_MODULE, strAddress);
        }



        public void NewMap()
        {
            Emit(OpCode.NEW_MAP);
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

        public void LoadArg(Byte index)
        {
            Emit(OpCode.LOAD_ARG, index);
        }

        public void LoadArgIsExist(Byte index)
        {
            Emit(OpCode.TRY_LOAD_ARG, index);
        }

        public void LoadLocal(int index)
        {
            Emit(OpCode.LOAD_LOCAL, index);
        }


        public void StoreLocal(int index)
        {
            Emit(OpCode.STORE_LOCAL, index);
        }

        public void NewRegex(String pattern, String flags)
        {
            EmitStr2(OpCode.NEW_REGEX, pattern,flags);
        }





        public void PushGlobal()
        {
            Emit(OpCode.PUSH_GLOBAL);
        }

        public void PushUserContext()
        {
            Emit(OpCode.PUSH_CONTEXT);
        }

        public void Call(Byte argsCount)
        {
            Emit(OpCode.CALL, argsCount);
        }

        public void Return()
        {
            Emit(OpCode.RETURN);
        }


        public void ReturnNull()
        {
            Emit(OpCode.RETURN_NULL);
        }


        public void ReturnGlobal()
        {
            Emit(OpCode.PUSH_GLOBAL);
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
            Console.WriteLine($"=====================================================================");
            Console.WriteLine($"= Start Code ========================================================");
            Console.WriteLine($"=====================================================================");


            foreach (var instruction in _instructions)
            {
                Console.WriteLine($"[{instruction.Offset:0000}] {instruction}");
                if (instruction.Comments != null)
                {
                    foreach (var comment in instruction.Comments)
                    {
                        var color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(comment);
                        Console.ForegroundColor = color;
                    }
                }
            }

            Console.WriteLine($"=====================================================================");
            Console.WriteLine($"= End Code ==========================================================");
            Console.WriteLine($"=====================================================================");


        }
    }

}