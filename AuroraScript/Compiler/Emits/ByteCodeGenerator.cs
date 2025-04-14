using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Core;
using AuroraScript.Tokens;



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

        public ByteCodeGenerator()
        {
            _stringSet = new StringList();
            _scope = new CodeScope(null, DomainType.Global, _stringSet);
            _instructionBuilder = new InstructionBuilder(_stringSet);

        }


        private void BeginScope(DomainType domain)
        {
            _scope = _scope.Enter(_stringSet, domain);
        }

        private void EndScope()
        {
            _scope = _scope.Leave();
        }


        public override void VisitImportDeclaration(ImportDeclaration node)
        {

        }

        /// <summary>
        /// 记录每个模块的地址
        /// 记录每个模块下方法地址
        /// 记录每个模块下导出方法
        /// 记录每个模块下导出变量
        ///
        /// 模块注册到 global  @module1
        /// 方法和模块变量注册到模块属性
        ///
        /// 访问global 全部以getproperty 和 setproperty
        /// 访问module内的变量 全部以getproperty 和 setproperty
        /// </summary>
        /// <param name="node"></param>
        public override void VisitModule(ModuleDeclaration node)
        {
            BeginScope(DomainType.Module);
            _instructionBuilder.Comment("# module: " + node.ModuleName);
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
            VisitBlock(node);

            _instructionBuilder.PushNull();
            _instructionBuilder.Return();

            // 3. compile each function
            //foreach (var function in node.Functions)
            //{
            //    _functionLocations[function.Name.Value] = _instructionBuilder.Position();
            //    function.Accept(this);
            //}
            EndScope();
        }



        public override void VisitBlock(BlockStatement node)
        {
            if (node.IsNewScope) BeginScope(DomainType.Code);
            // 1. 声明代码块内方法
            var closureMap = new Dictionary<string, ClosureInstruction>();
            foreach (var function in node.Functions)
            {
                var slot = _scope.Declare((node is ModuleDeclaration) ? DeclareType.Property : DeclareType.Variable, function);
                var closure = _instructionBuilder.NewClosure();
                closureMap[function.Name.UniqueValue] = closure;
                if (node is ModuleDeclaration)
                {
                    _instructionBuilder.SetThisProperty(function.Name.Value);
                }
                else
                {
                    _instructionBuilder.PopLocal(slot);
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
                VisitFunction(function);
            }


            if (node.IsNewScope) EndScope();
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

            // 处理捕获的变量
            Dictionary<string, int> captureIndexMap = new Dictionary<string, int>();
            int captureIndex = 0;

            foreach (var varName in capturedVariables)
            {
                if (_scope.Resolve(varName, out var declareObject))
                {
                    // 将变量值压入栈
                    if (declareObject.Type == DeclareType.Variable)
                    {
                        _instructionBuilder.PushLocal(declareObject.Index);
                    }
                    else if (declareObject.Type == DeclareType.Property)
                    {
                        _instructionBuilder.GetThisProperty(declareObject.Index);
                    }

                    // 捕获变量
                    _instructionBuilder.CaptureVariable(captureIndex);
                    captureIndexMap[varName] = captureIndex++;
                }
            }

            BeginScope(DomainType.Function);
            var begin = _instructionBuilder.Position();

            // 为捕获的变量创建局部变量
            foreach (var entry in captureIndexMap)
            {
                var slot = _scope.Declare(DeclareType.Variable, entry.Key);
                _instructionBuilder.LoadCapturedVariable(entry.Value);
                _instructionBuilder.PopLocal(slot);
            }

            // 定义参数变量
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }

            // 编译函数体
            node.Body?.Accept(this);

            // 添加默认返回值
            _instructionBuilder.PushNull();
            _instructionBuilder.Return();

            EndScope();
            _instructionBuilder.Comment($"# end_func {node.Name?.Value}");
        }


        public override void VisitLambdaExpression(LambdaExpression node)
        {
            // 使用 VariableCatcher 分析 Lambda 函数中的变量使用情况
            var variableCatcher = new VariableCatcher();

            // 分析 Lambda 函数中捕获的变量
            var capturedVariables = variableCatcher.AnalyzeFunction(node.Function, _scope);

            // 输出调试信息
            _instructionBuilder.Comment($"# Lambda captured variables: {string.Join(", ", capturedVariables)}");

            // 处理捕获的变量
            int captureIndex = 0;
            foreach (var varName in capturedVariables)
            {
                if (_scope.Resolve(varName, out var declareObject))
                {
                    // 将变量值压入栈
                    if (declareObject.Type == DeclareType.Variable)
                    {
                        _instructionBuilder.PushLocal(declareObject.Index);
                    }
                    else if (declareObject.Type == DeclareType.Property)
                    {
                        _instructionBuilder.GetThisProperty(declareObject.Index);
                    }

                    // 捕获变量
                    _instructionBuilder.CaptureVariable(captureIndex++);
                }
            }

            // 创建闭包
            var closure = _instructionBuilder.NewClosure();
            var toEndJump = _instructionBuilder.Jump();
            _instructionBuilder.FixClosure(closure);

            // 编译函数体
            VisitFunction(node.Function);

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
            _instructionBuilder.PopLocal(slot);
        }











        public override void VisitGetElementExpression(GetElementExpression node)
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
                if (declare.Type == DeclareType.Variable)
                {
                    _instructionBuilder.PushLocal(declare.Index);
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
            }
            else
            {
                _instructionBuilder.PushNull();
            }
            _instructionBuilder.Return();
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
                _instructionBuilder.Emit(OpCode.LOGIC_MOD);
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
                
                // 如果是变量，需要保存回变量
                if (operand is NameExpression nameExpr)
                {
                    if (node.Parent != null) _instructionBuilder.Duplicate();
                    MoveValueTo(nameExpr.Identifier.Value);
                }
                else if (operand is GetPropertyExpression propExpr)
                {
                    if (node.Parent != null) _instructionBuilder.Duplicate();
                    // 处理属性赋值
                    HandlePropertyAssignment(propExpr);
                }
            }
            else // PostFix
            {
                // 后缀操作：先加载值，复制一份，执行操作后保存，返回原值
                operand.Accept(this);
                _instructionBuilder.Duplicate(); // 复制一份用于操作
                _instructionBuilder.Emit(opCode);
                
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
            }
        }

        private OpCode DetermineUnaryOpCode(Operator op)
        {
            if (Operator.PreIncrement == op || Operator.PostIncrement == op) return OpCode.INCREMENT;
            if (Operator.PreDecrement == op || Operator.PostDecrement == op) return OpCode.DECREMENT;
            if (Operator.LogicalNot == op ) return OpCode.LOGIC_NOT;
            if (Operator.BitwiseNot == op ) return OpCode.BIT_NOT;
            if (Operator.Negate == op) return OpCode.NEGATE;
            throw new Exception($"Invalid operator: {op}");
        }

        private void HandlePropertyAssignment(GetPropertyExpression propExpr)
        {
            // 创建一个SetPropertyExpression并访问它
            var setter = new SetPropertyExpression();
            setter.AddNode(propExpr.Object);
            setter.AddNode(propExpr.Property);
            setter.Accept(this);
        }







        public override void VisitVarDeclaration(VariableDeclaration node)
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
            if (_scope.Domain == DomainType.Module)
            {
                _scope.Declare(DeclareType.Property, node);
                _instructionBuilder.SetThisProperty(node.Name.Value);
            }
            else
            {
                var slot = _scope.Declare(DeclareType.Variable, node);
                _instructionBuilder.PopLocal(slot);
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
                if (declareObject.Type == DeclareType.Variable)
                {
                    _instructionBuilder.PopLocal(declareObject.Index);
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
