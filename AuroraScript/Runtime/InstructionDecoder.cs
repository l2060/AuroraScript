using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using AuroraScript.Core;

namespace AuroraScript.Runtime
{
    internal static class InstructionDecoder
    {
        public static Instruction[] Decode(byte[] bytecode, IntPtr[] handlerTable)
        {
            if (bytecode == null || bytecode.Length == 0)
            {
                return Array.Empty<Instruction>();
            }

            var instructions = new List<Instruction>(bytecode.Length / 2);
            var byteToInstruction = new Dictionary<int, int>(bytecode.Length / 2);
            var pendingJumps = new List<(int InstructionIndex, int TargetByteOffset)>();

            var ip = 0;
            while (ip < bytecode.Length)
            {
                byteToInstruction[ip] = instructions.Count;
                var sourceOffset = ip;
                var opCode = (OpCode)bytecode[ip++];
                var operand0 = 0;
                long operand1 = 0;
                double operandNumber = double.NaN;

                switch (opCode)
                {
                    case OpCode.NOP:
                    case OpCode.POP:
                    case OpCode.DUP:
                    case OpCode.SWAP:
                    case OpCode.NEW_MAP:
                    case OpCode.SET_ELEMENT:
                    case OpCode.GET_ELEMENT:

                    case OpCode.PUSH_0:
                    case OpCode.PUSH_1:
                    case OpCode.PUSH_2:
                    case OpCode.PUSH_3:
                    case OpCode.PUSH_4:
                    case OpCode.PUSH_5:
                    case OpCode.PUSH_6:
                    case OpCode.PUSH_7:
                    case OpCode.PUSH_8:
                    case OpCode.PUSH_9:
                    case OpCode.PUSH_NULL:
                    case OpCode.PUSH_FALSE:
                    case OpCode.PUSH_TRUE:
                    case OpCode.PUSH_THIS:
                    case OpCode.PUSH_GLOBAL:
                    case OpCode.PUSH_CONTEXT:
                    case OpCode.PUSH_ARGUMENTS:


                    case OpCode.GET_ITERATOR:
                    case OpCode.ITERATOR_VALUE:
                    case OpCode.ITERATOR_HAS_VALUE:
                    case OpCode.ITERATOR_NEXT:
                    case OpCode.TYPEOF:

                    case OpCode.ADD:
                    case OpCode.DELETE_PROPERTY:

                    case OpCode.DECONSTRUCT_ARRAY:
                    case OpCode.DECONSTRUCT_MAP:
                        

                    case OpCode.SUBTRACT:
                    case OpCode.MULTIPLY:
                    case OpCode.DIVIDE:
                    case OpCode.MOD:
                    case OpCode.NEGATE:
                    case OpCode.INCREMENT:

                    case OpCode.DECREMENT:
                    case OpCode.BIT_NOT:
                    case OpCode.BIT_SHIFT_L:
                    case OpCode.BIT_SHIFT_R:
                    case OpCode.BIT_USHIFT_R:
                    case OpCode.BIT_AND:
                    case OpCode.BIT_OR:

                    case OpCode.BIT_XOR:
                    case OpCode.LOGIC_NOT:
                    case OpCode.LOGIC_AND:
                    case OpCode.LOGIC_OR:

                    case OpCode.EQUAL:
                    case OpCode.NOT_EQUAL:
                    case OpCode.LESS_THAN:
                    case OpCode.LESS_EQUAL:
                    case OpCode.GREATER_THAN:
                    case OpCode.GREATER_EQUAL:
                    case OpCode.YIELD:


                    case OpCode.RETURN:
                    case OpCode.RETURN_NULL:
                        break;



                    case OpCode.CALL:
                    case OpCode.LOAD_ARG:
                    case OpCode.TRY_LOAD_ARG:
                    case OpCode.LOAD_LOCAL:
                    case OpCode.STORE_LOCAL:
                    case OpCode.INC_LOCAL:
                    case OpCode.DEC_LOCAL:
                    case OpCode.INC_LOCAL_POST:
                    case OpCode.DEC_LOCAL_POST:
                    case OpCode.ADD_LOCAL_STACK:
                    case OpCode.SUB_LOCAL_STACK:
                    case OpCode.MUL_LOCAL_STACK:
                    case OpCode.DIV_LOCAL_STACK:
                    case OpCode.MOD_LOCAL_STACK:
                        operand0 = bytecode[ip++];
                        break;




                    case OpCode.INC_LOCAL_L:
                    case OpCode.DEC_LOCAL_L:
                    case OpCode.INC_LOCAL_POST_L:
                    case OpCode.DEC_LOCAL_POST_L:
                    case OpCode.ADD_LOCAL_STACK_L:
                    case OpCode.SUB_LOCAL_STACK_L:
                    case OpCode.MUL_LOCAL_STACK_L:
                    case OpCode.DIV_LOCAL_STACK_L:
                    case OpCode.MOD_LOCAL_STACK_L:


                    case OpCode.ALLOC_LOCALS:
                    case OpCode.LOAD_LOCAL_L:
                    case OpCode.STORE_LOCAL_L:
                    case OpCode.LOAD_CAPTURE:
                    case OpCode.STORE_CAPTURE:
                    case OpCode.CAPTURE_VAR:
                    case OpCode.PUSH_STRING:
                    case OpCode.GET_PROPERTY:
                    case OpCode.SET_PROPERTY:
                    case OpCode.GET_THIS_PROPERTY:
                    case OpCode.SET_THIS_PROPERTY:
                    case OpCode.GET_GLOBAL_PROPERTY:
                    case OpCode.SET_GLOBAL_PROPERTY:
                    case OpCode.INIT_MODULE:
                    case OpCode.NEW_ARRAY:

                        operand0 = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        break;

                    case OpCode.NEW_REGEX:
                        operand0 = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        operand1 = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        break;





                    case OpCode.PUSH_I8:
                        operand0 = (sbyte)bytecode[ip++];
                        break;
                    case OpCode.PUSH_I16:
                        operand0 = BinaryPrimitives.ReadInt16LittleEndian(bytecode.AsSpan(ip));
                        ip += 2;
                        break;
                    case OpCode.PUSH_I32:
                        operand0 = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        break;
                    case OpCode.PUSH_I64:
                        operand1 = BinaryPrimitives.ReadInt64LittleEndian(bytecode.AsSpan(ip));
                        ip += 8;
                        break;
                    case OpCode.PUSH_F32:
                        operandNumber = BinaryPrimitives.ReadSingleLittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        break;
                    case OpCode.PUSH_F64:
                        operandNumber = BinaryPrimitives.ReadDoubleLittleEndian(bytecode.AsSpan(ip));
                        ip += 8;
                        break;

                    case OpCode.JUMP:

                    case OpCode.JUMP_IF_FALSE:
                    case OpCode.JUMP_IF_TRUE:
                        var offset = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        pendingJumps.Add((instructions.Count, ip + offset));
                        break;


                    case OpCode.CREATE_CLOSURE:
                        operand0 = BinaryPrimitives.ReadInt32LittleEndian(bytecode.AsSpan(ip));
                        ip += 4;
                        operand1 = bytecode[ip++];
                        pendingJumps.Add((instructions.Count, ip + operand0));
                        break;



                    default:
                        // 对尚未列出的指令，保留字节偏移，稍后可扩展
                        break;
                }

                var handlerPtr = handlerTable[(int)opCode];
                var instruction = new Instruction(opCode, sourceOffset, operand0, operand1, operandNumber, handlerPtr);
                instructions.Add(instruction);
            }

            // 跳转目标回填
            for (int i = 0; i < pendingJumps.Count; i++)
            {
                var (instructionIndex, targetByteOffset) = pendingJumps[i];
                if (byteToInstruction.TryGetValue(targetByteOffset, out var targetIndex))
                {
                    var inst = instructions[instructionIndex];
                    instructions[instructionIndex] = new Instruction(
                        inst.OpCode,
                        inst.SourceOffset,
                        targetIndex,
                        inst.Operand1,
                        inst.OperandNumber,
                        inst.HandlerPtr);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid jump target: {targetByteOffset}");
                }
            }

            return instructions.ToArray();
        }
    }
}


