using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Emits;
using AuroraScript.Core;


namespace AuroraScript.Compiler
{
    public class ByteCodeGenerator : IAstVisitor
    {
        private CodeScope _scope = new CodeScope(null);
        private InstructionBuilder _instructionBuilder = new InstructionBuilder();

        private Stack<List<Instruction>> _breakJumps = new Stack<List<Instruction>>();
        private Stack<List<Instruction>> _continueJumps = new Stack<List<Instruction>>();

        private readonly Dictionary<string, Instruction> _functionLocations = new Dictionary<string, Instruction>();

        
        private void BeginScope()
        {
            _scope = _scope.Promotion();
        }

        private void EndScope()
        {
            _scope = _scope.Demotion();
        }
        private void DeclareLocalVariable()
        {

        }




        public void VisitProgram(ModuleDeclaration node)
        {
            // Second pass: compile each function
            foreach (var function in node.Functions)
            {
                // Reset local state for each function
                //_currentFunctionName = function.Identifier.Symbol.Name;
                // Record the function's location
                _functionLocations[function.Identifier.Value] = _instructionBuilder.Position();
                // Compile the function
                function.Accept(this);
            }

            _functionLocations["__DOMAIN__"] = _instructionBuilder.Position();
            
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }


            // Convert instructions to bytecode
            //var bytecode = new List<byte>();
            //foreach (var instruction in _instructions)
            //{
            //    bytecode.Add((byte)instruction.OpCode);

            //    // Add operands if any
            //    if (instruction.Operands != null)
            //    {
            //        foreach (var operand in instruction.Operands)
            //        {
            //            bytecode.AddRange(BitConverter.GetBytes(operand));
            //        }
            //    }
            //}

            //return bytecode.ToArray();
            Console.WriteLine(_instructionBuilder);
        }


        public void VisitArrayAccessExpression(ArrayAccessExpression node)
        {
            Console.WriteLine("ArrayAccess");
        }

