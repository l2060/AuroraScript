using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Compiler.Emits;
using AuroraScript.Core;
using AuroraScript.Tokens;
using System.Text;


namespace AuroraScript.Compiler
{
    public class ByteCodeGenerator : IAstVisitor
    {
        private CodeScope _scope = new CodeScope(null);
        private Stack<List<Instruction>> _breakJumps = new Stack<List<Instruction>>();
        private Stack<List<Instruction>> _continueJumps = new Stack<List<Instruction>>();
        private readonly Dictionary<string, Instruction> _functionLocations = new Dictionary<string, Instruction>();
        private readonly List<string> _stringTable = new List<string>();
        private readonly InstructionBuilder _instructionBuilder;

        public ByteCodeGenerator()
        {
            _instructionBuilder = new InstructionBuilder(_stringTable);
        }


        private void BeginScope()
        {
            _scope = _scope.Promotion();
        }

        private void EndScope()
        {
            _scope = _scope.Demotion();
        }

        public void VisitProgram(ModuleDeclaration node)
        {
            VisitBlock(node);
            var asm = Dump();
            // Convert instructions to bytecode
            var bytes = _instructionBuilder.Build();
            //return bytecode.ToArray();
            Console.WriteLine(asm);

        }



        public void VisitBlock(BlockStatement node)
        {
            // 方法指针应该是带执行上下文的。
            // 方法引用外层scope内的变量 ， 外层SCOPE内变量引用了方法。 互修复
            BeginScope();
            // 1. Compile function declare ..
            foreach (var function in node.Functions)
            {
                _scope.Declare(DeclareType.Variable, function.Identifier.Value);
            }
            // 2. Compile variables or init code. 
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }
            if (node is ModuleDeclaration)
            {
                _instructionBuilder.PushNull();
                _instructionBuilder.Return();
            }
            // 3. compile each function
            foreach (var function in node.Functions)
            {
                _functionLocations[function.Identifier.Value] = _instructionBuilder.Position();
                function.Accept(this);
            }
            EndScope();
        }



