using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Core;


namespace AuroraScript.Compiler.Emits
{
    public class ByteCodeGenerator : IAstVisitor
    {

        private Stack<List<JumpInstruction>> _breakJumps = new Stack<List<JumpInstruction>>();
        private Stack<List<JumpInstruction>> _continueJumps = new Stack<List<JumpInstruction>>();
        private readonly Dictionary<string, Instruction> _functionLocations = new Dictionary<string, Instruction>();
        private readonly InstructionBuilder _instructionBuilder;
        public readonly StringList _stringSet;
        private CodeScope _scope;

        public ByteCodeGenerator(InstructionBuilder instructionBuilder, StringList stringSet)
        {
            _stringSet = stringSet;
            _scope = new CodeScope(null, DomainType.Global, _stringSet);
            _instructionBuilder = instructionBuilder;

        }


        private void BeginScope(DomainType domain)
        {
            _scope = _scope.Enter(_stringSet, domain);
        }

        private void EndScope()
        {
            _scope = _scope.Leave();
        }

        public void Visit(ModuleSyntaxRef[] syntaxRefs)
        {
            _instructionBuilder.Emit(OpCode.NOP);
            _instructionBuilder.Comment("Create Domain:");
            // create module init closure
            // 创建Global属性
            var moduleClosures = new Dictionary<string, ClosureInstruction>();
            foreach (ModuleSyntaxRef syntaxRef in syntaxRefs)
            {
                _instructionBuilder.NewModule(syntaxRef.SyntaxTree.ModuleName);
                _instructionBuilder.SetGlobalProperty($"@{syntaxRef.SyntaxTree.ModuleName}");
            }

            // 创建模块闭包，并调用闭包初始化
            foreach (ModuleSyntaxRef syntaxRef in syntaxRefs)
            {
                _instructionBuilder.GetGlobalProperty($"@{syntaxRef.SyntaxTree.ModuleName}");
                var closureIns = _instructionBuilder.NewClosure();
                _instructionBuilder.Call(0);
                _instructionBuilder.Pop();
                moduleClosures.Add(syntaxRef.SyntaxTree.ModuleName, closureIns);
            }
            // 返回初始化完毕
            _instructionBuilder.ReturnGlobal();

            foreach (var item in syntaxRefs)
            {
                var pos = _instructionBuilder.Position;
                if (moduleClosures.TryGetValue(item.SyntaxTree.ModuleName, out var instruction))
                {
                    // fix module closure addreess
                    _instructionBuilder.FixClosure(instruction);
                }
                item.SyntaxTree.Accept(this);
            }
        }



        /// <summary>
        /// 模块注册到 global  @module1
        /// 方法和模块变量注册到模块属性
        ///
        /// 访问global 全部以getproperty 和 setproperty
        /// 访问module内的变量 全部以getproperty 和 setproperty
        /// </summary>
        /// <param name="node"></param>
        public override void VisitModule(ModuleDeclaration node)
        {
            _instructionBuilder.DefineModule(node);
            BeginScope(DomainType.Module);
            _instructionBuilder.Comment($"module: {node.ModuleName} {node.ModulePath}");
            _instructionBuilder.Comment("# Module Import");
            foreach (var module in node.Imports)
            {
                module.Accept(this);
            }
            _instructionBuilder.Comment("# Variable Define");
            foreach (var module in node.Members)
            {
                _instructionBuilder.PushNull();// push this
                _instructionBuilder.SetThisProperty(module.Name.Value);
            }

            _instructionBuilder.Comment("# Code Block");
            // 1. 声明代码块内方法
            var closureMap = new Dictionary<string, ClosureInstruction>();
            foreach (var function in node.Functions)
            {
                var slot = _scope.Declare(DeclareType.Property, function);
                _instructionBuilder.PushThis();
                closureMap[function.Name.UniqueValue] = _instructionBuilder.NewClosure();
                _instructionBuilder.SetThisProperty(function.Name.Value);
            }

            // 2. 代码块内的语句
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }

