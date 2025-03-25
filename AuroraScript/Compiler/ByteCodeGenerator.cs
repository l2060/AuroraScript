using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
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

        private readonly List<string> _stringTable = new List<string>();

        private void BeginScope()
        {
            _scope = _scope.Promotion();
        }

        private void EndScope()
        {
            _scope = _scope.Demotion();
        }


        private int GetOrAddStringTable(String str)
        {
            var index = _stringTable.IndexOf(str);
            if (index > -1) return index;
            _stringTable.Add(str);
            return _stringTable.Count - 1;

        }


        public void VisitProgram(ModuleDeclaration node)
        {
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }
            _instructionBuilder.PushNull();
            _instructionBuilder.Return();
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

            // Convert instructions to bytecode
            var bytes = _instructionBuilder.Build();

            //return bytecode.ToArray();
             Console.WriteLine(bytes);
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
            // Compile each argument
            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }

            // Compile the callee
            node.Target.Accept(this);
            // Emit the call instruction with the argument count
            _instructionBuilder.Call(node.Arguments.Count);

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
        public void VisitGetElementExpression(GetElementExpression node)
        {
            node.Object.Accept(this);
            var index = (LiteralExpression)node.Index;
            if (index.Token.Type == Tokens.ValueType.String)
            {
                // get property
                var strIndex = GetOrAddStringTable(index.Token.Value);
                _instructionBuilder.PushConstInt(strIndex);
                _instructionBuilder.GetProperty();
            }
            else if (index.Token.Type == Tokens.ValueType.IntegerNumber)
            {
                // get element
                _instructionBuilder.PushConstInt((int)index.Value);
                _instructionBuilder.GetElement();
            }
            else
            {
                throw new Exception("无效的索引");
            }
        }

        public void VisitGetPropertyExpression(GetPropertyExpression node)
        {
            node.Object.Accept(this);
            var propery = (NameExpression)node.Property;
            var index = GetOrAddStringTable(propery.Identifier.Value);
            _instructionBuilder.PushConstInt(index);
            //node.Property.Accept(this);
            _instructionBuilder.GetProperty();
        }



        public void VisitSetElementExpression(SetElementExpression node)
        {
            // Compile the value expression
            node.Value.Accept(this);
            // Compile the object expression
            node.Object.Accept(this);
            var index = (LiteralExpression)node.Index;
            if (index.Token.Type == Tokens.ValueType.IntegerNumber)
            {
                _instructionBuilder.PushConstInt((int)index.Value);
                _instructionBuilder.SetProperty();
            }
            else if (index.Token.Type == Tokens.ValueType.String)
            {
                // Compile the value expression
                var strIndex = GetOrAddStringTable(index.Token.Value);
                _instructionBuilder.PushConstInt(strIndex);
                _instructionBuilder.SetProperty();
            }
            else
            {
                throw new Exception("无效的索引");
            }


        }



        public void VisitSetPropertyExpression(SetPropertyExpression node)
        {            
            // Compile the value expression
            node.Value.Accept(this);
            // Compile the object expression
            node.Object.Accept(this);
            // Compile the value expression
            var propery = (NameExpression)node.Property;
            var index = GetOrAddStringTable(propery.Identifier.Value);
            _instructionBuilder.PushConstInt(index);
            _instructionBuilder.SetProperty();

        }

        public void VisitGroupingExpression(GroupExpression node)
        {

        }

        public void VisitIfStatement(IfStatement node)
        {
            // Compile condition
            node.Condition.Accept(this);
            // Jump to else branch if condition is false
            var jumpToElse = _instructionBuilder.JumpFalse();
            // Pop the condition value (not needed anymore)
            _instructionBuilder.Pop();
            // Compile the then branch
            node.Body.Accept(this);
            // Jump over the else branch
            var jumpOverElse = _instructionBuilder.Jump();
            // Patch the jump to else
            _instructionBuilder.FixJump(jumpToElse);
            // Pop the condition value
            _instructionBuilder.Pop();
            // Compile the else branch if present
            if (node.Else != null)
            {
                node.Else.Accept(this);
            }
            // Patch the jump over else
            _instructionBuilder.FixJump(jumpOverElse);
        }

        public void VisitLambdaExpression(LambdaExpression node)
        {

        }

        public void VisitLiteralExpression(LiteralExpression node)
        {
            var token = node.Token;
            if (token.Type == Tokens.ValueType.Null)
            {
                _instructionBuilder.PushNull();
            }
            else if (token.Type == Tokens.ValueType.IntegerNumber)
            {
                _instructionBuilder.PushConstInt((Int32)node.Value);
            }
            else if (token.Type == Tokens.ValueType.DoubleNumber)
            {
                _instructionBuilder.PushConstDouble((Double)node.Value);
            }
            else if (token.Type == Tokens.ValueType.String)
            {
                var index = GetOrAddStringTable((String)node.Value);
                _instructionBuilder.PushString(index);
            }
            else if (token.Type == Tokens.ValueType.Boolean)
            {
                _instructionBuilder.PushBoolean((Boolean)node.Value);
            }
        }


        public void VisitName(NameExpression node)
        {
            if (_scope.TryGetVariablee(node.Identifier.Value, out int slot))
            {
                _instructionBuilder.LoadLocal(slot);
                return;
            }
            throw new Exception("无效的变量");
        }



        public void VisitMapExpression(MapExpression node)
        {
            // Create a new map with the specified initial capacity
            //EmitInstruction(OpCode.NEW_MAP, node.Entries.Count);
            _instructionBuilder.NewMap(node.ChildNodes.Count);
            // Populate the map with each entry
            foreach (var entry in node.ChildNodes)
            {
                // Duplicate the map reference
                _instructionBuilder.Duplicate() ;

                // Push the key
                //int keyIndex = GetOrAddConstant(entry.Key);
                //EmitInstruction(OpCode.PUSH_CONST, keyIndex);

                if (entry is MapKeyValueExpression property)
                {
                    property.Value.Accept(this);
                    // MAP 和 数组 无法在 GetElement/SetElement GetProperty/SetProperty中区分。 需要到运行时虚拟机中鉴定
                    // 所以   GetElement=GetProperty  SetElement=SetProperty
                    //property.Key
                }


                // Push the value
                entry.Accept(this);
                
                // Set the entry
                //EmitInstruction(OpCode.SET_PROPERTY, keyIndex);
                _instructionBuilder.SetProperty();
                // Pop the result of SET_PROPERTY (the value)
                _instructionBuilder.Pop();
            }
        }


        public void VisitReturnStatement(ReturnStatement node)
        {
            if (node.Expression != null)
            {
                node.Expression.Accept(this);
            }
            else
            {
                _instructionBuilder.PushNull();
            }
            _instructionBuilder.Return();
        }



        public void VisitBinaryExpression(BinaryExpression node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            // Emit the appropriate instruction based on the operator

            if (node.Operator.Symbol == Symbols.OP_PLUS)
            {
                _instructionBuilder.Emit(OpCode.ADD);
            }
            else if (node.Operator.Symbol == Symbols.OP_MINUS)
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