        public String Dump()
        {
            using (var stream = new MemoryStream())
            {
                using (var write = new StreamWriter(stream, Encoding.UTF8))
                {
                    write.WriteLine(".str_table");
                    for (int i = 0; i < _stringTable.Count; i++)
                    {
                        var str = _stringTable[i];
                        write.WriteLine($"[{i:0000}] {str.Replace("\n", "\\n").Replace("\r", "")}");
                    }
                    write.WriteLine();
                    write.WriteLine();
                    write.WriteLine();






                    write.WriteLine(".func_index");

                    foreach (var item in _functionLocations)
                    {
                        write.WriteLine($"{item.Key} {item.Value.Offset}");
                    }


                    _instructionBuilder.Dump(write);
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
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
                _instructionBuilder.PushConstantNumber(i);
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
            //_instructionBuilder.Duplicate();
            // Store the value in the variable
            if (_scope.Resolve(DeclareType.Variable, node.Left.ToString(), out var slot))
            {
                _instructionBuilder.StoreLocal(slot);
            }
            else
            {
                _instructionBuilder.StoreGlobal(node.Left.ToString());
            }
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
                _instructionBuilder.PushConstantString(index.Token.Value);
                _instructionBuilder.GetProperty();
            }
            else if (index.Token.Type == Tokens.ValueType.Number)
            {
                // get element
                _instructionBuilder.PushConstantNumber((Double)index.Value);
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
            _instructionBuilder.PushConstantString(propery.Identifier.Value);
            _instructionBuilder.GetProperty();
        }



        public void VisitSetElementExpression(SetElementExpression node)
        {
            // Compile the value expression
            node.Value.Accept(this);

            var index = (LiteralExpression)node.Index;
            if (index.Token.Type == Tokens.ValueType.Number)
            {
                _instructionBuilder.PushConstantNumber((Double)index.Value);
                // Compile the object expression
                node.Object.Accept(this);
                _instructionBuilder.SetElement();
            }
            else if (index.Token.Type == Tokens.ValueType.String)
            {
                // Compile the value expression
                _instructionBuilder.PushConstantString(index.Token.Value);
                // Compile the object expression
                node.Object.Accept(this);
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
            // Compile the value expression
            var propery = (NameExpression)node.Property;
            _instructionBuilder.PushConstantString(propery.Identifier.Value);
            // Compile the object expression
            node.Object.Accept(this);
            _instructionBuilder.SetProperty();

        }

        public void VisitGroupingExpression(GroupExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }

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
            else if (token.Type == Tokens.ValueType.Number)
            {
                _instructionBuilder.PushConstantNumber((Double)node.Value);
            }
            else if (token.Type == Tokens.ValueType.String)
            {
                _instructionBuilder.PushConstantString((String)node.Value);
            }
            else if (token.Type == Tokens.ValueType.Boolean)
            {
                _instructionBuilder.PushConstantBoolean((Boolean)node.Value);
            }
        }


        public void VisitName(NameExpression node)
        {
            if (_scope.Resolve(DeclareType.Variable, node.Identifier.Value, out int slot))
            {
                _instructionBuilder.LoadLocal(slot);
                return;
            }
            else
            {
                _instructionBuilder.LoadGlobal(node.Identifier.Value);
            }
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
                _instructionBuilder.Duplicate();
                if (entry is MapKeyValueExpression property)
                {
                    // Push the value
                    property.Value.Accept(this);
                    // Push the key
                    if (property.Key is IdentifierToken identifierToken)
                    {
                        _instructionBuilder.PushConstantString(identifierToken.Value);
                    }
                    else if (property.Key is NumberToken numberToken)
                    {
                        _instructionBuilder.PushConstantNumber(numberToken.NumberValue);
                    }
                    else if (property.Key is BooleanToken boolToken)
                    {
                        _instructionBuilder.PushConstantBoolean(boolToken.BoolValue);
                    }
                    else if (property.Key is NullToken nullToken)
                    {
                        _instructionBuilder.PushConstantString("null");
                    }
                    else
                    {
                        throw new Exception("无效的Map 键");
                    }
                    //property
                    _instructionBuilder.SetProperty();
                }
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
            OpCode opCode = OpCode.NOP;
            if (node.Operator == Operator.PostIncrement || node.Operator == Operator.PreIncrement)
            {
                opCode = OpCode.INCREMENT;
            }
            else if (node.Operator == Operator.PreDecrement || node.Operator == Operator.PostDecrement)
            {
                opCode = OpCode.DECREMENT;
            }
            else
            {
                throw new Exception($"无效的操作符:{node.Operator}");
            }
            var exp = node.ChildNodes[0];
            if (node.Type == UnaryType.Prefix)
            {
                exp.Accept(this);
                _instructionBuilder.Emit(opCode);
                if (exp is LiteralExpression literal)
                {
                }
                else if (exp is NameExpression nameExpression)
                {
                    _instructionBuilder.Duplicate();
                    if (_scope.Resolve(DeclareType.Variable, nameExpression.Identifier.Value, out var slot))
                    {
                        _instructionBuilder.StoreLocal(slot);
                    }
                    else
                    {
                        _instructionBuilder.StoreGlobal(nameExpression.Identifier.Value);
                    }
                }
                else
                {
                    throw new Exception($"无效的表达式:{exp}");
                }
            }
            else
            {
                exp.Accept(this);
                // 后缀时 常量不处理
                if (exp is LiteralExpression literal)
                {
                }
                else if (exp is NameExpression nameExpression)
                {
                    _instructionBuilder.Duplicate();
                    _instructionBuilder.Emit(opCode);
                    if (_scope.Resolve(DeclareType.Variable, nameExpression.Identifier.Value, out var slot))
                    {
                        _instructionBuilder.StoreLocal(slot);
                    }
                    else
                    {
                        _instructionBuilder.StoreGlobal(nameExpression.Identifier.Value);
                    }
                }
                else
                {
                    throw new Exception($"无效的表达式:{exp}");
                }

            }
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
               // _instructionBuilder.Duplicate();
                var slot = _scope.Declare(DeclareType.Variable, item.Value);
                _instructionBuilder.StoreLocal(slot);
            }
            // 暂时先这么写 前面 Duplicate 冗余了一个
           // _instructionBuilder.Pop();
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