            _instructionBuilder.PushConstantBoolean(true);
            _instructionBuilder.Return();

            // 3. 代码块内的方法体
            foreach (var function in node.Functions)
            {
                var closure = closureMap[function.Name.UniqueValue];
                _instructionBuilder.FixClosure(closure);
                function.Accept(this);
            }

            EndScope();
        }





        public override void VisitImportDeclaration(ImportDeclaration node)
        {
            // 加载模块至属性
            _instructionBuilder.GetGlobalProperty("@" + node.ModuleName);
            // 将导入的模块声明为当前模块的变量
            var solt = _scope.Declare(DeclareType.Property, node);
            _instructionBuilder.SetThisProperty(solt);
        }

        public override void VisitBlock(BlockStatement node)
        {
            if (!node.IsFunction) BeginScope(DomainType.Code);
            // 1. 声明代码块内方法
            var closureMap = new Dictionary<string, ClosureInstruction>();
            foreach (var function in node.Functions)
            {
                var slot = _scope.Declare((node is ModuleDeclaration) ? DeclareType.Property : DeclareType.Variable, function);
                _instructionBuilder.PushThis();
                closureMap[function.Name.UniqueValue] = _instructionBuilder.NewClosure();
                if (node is ModuleDeclaration)
                {
                    _instructionBuilder.SetThisProperty(function.Name.Value);
                }
                else
                {
                    _instructionBuilder.StoreLocal(slot);
                }
            }
            // 2. 代码块内的语句
            foreach (var statement in node.ChildNodes)
            {
                statement.Accept(this);
            }
            // 3. 代码块内的方法体
            foreach (var function in node.Functions)
            {
                var closure = closureMap[function.Name.UniqueValue];
                _instructionBuilder.FixClosure(closure);
                function.Accept(this);
            }
            if (!node.IsFunction) EndScope();
        }

        public override void VisitFunction(FunctionDeclaration node)
        {
            if (node.Flags == FunctionFlags.Declare) return;
            _instructionBuilder.Comment($"# begin_func {node.Name?.Value}", 4);

            // 使用 VariableCatcher 分析函数中的变量使用情况
            var variableCatcher = new VariableCatcher();
            // 分析函数中捕获的变量
            var capturedVariables = variableCatcher.AnalyzeFunction(node, _scope);
            // 输出调试信息
            _instructionBuilder.Comment($"# Captured variables: {string.Join(", ", capturedVariables)}");

            BeginScope(DomainType.Function);
            var begin = _instructionBuilder.Position();

            // 定义参数变量
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }

            // 为捕获的变量创建局部变量
            foreach (var varName in capturedVariables)
            {
                if (_scope.Resolve(varName, out var declareObject))
                {
                    //将变量值压入栈
                    if (declareObject.Type == DeclareType.Variable)
                    {
                        var slot = _scope.Declare(DeclareType.Captured, varName);
                        _instructionBuilder.CaptureVariable(declareObject.Index);
                        _instructionBuilder.StoreLocal(slot);
                    }
                }
            }

            // 编译函数体
            node.Body?.Accept(this);

            // 添加默认返回值
            _instructionBuilder.ReturnNull();

