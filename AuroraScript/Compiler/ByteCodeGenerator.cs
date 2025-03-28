using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Compiler.Emits;
using AuroraScript.Core;
using AuroraScript.Tokens;
using System.Reflection;
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
        /// <summary>
        /// 记录每个模块的地址
        /// 记录每个模块下方法地址
        /// 记录每个模块下导出方法
        /// 记录每个模块下导出变量
        /// </summary>
        /// <param name="node"></param>
        public void VisitModule(ModuleDeclaration node)
        {
            foreach (var module in node.Imports)
            {
                module.Accept(this);
            }
            
            _instructionBuilder.Comment("# module: " + node.ModuleName);
            VisitBlock(node);
        }


        public void VisitImportDeclaration(ImportDeclaration node)
        {

            Console.WriteLine(node);
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



        public void DumpCode()
        {

            Print("# str_table", ConsoleColor.Yellow);
            for (int i = 0; i < _stringTable.Count; i++)
            {
                var str = _stringTable[i];
                Print($"[{i:0000}] {str.Replace("\n", "\\n").Replace("\r", "")}", ConsoleColor.Blue);
            }
            Console.WriteLine();
            Console.WriteLine();
            Print("# func_index", ConsoleColor.Yellow);
            foreach (var item in _functionLocations)
            {
                Print($"{item.Key} {item.Value.Offset}", ConsoleColor.Blue);
            }
            Console.WriteLine();
            Console.WriteLine();
            _instructionBuilder.DumpCode();

        }

        public Byte [] Build()
        {
          return  _instructionBuilder.Build();
        }




        private void Print(String text, ConsoleColor fontColor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
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
                //_instructionBuilder.Pop();
            }

        }




        public void VisitAssignmentExpression(AssignmentExpression node)
        {
            _instructionBuilder.Comment($"# {node.ToString()}");
            // Compile the value expression
            node.Right.Accept(this);
            // Duplicate the value on the stack (for the assignment expression's own value)
            //_instructionBuilder.Duplicate();
            // Store the value in the variable
            if (_scope.Resolve(DeclareType.Variable, node.Left.ToString(), out var slot))
            {
                _instructionBuilder.PopLocal(slot);
            }
            else
            {
                _instructionBuilder.PopGlobal(node.Left.ToString());
            }
        }




        public void VisitBreakExpression(BreakStatement node)
        {
            _instructionBuilder.Comment($"# break;");
            _breakJumps.Peek().Add(_instructionBuilder.Jump());
        }

        public void VisitContinueExpression(ContinueStatement node)
        {
            _instructionBuilder.Comment($"# continue;");
            _continueJumps.Peek().Add(_instructionBuilder.Jump());
        }

        public void VisitCallExpression(FunctionCallExpression node)
        {
            if (node.IsStateSegment) _instructionBuilder.Comment($"# {node}");

            // Compile each argument
            foreach (var arg in node.Arguments)
            {
                arg.Accept(this);
            }

            // Compile the callee
            node.Target.Accept(this);
            // Emit the call instruction with the argument count
            _instructionBuilder.Call(node.Arguments.Count);
            if (node.Parent == null)
                _instructionBuilder.Pop();
        }


        public void VisitDeconstructionExpression(DeconstructionExpression node)
        {

        }



        public void VisitParameterDeclaration(ParameterDeclaration node)
        {
            var slot = _scope.Declare(DeclareType.Variable, node.Name.Value);
            if (node.DefaultValue != null)
            {
                node.DefaultValue?.Accept(this);
                _instructionBuilder.LoadArgIsExist(node.Index);
            }
            else
            {
                // load args
                _instructionBuilder.LoadArg(node.Index);
            }
            _instructionBuilder.PopLocal(slot);
        }



        public void VisitFunction(FunctionDeclaration node)
        {
            _instructionBuilder.Comment($"# begin_func {node.Identifier?.Value}", 4);
            BeginScope();

            // define arg var
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
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
            _instructionBuilder.Comment($"# end_func {node.Identifier?.Value}");
        }
        public void VisitGetElementExpression(GetElementExpression node)
        {
            node.Object.Accept(this);
            if (node.Index is NameExpression name)
            {
                // get property
                VisitName(name);
                _instructionBuilder.GetElement();
            }
            else if (node.Index is LiteralExpression literal)
            {
                VisitLiteralExpression(literal);
                _instructionBuilder.GetElement();
            }
            else
            {
                node.Index.Accept(this);
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
            _instructionBuilder.Comment($"# {node}");
            // Compile the value expression
            node.Value.Accept(this);
            if (node.Index is NameExpression name)
            {
                // get property
                VisitName(name);
                node.Object.Accept(this);
                _instructionBuilder.SetElement();
            }
            else if (node.Index is LiteralExpression literal)
            {
                VisitLiteralExpression(literal);
                node.Object.Accept(this);
                _instructionBuilder.GetElement();
            }
            else
            {
                node.Index.Accept(this);
            }
        }



        public void VisitSetPropertyExpression(SetPropertyExpression node)
        {
            _instructionBuilder.Comment($"# {node}");
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
            // Compile the then branch
            node.Body.Accept(this);
            // Patch the jump to else
            _instructionBuilder.FixJumpToHere(jumpToElse);
            // Compile the else branch if present
            if (node.Else != null)
            {
                // Jump over the else branch
                var jumpOverElse = _instructionBuilder.Jump();
                node.Else.Accept(this);
                // Patch the jump over else
                _instructionBuilder.FixJumpToHere(jumpOverElse);
            }

        }

        public void VisitLambdaExpression(LambdaExpression node)
        {
            //  
            this.VisitFunction(node.Function);


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
                _instructionBuilder.PushLocal(slot);
                return;
            }
            else
            {
                _instructionBuilder.PushGlobal(node.Identifier.Value);
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
                //_instructionBuilder.Pop();
            }
        }


        public void VisitReturnStatement(ReturnStatement node)
        {
            _instructionBuilder.Comment($"# {node}");
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
            else if (node.Operator.Symbol == Symbols.OP_LEFTSHIFT)
            {
                _instructionBuilder.Emit(OpCode.BIT_SHIFT_L);
            }
            else if (node.Operator.Symbol == Symbols.OP_SIGNEDRIGHTSHIFT)
            {
                _instructionBuilder.Emit(OpCode.BIT_SHIFT_R);
            }
            else if (node.Operator.Symbol == Symbols.OP_LOGICAL_AND)
            {
                _instructionBuilder.Emit(OpCode.LOGIC_AND);
            }
            else if (node.Operator.Symbol == Symbols.OP_LOGICAL_OR)
            {
                _instructionBuilder.Emit(OpCode.LOGIC_OR);
            }
            else if (node.Operator.Symbol == Symbols.OP_BIT_AND)
            {
                _instructionBuilder.Emit(OpCode.LOGIC_AND);
            }
            else if (node.Operator.Symbol == Symbols.OP_BIT_OR)
            {
                _instructionBuilder.Emit(OpCode.BIT_OR);
            }
            else if (node.Operator.Symbol == Symbols.OP_BIT_XOR)
            {
                _instructionBuilder.Emit(OpCode.BIT_XOR);
            }
            else if (node.Operator.Symbol == Symbols.OP_BIT_NOT)
            {
                _instructionBuilder.Emit(OpCode.BIT_NOT);
            }
            else if (node.Operator.Symbol == Symbols.OP_MODULO)
            {
                _instructionBuilder.Emit(OpCode.LOGIC_MOD);
            }

            
            else
            {
                throw new NotSupportedException($"Unsupported binary operator: {node.Operator.Symbol}");
            }
        }


        public void VisitUnaryExpression(UnaryExpression node)
        {
            if (node.IsStateSegment) _instructionBuilder.Comment($"# {node.ToString()}");
            OpCode opCode = OpCode.NOP;
            if (node.Operator == Operator.PostIncrement || node.Operator == Operator.PreIncrement)
            {
                opCode = OpCode.INCREMENT;
            }
            else if (node.Operator == Operator.PreDecrement || node.Operator == Operator.PostDecrement)
            {
                opCode = OpCode.DECREMENT;
            }
            else if (node.Operator == Operator.LogicalNot)
            {
                opCode = OpCode.LOGIC_NOT;
            }
            else if (node.Operator == Operator.BitwiseNot)
            {
                opCode = OpCode.BIT_NOT;
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
                    if (node.Parent != null) _instructionBuilder.Duplicate();
                    if (_scope.Resolve(DeclareType.Variable, nameExpression.Identifier.Value, out var slot))
                    {
                        _instructionBuilder.PopLocal(slot);
                    }
                    else
                    {
                        _instructionBuilder.PopGlobal(nameExpression.Identifier.Value);
                    }
                }
                else
                {
                   // throw new Exception($"无效的表达式:{exp}");
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
                    if (node.Parent != null) _instructionBuilder.Duplicate();
                    _instructionBuilder.Emit(opCode);
                    if (_scope.Resolve(DeclareType.Variable, nameExpression.Identifier.Value, out var slot))
                    {
                        _instructionBuilder.PopLocal(slot);
                    }
                    else
                    {
                        _instructionBuilder.PopGlobal(nameExpression.Identifier.Value);
                    }
                }
                else
                {
                 //   throw new Exception($"无效的表达式:{exp}");
                }

            }
        }







        public void VisitVarDeclaration(VariableDeclaration node)
        {
            _instructionBuilder.Comment($"# {node}");
            if (node.Initializer != null)
            {
                node.Initializer.Accept(this);
            }
            else
            {
                _instructionBuilder.PushNull();
            }
            // Local variable
            // _instructionBuilder.Duplicate();
            var slot = _scope.Declare(DeclareType.Variable, node.Name.Value);
            _instructionBuilder.PopLocal(slot);
        }


        public void VisitForStatement(ForStatement node)
        {
            _instructionBuilder.Comment($"# for");
            node.Initializer?.Accept(this);
            var begin = _instructionBuilder.Position();
            _breakJumps.Push(new List<Instruction>());
            _continueJumps.Push(new List<Instruction>());
            _instructionBuilder.Comment($"# condition {node.Condition}");
            node.Condition?.Accept(this);
            // Jump out of loop if condition is false
            var exitJump = _instructionBuilder.JumpFalse();
            _instructionBuilder.Comment($"# body");
            // Compile the loop body
            node.Body.Accept(this);
            foreach (var continueJump in _continueJumps.Peek())
            {
                _instructionBuilder.FixJumpToHere(continueJump);
            }
            var end = _instructionBuilder.Position();
            _instructionBuilder.Comment($"# inc {node.Incrementor}");
            node.Incrementor?.Accept(this);
            _instructionBuilder.JumpTo(begin);
            _instructionBuilder.FixJumpToHere(exitJump);
            foreach (var breakJump in _breakJumps.Peek())
            {
                _instructionBuilder.FixJumpToHere(breakJump);
            }
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
            // Compile the loop body
            node.Body.Accept(this);

            // Patch continue jumps to point to the condition check
            foreach (var continueJump in _continueJumps.Peek())
            {
                _instructionBuilder.FixJump(continueJump, begin);
            }
            _instructionBuilder.JumpTo(begin);
            _instructionBuilder.FixJumpToHere(exitJump);
            // Patch continue jumps to point to the condition check
            foreach (var breakJump in _breakJumps.Peek())
            {
                _instructionBuilder.FixJumpToHere(breakJump);
            }

        }

        public void VisitCompoundExpression(CompoundExpression node)
        {

        }
    }
}