        public void VisitArrayExpression(ArrayLiteralExpression node)
        {
            // Create a new array with the specified initial capacity
            _instructionBuilder.NewArray(node.ChildNodes.Count);
            // Populate the array with each element
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                // Duplicate the array reference
                _instructionBuilder.Duplicate();
                // Push the index
                _instructionBuilder.PushConstInt(i);
                // Push the element value
                node.ChildNodes[i].Accept(this);
                // Set the element
                _instructionBuilder.SetElement();
                // Pop the result of SET_ELEMENT (the value)
                _instructionBuilder.Pop();
            }

        }




        public void VisitAssignmentExpression(AssignmentExpression node)
        {
            // Compile the value expression
            node.Right.Accept(this);

            // Duplicate the value on the stack (for the assignment expression's own value)
            _instructionBuilder.Duplicate();

            if (node.Left is NameExpression varName)
            {

            } else if (node.Left is MemberAccessExpression access)
            {
                // set
            }

            // 如果赋值表达式左边为成员访问则将赋值/成员访问表达式换为 属性设置表达式

            // Store the value in the variable
            if (_scope.TryGetVariablee(node.Left.ToString(), out var slot))
            {
                _instructionBuilder.StoreLocal(slot);
            }
            else
            {
                // If not a local, it's a global
                //_globalVariables.Add(node.Name);
                //int constIndex = GetOrAddConstant(node.Name);
                //EmitInstruction(OpCode.STORE_GLOBAL, constIndex);
            }
        }



        public void VisitBlock(BlockStatement node)
        {
            BeginScope();
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }
            EndScope();
        }

        public void VisitBreakExpression(BreakStatement node)
        {
            _breakJumps.Peek().Add(_instructionBuilder.Jump());
        }

        public void VisitContinueExpression(ContinueStatement node)
        {
            _continueJumps.Peek().Add(_instructionBuilder.Jump());
        }



        public void VisitCallExpression(FunctionCallExpression node)
        {

        }


        public void VisitDeconstructionExpression(DeconstructionExpression node)
        {

        }

        public void VisitForStatement(ForStatement node)
        {

        }

        public void VisitFunction(FunctionDeclaration node)
        {
            BeginScope();

            // define arg var
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }
            // load args
            for (int i = 0; i < node.Parameters.Count; i++)
            {
                _instructionBuilder.LoadArg(i);
                _instructionBuilder.StoreLocal(1 /* ResolveLocalVariable(node.Parameters[i]) */);
            }
            // Compile the function body
            node.Body.Accept(this);
            // If the function doesn't end with a return, add an implicit return null
            var lastInstruction = _instructionBuilder.LastInstruction;
            if (lastInstruction.OpCode != OpCode.RETURN)
            {
                _instructionBuilder.PushNull();
                _instructionBuilder.Return();
            }
            EndScope();
        }

        public void VisitGetPropertyExpression(MemberAccessExpression node)
        {

        }

        public void VisitGroupingExpression(GroupExpression node)
        {

        }

        public void VisitIfStatement(IfStatement node)
        {

        }

        public void VisitLambdaExpression(LambdaExpression node)
        {

        }

        public void VisitLiteralExpression(LiteralExpression node)
        {

        }

        public void VisitMapExpression(ObjectLiteralExpression node)
        {

        }

        public void VisitName(NameExpression node)
        {

        }


        public void VisitReturnStatement(ReturnStatement node)
        {

        }

        public void VisitSetPropertyExpression(PropertyAssignmentExpression node)
        {

        }


        public void VisitBinaryExpression(BinaryExpression node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            // Emit the appropriate instruction based on the operator

            if (node.Operator.Symbol == Symbols.OP_PLUS)
            {
                _instructionBuilder.Emit(OpCode.ADD);
            }else if (node.Operator.Symbol == Symbols.OP_MINUS)
            {
                _instructionBuilder.Emit(OpCode.SUBTRACT);
            }
            else if (node.Operator.Symbol == Symbols.OP_MULTIPLY)
            {
                _instructionBuilder.Emit(OpCode.MULTIPLY);
            }
            else if (node.Operator.Symbol == Symbols.OP_DIVIDE)
            {
                _instructionBuilder.Emit(OpCode.DIVIDE);
            }
            else if (node.Operator.Symbol == Symbols.OP_EQUALITY)
            {
                _instructionBuilder.Emit(OpCode.EQUAL);
            }
            else if (node.Operator.Symbol == Symbols.OP_INEQUALITY)
            {
                _instructionBuilder.Emit(OpCode.NOT_EQUAL);
            }
            else if (node.Operator.Symbol == Symbols.OP_LESSTHAN)
            {
                _instructionBuilder.Emit(OpCode.LESS_THAN);
            }
            else if (node.Operator.Symbol == Symbols.OP_GREATERTHAN)
            {
                _instructionBuilder.Emit(OpCode.GREATER_THAN);
            }
            else if (node.Operator.Symbol == Symbols.OP_LESS_EQUAL)
            {
                _instructionBuilder.Emit(OpCode.LESS_EQUAL);
            }
            else if (node.Operator.Symbol == Symbols.OP_GREATER_EQUAL)
            {
                _instructionBuilder.Emit(OpCode.GREATER_EQUAL);
            }
            else
            {
                throw new NotSupportedException($"Unsupported binary operator: {node.Operator.Symbol}");
            }
        }


        public void VisitUnaryExpression(UnaryExpression node)
        {

            if (node.Type == UnaryType.Prefix)
            {


            }
            else
            {

            }


            // Compile the operand
            //node.Right.Accept(this);

            //// Emit the appropriate instruction based on the operator
            //switch (node.Operator.Type)
            //{
            //    case TokenType.Minus:
            //        EmitInstruction(OpCode.NEGATE);
            //        break;
            //    case TokenType.Not:
            //        EmitInstruction(OpCode.NOT);
            //        break;
            //    default:
            //        throw new NotSupportedException($"Unsupported unary operator: {node.Operator.Type}");
            //}

        }



        public void VisitVarDeclaration(VariableDeclaration node)
        {
            if (node.Initializer != null)
            {
                node.Initializer.Accept(this);
            }
            else
            {
                _instructionBuilder.PushNull();
            }
            // Local variable
            foreach (var item in node.Variables)
            {
                _instructionBuilder.Duplicate();
                var slot = _scope.DeclareLocalVariable(item.Value);
                _instructionBuilder.StoreLocal(slot);
            }
            // 暂时先这么写 前面 Duplicate 冗余了一个
            _instructionBuilder.Pop();
        }

        public void VisitWhileStatement(WhileStatement node)
        {
            var begin = _instructionBuilder.Position();
            // Set up break and continue targets
            _breakJumps.Push(new List<Instruction>());
            _continueJumps.Push(new List<Instruction>());
            // Compile condition
            node.Condition.Accept(this);
            // Jump out of loop if condition is false
            var exitJump = _instructionBuilder.JumpFalse();
            // Pop the condition value
            _instructionBuilder.Pop();
            // Compile the loop body
            node.Body.Accept(this);

            // Patch continue jumps to point to the condition check
            foreach (var continueJump in _continueJumps.Peek())
            {
                //PatchJumpTo(continueJump, loopStart);
                _instructionBuilder.FixJump(continueJump, begin);
            }
            _instructionBuilder.JumpTo(begin);
            _instructionBuilder.FixJump(exitJump);
        }
    }
}