            EndScope();
            _instructionBuilder.Comment($"# end_func {node.Name?.Value}");
        }


        public override void VisitLambdaExpression(LambdaExpression node)
        {
            // 创建闭包
            _instructionBuilder.PushThis();
            var closure = _instructionBuilder.NewClosure();
            var toEndJump = _instructionBuilder.Jump();
            _instructionBuilder.FixClosure(closure);
            // 编译函数体
            node.Function.Accept(this);
            // 修复跳转
            _instructionBuilder.FixJumpToHere(toEndJump);
        }



        public void DumpCode()
        {
            Print("# str_table", ConsoleColor.Yellow);
            var stringTable = _stringSet.List;
            for (int i = 0; i < stringTable.Length; i++)
            {
                var str = stringTable[i];
                Print($"[{i:0000}] {str.Replace("\n", "\\n").Replace("\r", "")} \t {str.GetHashCode()}", ConsoleColor.Blue);
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

        public byte[] Build()
        {
            return _instructionBuilder.Build();
        }




        private void Print(string text, ConsoleColor fontColor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = fontColor;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
        }





        public override void VisitArrayExpression(ArrayLiteralExpression node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                // Push the element value
                node.ChildNodes[i].Accept(this);
            }
            _instructionBuilder.NewArray(node.ChildNodes.Count);
        }




        public override void VisitAssignmentExpression(AssignmentExpression node)
        {
            _instructionBuilder.Comment($"# {node.ToString()}");
            // Compile the value expression
            node.Right.Accept(this);
            // Duplicate the value on the stack (for the assignment expression's own value)
            //_instructionBuilder.Duplicate();
            // Store the value in the variable
            MoveValueTo(node.Left.ToString());
        }





        public override void VisitBreakExpression(BreakStatement node)
        {
            _instructionBuilder.Comment($"# break;");
            _breakJumps.Peek().Add(_instructionBuilder.Jump());
        }

        public override void VisitContinueExpression(ContinueStatement node)
        {
            _instructionBuilder.Comment($"# continue;");
            _continueJumps.Peek().Add(_instructionBuilder.Jump());
        }

        public override void VisitCallExpression(FunctionCallExpression node)
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
            _instructionBuilder.Call((Byte)node.Arguments.Count);
            if (node.Parent == null)
                _instructionBuilder.Pop();
        }


        public override void VisitDeconstructionExpression(DeconstructionExpression node)
        {

        }



        public override void VisitParameterDeclaration(ParameterDeclaration node)
        {
            var slot = _scope.Declare(DeclareType.Variable, node);
            if (node.Initializer != null)
            {
                node.Initializer?.Accept(this);
                _instructionBuilder.LoadArgIsExist(node.Index);
            }
            else
            {
                // load args
                _instructionBuilder.LoadArg(node.Index);
            }
            _instructionBuilder.StoreLocal(slot);
        }











        public override void VisitGetElementExpression(GetElementExpression node)
        {
            node.Object.Accept(this);
            if (node.Index is NameExpression name)
            {
                // get property
                name.Accept(this);
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

        public override void VisitGetPropertyExpression(GetPropertyExpression node)
        {
            node.Object.Accept(this);
            var propery = (NameExpression)node.Property;
            _instructionBuilder.GetProperty(propery.Identifier.Value);
        }



        public override void VisitSetElementExpression(SetElementExpression node)
        {
            _instructionBuilder.Comment($"# {node}");
            // Compile the value expression
            node.Value.Accept(this);
            if (node.Index is NameExpression name)
            {
                // get property
                name.Accept(this);
                node.Object.Accept(this);
                _instructionBuilder.SetElement();
            }
            else if (node.Index is LiteralExpression literal)
            {
                VisitLiteralExpression(literal);
                node.Object.Accept(this);
                _instructionBuilder.SetElement();
            }
            else
            {
                node.Index.Accept(this);
            }
        }



        public override void VisitSetPropertyExpression(SetPropertyExpression node)
        {
            _instructionBuilder.Comment($"# {node}");

            // Compile the object expression
            node.Object.Accept(this);
            // Compile the value expression
            node.Value.Accept(this);
            // Compile the value expression
            var propery = (NameExpression)node.Property;
            _instructionBuilder.SetProperty(propery.Identifier.Value);

        }

        public override void VisitGroupingExpression(GroupExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }

        }

        public override void VisitIfStatement(IfStatement node)
        {
            // Compile condition
            node.Condition.Accept(this);
            // Jump to else branch if condition is false
            var jumpToElse = _instructionBuilder.JumpFalse();
            // Compile the then branch
            node.Body.Accept(this);
            // Compile the else branch if present
            if (node.Else != null)
            {
                // Jump over the else branch
                var jumpOverElse = _instructionBuilder.Jump();
                // Patch the jump to end
                _instructionBuilder.FixJumpToHere(jumpToElse);
                node.Else.Accept(this);
                // Patch the jump over else
                _instructionBuilder.FixJumpToHere(jumpOverElse);
            }
            else
            {
                // Patch the jump to end
                _instructionBuilder.FixJumpToHere(jumpToElse);
            }
        }


        public override void VisitLiteralExpression(LiteralExpression node)
        {
            var token = node.Token;
            if (token.Type == Tokens.ValueType.Null)
            {
                _instructionBuilder.PushNull();
            }
            else if (token.Type == Tokens.ValueType.Number)
            {
                _instructionBuilder.PushConstantNumber((double)node.Value);
            }
            else if (token.Type == Tokens.ValueType.String)
            {
                _instructionBuilder.PushConstantString((string)node.Value);
            }
            else if (token.Type == Tokens.ValueType.Boolean)
            {
                _instructionBuilder.PushConstantBoolean((bool)node.Value);
            }
        }


        public override void VisitName(NameExpression node)
        {
            if (node.Identifier.Value == "this")
            {
                _instructionBuilder.PushThis();
                return;
            }
            if (node.Identifier.Value == "global")
            {
                _instructionBuilder.PushGlobal();
                return;
            }
            if (_scope.Resolve(node.Identifier.Value, out var declare))
            {
                if (declare.Type == DeclareType.Captured)
                {
                    _instructionBuilder.LoadCapturedVariable(declare.Index);
                    return;
                }
                else if (declare.Type == DeclareType.Variable)
                {
                    _instructionBuilder.LoadLocal(declare.Index);
                }
                else if (declare.Type == DeclareType.Property)
                {
                    _instructionBuilder.GetThisProperty(declare.Index);
                }
            }
            else
            {
                _instructionBuilder.GetGlobalProperty(node.Identifier.Value);
            }
        }



        public override void VisitMapExpression(MapExpression node)
        {
            // Create a new map with the specified initial capacity
            //EmitInstruction(OpCode.NEW_MAP, node.Entries.Count);
            _instructionBuilder.NewMap();
            // Populate the map with each entry
            foreach (var entry in node.ChildNodes)
            {
                // Duplicate the map reference
                _instructionBuilder.Duplicate();
                if (entry is MapKeyValueExpression property)
                {
                    // Push the value
                    property.Value.Accept(this);
                    _instructionBuilder.SetProperty(property.Key.Value);
                }
                // Pop the result of SET_PROPERTY (the value)
                //_instructionBuilder.Pop();
            }
        }


        public override void VisitReturnStatement(ReturnStatement node)
        {
            _instructionBuilder.Comment($"# {node}");
            if (node.Expression != null)
            {
                node.Expression.Accept(this);
                _instructionBuilder.Return();
            }
            else
            {
                _instructionBuilder.ReturnNull();
            }

        }



        public override void VisitBinaryExpression(BinaryExpression node)
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
                _instructionBuilder.Emit(OpCode.MOD);
            }


            else
            {
                throw new NotSupportedException($"Unsupported binary operator: {node.Operator.Symbol}");
            }
        }


        public override void VisitUnaryExpression(UnaryExpression node)
        {
            if (node.IsStateSegment) _instructionBuilder.Comment($"# {node.ToString()}");

            // 获取操作数
            var operand = node.Operand;

            // 确定操作码
            OpCode opCode = DetermineUnaryOpCode(node.Operator);

            if (node.Type == UnaryType.Prefix)
            {
                // 前缀操作：先执行操作，再返回值
                operand.Accept(this);
                _instructionBuilder.Emit(opCode);
                if (node.Parent != null) _instructionBuilder.Duplicate();
                // 如果是变量，需要保存回变量
                if (operand is NameExpression nameExpr)
                {
                    MoveValueTo(nameExpr.Identifier.Value);
                }
                else if (operand is GetPropertyExpression propExpr)
                {
                    // 处理属性赋值
                    HandlePropertyAssignment(propExpr);
                }
                else if (operand is GetElementExpression eleExpr)
                {
                    // 处理索引赋值
                    HandleElementAssignment(eleExpr);
                }
            }
            else // PostFix
            {
                // 后缀操作：先加载值，复制一份，执行操作后保存，返回原值
                operand.Accept(this);

                _instructionBuilder.Emit(opCode);
                if (node.Parent != null) _instructionBuilder.Duplicate();
                // 如果是变量，需要保存回变量
                if (operand is NameExpression nameExpr)
                {
                    MoveValueTo(nameExpr.Identifier.Value);
                    // 不需要额外的Duplicate，因为栈顶已经是原值
                }
                else if (operand is GetPropertyExpression propExpr)
                {
                    // 处理属性赋值
                    HandlePropertyAssignment(propExpr);
                }
                else if (operand is GetElementExpression eleExpr)
                {
                    // 处理索引赋值
                    HandleElementAssignment(eleExpr);
                }
            }
        }

        private OpCode DetermineUnaryOpCode(Operator op)
        {
            if (Operator.PreIncrement == op || Operator.PostIncrement == op) return OpCode.INCREMENT;
            if (Operator.PreDecrement == op || Operator.PostDecrement == op) return OpCode.DECREMENT;
            if (Operator.LogicalNot == op) return OpCode.LOGIC_NOT;
            if (Operator.BitwiseNot == op) return OpCode.BIT_NOT;
            if (Operator.Negate == op) return OpCode.NEGATE;
            throw new Exception($"Invalid operator: {op}");
        }

        private void HandlePropertyAssignment(GetPropertyExpression propExpr)
        {
            propExpr.Object.Accept(this);
            _instructionBuilder.SetProperty(propExpr.Property.ToString());
        }


        private void HandleElementAssignment(GetElementExpression eleExpr)
        {
            eleExpr.Object.Accept(this);
            eleExpr.Index.Accept(this);
            _instructionBuilder.SetElement();
        }




        public override void VisitVarDeclaration(VariableDeclaration node)
        {
            Int32 slot = 0;
            if (_scope.Domain == DomainType.Module)
            {
                _scope.Declare(DeclareType.Property, node);
            }
            else
            {
                slot = _scope.Declare(DeclareType.Variable, node);
            }

            _instructionBuilder.Comment($"# {node}");
            if (node.Initializer != null)
            {
                node.Initializer.Accept(this);
            }
            else
            {
                _instructionBuilder.PushNull();
            }

            if (_scope.Domain == DomainType.Module)
            {
                _instructionBuilder.SetThisProperty(node.Name.Value);
            }
            else
            {
                _instructionBuilder.StoreLocal(slot);
            }

        }


        public override void VisitForStatement(ForStatement node)
        {
            _instructionBuilder.Comment($"# for");
            node.Initializer?.Accept(this);
            _breakJumps.Push(new List<JumpInstruction>());
            _continueJumps.Push(new List<JumpInstruction>());
            _instructionBuilder.Comment($"# condition {node.Condition}");
            var begin = _instructionBuilder.Position();
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


        public override void VisitWhileStatement(WhileStatement node)
        {
            var begin = _instructionBuilder.Position();
            // Set up break and continue targets
            _breakJumps.Push(new List<JumpInstruction>());
            _continueJumps.Push(new List<JumpInstruction>());
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

        public override void VisitCompoundExpression(CompoundExpression node)
        {
            // TODO
        }



        private void MoveValueTo(String property)
        {
            if (_scope.Resolve(property, out var declareObject))
            {
                if (declareObject.Type == DeclareType.Captured)
                {
                    _instructionBuilder.StoreLocal(declareObject.Index);
                    return;
                }
                else if (declareObject.Type == DeclareType.Variable)
                {
                    _instructionBuilder.StoreLocal(declareObject.Index);
                    return;
                }
                else if (declareObject.Type == DeclareType.Property)
                {
                    _instructionBuilder.SetThisProperty(property);
                    return;
                }

            }
            _instructionBuilder.SetGlobalProperty(property);
        }
    }
}
