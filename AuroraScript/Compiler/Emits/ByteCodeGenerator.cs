using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Compiler.Ast.Statements;
using AuroraScript.Compiler.Exceptions;
using AuroraScript.Core;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;



namespace AuroraScript.Compiler.Emits
{
    internal class ByteCodeGenerator : IAstVisitor
    {
        private Stack<DebugSymbol> _debugSymbolStacck = new Stack<DebugSymbol>();

        private Stack<List<JumpInstruction>> _breakJumps = new Stack<List<JumpInstruction>>();
        private Stack<List<JumpInstruction>> _continueJumps = new Stack<List<JumpInstruction>>();
        private readonly InstructionBuilder _instructionBuilder;
        public readonly StringList _stringSet;
        private CodeScope _scope;
        private readonly CodeScope _global;
        private DebugSymbol _symbol;

        public ByteCodeGenerator(InstructionBuilder instructionBuilder, StringList stringSet, DebugSymbolInfo debugSymbols)
        {
            _symbol = debugSymbols;
            _stringSet = stringSet;
            _global = _scope = new CodeScope(null, DomainType.Global, _stringSet);
            _instructionBuilder = instructionBuilder;

        }

        DebugSymbol _segmentSymbol = new DebugSymbolInfo();


        protected override void BeforeVisitNode(AstNode node)
        {
            if (node is ModuleDeclaration || node is FunctionDeclaration || node.LineNumber < 0) return;

            if (_segmentSymbol == null || node.LineNumber != _segmentSymbol.LineNumber)
            {
                var point = _instructionBuilder.Position().Offset;
                if (_segmentSymbol != null)
                {
                    _segmentSymbol.EndPoint = point - 1;
                    _segmentSymbol.LineNumber = node.LineNumber - 1;
                }
                _segmentSymbol = new StatementSymbol() { LineNumber = node.LineNumber, StartPoint = point };
                _symbol.Add(_segmentSymbol);
            }
        }




        private void BeginScope(DomainType domain)
        {
            DebugSymbol newSymbol = null;
            if (domain == DomainType.Module)
            {
                newSymbol = new ModuleSymbol();
            }
            else if (domain == DomainType.Function)
            {
                newSymbol = new FunctionSymbol();
            }
            // debug symbol
            if (newSymbol != null)
            {
                _symbol.Add(newSymbol);
                _debugSymbolStacck.Push(_symbol);
                _symbol = newSymbol;
                _symbol.StartPoint = _instructionBuilder.Position().Offset;
            }
            // scope
            _scope = _scope.Enter(_stringSet, domain);
        }

        private void EndScope()
        {
            if (_scope.Domain == DomainType.Function || _scope.Domain == DomainType.Module)
            {
                _symbol.EndPoint = _instructionBuilder.Position().Offset - 1;
                if (_segmentSymbol != null)
                {
                    _segmentSymbol.EndPoint = _symbol.EndPoint;
                    _segmentSymbol = null;
                }

                _symbol = _debugSymbolStacck.Pop();
            }

            _scope = _scope.Leave();
        }

        private void PushCapturedUpvalue(ClosureCaptured captured)
        {
            _instructionBuilder.CaptureVariable(captured.AliasSlot);
        }

        private ClosureCaptured[] EnsureCapturedVariables(FunctionDeclaration function)
        {
            if (function.CapturedVariables != null && function.CapturedVariables.Length > 0)
            {
                return function.CapturedVariables;
            }

            var catcher = new VariableCatcher();
            var capturedNames = catcher.AnalyzeFunction(function, _scope);
            if (capturedNames == null || capturedNames.Count == 0)
            {
                function.CapturedVariables = Array.Empty<ClosureCaptured>();
                return function.CapturedVariables;
            }

            var list = new List<ClosureCaptured>();
            foreach (var name in capturedNames)
            {
                if (_scope.Resolve(name, out var declareObject) && (declareObject.Type == DeclareType.Variable || declareObject.Type == DeclareType.Captured))
                {
                    var aliasSlot = declareObject.Type == DeclareType.Captured
                        ? declareObject.CaptureAlias
                        : declareObject.Index;
                    list.Add(new ClosureCaptured(name, declareObject.Index, aliasSlot, declareObject.Type));
                }
            }
            function.CapturedVariables = list.ToArray();
            return function.CapturedVariables;
        }

        private void PredeclareVariables(IEnumerable<AstNode> statements, DeclareType declareType)
        {
            foreach (var statement in statements)
            {
                if (statement is VariableDeclaration variable)
                {
                    _scope.Declare(declareType, variable);
                }
            }
        }


        public void Visit(ModuleSyntaxRef[] syntaxRefs)
        {
            var rootSymbol = _symbol;
            rootSymbol.StartPoint = _instructionBuilder.Position().Offset;
            _instructionBuilder.Emit(OpCode.NOP);
            _instructionBuilder.Comment("Create Domain:");
            // create module init closure
            // 创建Global属性
            var moduleClosures = new Dictionary<string, ClosureInstruction>();
            foreach (ModuleSyntaxRef syntaxRef in syntaxRefs)
            {
                _instructionBuilder.InitModule(syntaxRef.SyntaxTree.ModuleName);
            }

            // 创建模块闭包，并调用闭包初始化
            foreach (ModuleSyntaxRef syntaxRef in syntaxRefs)
            {
                _instructionBuilder.GetGlobalProperty($"@{syntaxRef.SyntaxTree.ModuleName}");
                var closureIns = _instructionBuilder.NewClosure(0);
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
            rootSymbol.EndPoint = _instructionBuilder.Position().Offset - 1;
            //this.DumpCode();
            //Console.WriteLine(_symbol);
        }



        /// <summary>
        /// 模块注册到 global  @module1
        /// 方法和模块变量注册到模块属性
        ///
        /// 访问global 全部以getproperty 和 setproperty
        /// 访问module内的变量 全部以getproperty 和 setproperty
        /// </summary>
        /// <param name="node"></param>
        protected override void VisitModule(ModuleDeclaration node)
        {
            _instructionBuilder.DefineModule(node);
            BeginScope(DomainType.Module);
            ((ModuleSymbol)_symbol).Name = node.ModuleName;
            ((ModuleSymbol)_symbol).FilePath = node.FullPath;
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

            PredeclareVariables(node.ChildNodes, DeclareType.Property);
            _instructionBuilder.Comment("# Code Block");

            // skip only declare
            var functions = node.Functions.Where(e => e.Flags != FunctionFlags.Declare).ToArray();


            // 1. 声明代码块内方法
            var closureMap = new Dictionary<string, ClosureInstruction>();
            foreach (var function in functions)
            {
                var captured = EnsureCapturedVariables(function);
                var slot = _scope.Declare(DeclareType.Property, function);
                foreach (var capturedVar in captured)
                {
                    PushCapturedUpvalue(capturedVar);
                }
                _instructionBuilder.PushThis();
                closureMap[function.Name.UniqueValue] = _instructionBuilder.NewClosure(captured.Length);
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
            foreach (var function in functions)
            {
                var closure = closureMap[function.Name.UniqueValue];
                _instructionBuilder.FixClosure(closure);
                function.Accept(this);
            }

            EndScope();
        }





        protected override void VisitImportDeclaration(ImportDeclaration node)
        {
            // 加载模块至属性
            _instructionBuilder.GetGlobalProperty("@" + node.ModuleName);
            // 将导入的模块声明为当前模块的变量
            var solt = _scope.Declare(DeclareType.Property, node);
            _instructionBuilder.SetThisProperty(solt);
        }

        protected override void VisitBlock(BlockStatement node)
        {
            if (!node.IsFunction) BeginScope(DomainType.Code);

            // skip only declare
            PredeclareVariables(node.ChildNodes, DeclareType.Variable);
            var functions = node.Functions.Where(e => e.Flags != FunctionFlags.Declare).ToArray();
            // 1. 声明代码块内方法
            var closureMap = new Dictionary<string, ClosureInstruction>();
            foreach (var function in functions)
            {
                // skip only declare
                if (function.Flags == FunctionFlags.Declare) continue;
                var captured = EnsureCapturedVariables(function);
                var slot = _scope.Declare((node is ModuleDeclaration) ? DeclareType.Property : DeclareType.Variable, function);
                foreach (var capturedVar in captured)
                {
                    PushCapturedUpvalue(capturedVar);
                }
                _instructionBuilder.PushThis();
                closureMap[function.Name.UniqueValue] = _instructionBuilder.NewClosure(captured.Length);
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
            foreach (var function in functions)
            {
                var closure = closureMap[function.Name.UniqueValue];
                _instructionBuilder.FixClosure(closure);
                function.Accept(this);
            }
            if (!node.IsFunction) EndScope();
        }

        protected override void VisitFunction(FunctionDeclaration node)
        {
            if (node.Flags == FunctionFlags.Declare) return;
            _instructionBuilder.Comment($"# begin_func {node.Name?.Value}", 4);
            var capturedVariables = EnsureCapturedVariables(node);
            _instructionBuilder.Comment($"# Captured variables: {string.Join(", ", capturedVariables.Select(v => v.Name))}");

            BeginScope(DomainType.Function);
            var functionScope = _scope;

            ((FunctionSymbol)_symbol).Name = node.Name.Value;
            ((FunctionSymbol)_symbol).LineNumber = node.LineNumber;
            var allocLocals = _instructionBuilder.AllocLocals();
            // 定义参数变量
            foreach (var statement in node.Parameters)
            {
                statement.Accept(this);
            }

            // 为捕获的变量创建局部变量
            foreach (var captured in capturedVariables)
            {
                var slot = _scope.Declare(DeclareType.Captured, captured.Name, captured.AliasSlot);
                PushCapturedUpvalue(captured);
                _instructionBuilder.StoreLocal(slot);
            }

            // 编译函数体
            node.Body?.Accept(this);

            // 添加默认返回值
            _instructionBuilder.ReturnNull();

            var localsRequired = Math.Max(functionScope.MaxVariableCount, 0);
            _instructionBuilder.FixAllocLocals(allocLocals, localsRequired);
            EndScope();
            _instructionBuilder.Comment($"# end_func {node.Name?.Value}");
        }


        protected override void VisitLambdaExpression(LambdaExpression node)
        {
            var captured = EnsureCapturedVariables(node.Function);
            foreach (var capturedVar in captured)
            {
                PushCapturedUpvalue(capturedVar);
            }
            // 创建闭包
            _instructionBuilder.PushThis();
            var closure = _instructionBuilder.NewClosure(captured.Length);
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
                Print($"[{i:0000}] {str.Replace("\n", "\\n").Replace("\r", "")}", ConsoleColor.Blue);
            }
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





        protected override void VisitArrayExpression(ArrayLiteralExpression node)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                // Push the element value
                node.ChildNodes[i].Accept(this);
            }
            _instructionBuilder.NewArray(node.ChildNodes.Count);
        }




        protected override void VisitAssignmentExpression(AssignmentExpression node)
        {
            _instructionBuilder.Comment($"# {node.ToString()}");
            // Compile the value expression
            node.Right.Accept(this);
            // Duplicate the value on the stack (for the assignment expression's own value)
            //_instructionBuilder.Duplicate();
            // Store the value in the variable
            MoveValueTo(node.Left.ToString());
        }





        protected override void VisitBreakExpression(BreakStatement node)
        {
            _instructionBuilder.Comment($"# break;");
            _breakJumps.Peek().Add(_instructionBuilder.Jump());
        }

        protected override void VisitContinueExpression(ContinueStatement node)
        {
            _instructionBuilder.Comment($"# continue;");
            _continueJumps.Peek().Add(_instructionBuilder.Jump());
        }

        protected override void VisitYieldExpression(YieldStatement node)
        {
            _instructionBuilder.Comment($"# Yield;");
            _instructionBuilder.Yield();
        }


        protected override void VisitCallExpression(FunctionCallExpression node)
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


        protected override void VisitDeconstructionExpression(DeconstructionExpression node)
        {
            node.Right?.Accept(this);
            if (node.Parent is ArrayLiteralExpression)
            {
                _instructionBuilder.DeConstructArray();
            }
            else if (node.Parent is MapExpression)
            {
                _instructionBuilder.DeConstructMap();
            }
            else
            {
                throw new AuroraCompilerException("", "不允许的解构运算符");
            }

        }



        protected override void VisitParameterDeclaration(ParameterDeclaration node)
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



        protected override void VisitDeleteStatement(DeleteStatement node)
        {
            if (node.Expression is GetElementExpression getElementExpression)
            {
                getElementExpression.Object.Accept(this);

                if (getElementExpression.Index is LiteralExpression literalExpression)
                {
                    _instructionBuilder.PushConstantString(getElementExpression.Index.ToString());
                }
                else
                {
                    getElementExpression.Index.Accept(this);
                }
                _instructionBuilder.DeleteProperty();
            }
            else if (node.Expression is GetPropertyExpression getPropertyExpression)
            {
                getPropertyExpression.Object.Accept(this);
                _instructionBuilder.PushConstantString(getPropertyExpression.Property.ToString());
                _instructionBuilder.DeleteProperty();
            }
            else
            {
                throw new NotImplementedException();
            }
        }








        protected override void VisitGetElementExpression(GetElementExpression node)
        {
            node.Object.Accept(this);
            node.Index.Accept(this);
            _instructionBuilder.GetElement();
        }

        protected override void VisitGetPropertyExpression(GetPropertyExpression node)
        {
            node.Object.Accept(this);
            var propery = (NameExpression)node.Property;
            _instructionBuilder.GetProperty(propery.Identifier.Value);
        }



        protected override void VisitSetElementExpression(SetElementExpression node)
        {
            _instructionBuilder.Comment($"# {node}");
            // Compile the value expression
            node.Value.Accept(this);
            node.Index.Accept(this);
            node.Object.Accept(this);
            _instructionBuilder.SetElement();
        }



        protected override void VisitSetPropertyExpression(SetPropertyExpression node)
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

        protected override void VisitGroupingExpression(GroupExpression node)
        {
            foreach (var item in node.ChildNodes)
            {
                item.Accept(this);
            }

        }

        protected override void VisitIfStatement(IfStatement node)
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


        protected override void VisitLiteralExpression(LiteralExpression node)
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
            else if (token.Type == Tokens.ValueType.Regex && token is RegexToken regex)
            {
                _instructionBuilder.NewRegex(regex.Pattern, regex.Flags);
            }
        }


        protected override void VisitName(NameExpression node)
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
            if (node.Identifier.Value == "$state")
            {
                _instructionBuilder.PushUserContext();
                return;
            }
            if (node.Identifier.Value == "$args")
            {
                _instructionBuilder.PushArguments();
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



        protected override void VisitMapExpression(MapExpression node)
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
                else if (entry is DeconstructionExpression deconstruction)
                {
                    deconstruction.Accept(this);
                }
                // Pop the result of SET_PROPERTY (the value)
                //_instructionBuilder.Pop();
            }
        }


        protected override void VisitReturnStatement(ReturnStatement node)
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



        protected override void VisitBinaryExpression(BinaryExpression node)
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
            else if (node.Operator.Symbol == Symbols.OP_UNSIGNEDRIGHTSHIFT)
            {
                _instructionBuilder.Emit(OpCode.BIT_USHIFT_R);
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
                _instructionBuilder.Emit(OpCode.BIT_AND);
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


        protected override void VisitUnaryExpression(UnaryExpression node)
        {
            if (node.IsStateSegment) _instructionBuilder.Comment($"# {node.ToString()}");

            // 获取操作数
            var operand = node.Operand;

            // 确定操作码
            OpCode opCode = DetermineUnaryOpCode(node.Operator);

            if ((opCode == OpCode.INCREMENT || opCode == OpCode.DECREMENT)
                && operand is NameExpression localName
                && TryResolveLocalSlot(localName.Identifier.Value, out var localSlot))
            {
                var needResult = node.Parent != null;
                if (node.Type == UnaryType.Prefix)
                {
                    if (opCode == OpCode.INCREMENT)
                    {
                        _instructionBuilder.IncrementLocal(localSlot);
                    }
                    else
                    {
                        _instructionBuilder.DecrementLocal(localSlot);
                    }
                }
                else
                {
                    if (opCode == OpCode.INCREMENT)
                    {
                        _instructionBuilder.IncrementLocalPost(localSlot);
                    }
                    else
                    {
                        _instructionBuilder.DecrementLocalPost(localSlot);
                    }
                }

                if (!needResult)
                {
                    _instructionBuilder.Pop();
                }
                return;
            }

            if (node.Type == UnaryType.Prefix)
            {
                // 前缀操作：先执行操作，再返回值
                operand.Accept(this);
                _instructionBuilder.Emit(opCode);
                if ((opCode == OpCode.INCREMENT || opCode == OpCode.DECREMENT))
                {
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

            }
            else // PostFix
            {
                // 后缀操作：先加载值，复制一份，执行操作后保存，返回原值
                operand.Accept(this);

                _instructionBuilder.Emit(opCode);
                if ((opCode == OpCode.INCREMENT || opCode == OpCode.DECREMENT))
                {
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
        }

        private OpCode DetermineUnaryOpCode(Operator op)
        {
            if (Operator.PreIncrement == op || Operator.PostIncrement == op) return OpCode.INCREMENT;
            if (Operator.PreDecrement == op || Operator.PostDecrement == op) return OpCode.DECREMENT;
            if (Operator.LogicalNot == op) return OpCode.LOGIC_NOT;
            if (Operator.BitwiseNot == op) return OpCode.BIT_NOT;
            if (Operator.Negate == op) return OpCode.NEGATE;
            if (Operator.TypeOf == op) return OpCode.TYPEOF;
            throw new AuroraCompilerException("", $"Invalid operator: {op}");
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




        protected override void VisitVarDeclaration(VariableDeclaration node)
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

        protected override void VisitInExpression(InExpression node)
        {
            // todo
        }



        protected override void VisitForInStatement(ForInStatement node)
        {
            _instructionBuilder.Comment($"# for in");
            node.Initializer?.Accept(this);
            _breakJumps.Push(new List<JumpInstruction>());
            _continueJumps.Push(new List<JumpInstruction>());
            _instructionBuilder.Comment($"# condition {node.Iterator}");

            var varName = (NameExpression)node.Iterator.Left;
            var IteratorName = "*Iterator_" + varName.Identifier.UniqueValue;

            //var namedVarIndex =  _scope.Declare(DeclareType.Variable, varName.Identifier.Value);
            var iteratorVarIndex = _scope.Declare(DeclareType.Variable, IteratorName);
            // INIT var iterator = exp.Iterator;
            // INIT var name = iterator.value();
            // Condition = iterator.hasValue();
            // Incrementor = iterator.next();

            // 获取 迭代器 到 变量 IteratorName
            node.Iterator.Right.Accept(this);
            _instructionBuilder.GetIterator();
            _instructionBuilder.StoreLocal(iteratorVarIndex);

            var begin = _instructionBuilder.Position();

            // 
            _instructionBuilder.LoadLocal(iteratorVarIndex);
            _instructionBuilder.IteratorHasValue();

            // Jump out of loop if condition is false
            var exitJump = _instructionBuilder.JumpFalse();

            _instructionBuilder.Comment($"# IteratorValue");
            // 获取迭代器第一个值到 name变量 
            _instructionBuilder.LoadLocal(iteratorVarIndex);
            _instructionBuilder.IteratorValue();
            MoveValueTo(varName.Identifier.Value);


            _instructionBuilder.Comment($"# body");
            // Compile the loop body
            node.Body.Accept(this);
            foreach (var continueJump in _continueJumps.Peek())
            {
                _instructionBuilder.FixJumpToHere(continueJump);
            }
            var end = _instructionBuilder.Position();

            //
            _instructionBuilder.LoadLocal(iteratorVarIndex);
            _instructionBuilder.IteratorNext();
            //
            _instructionBuilder.JumpTo(begin);
            _instructionBuilder.FixJumpToHere(exitJump);
            foreach (var breakJump in _breakJumps.Peek())
            {
                _instructionBuilder.FixJumpToHere(breakJump);
            }





        }





        protected override void VisitForStatement(ForStatement node)
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


        protected override void VisitWhileStatement(WhileStatement node)
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

        protected override void VisitCompoundExpression(CompoundExpression node)
        {
            if (TryEmitLocalCompound(node))
            {
                return;
            }

            node.Left.Accept(this);
            node.Right.Accept(this);
            if (node.Operator == Operator.CompoundAdd) _instructionBuilder.Emit(OpCode.ADD);
            if (node.Operator == Operator.CompoundSubtract) _instructionBuilder.Emit(OpCode.SUBTRACT);
            if (node.Operator == Operator.CompoundMultiply) _instructionBuilder.Emit(OpCode.MULTIPLY);
            if (node.Operator == Operator.CompoundDivide) _instructionBuilder.Emit(OpCode.DIVIDE);
            if (node.Operator == Operator.CompoundModulo) _instructionBuilder.Emit(OpCode.MOD);


            MoveValueTo(node.Left.ToString());

            // TODO
        }



        private bool TryResolveLocalSlot(String identifier, out Int32 slot)
        {
            slot = -1;
            if (_scope.Resolve(identifier, out var declareObject) && declareObject.Type == DeclareType.Variable)
            {
                slot = declareObject.Index;
                return true;
            }
            return false;
        }

        private bool TryEmitLocalCompound(CompoundExpression node)
        {
            if (node.Left is not NameExpression nameExpr)
            {
                return false;
            }

            if (!TryResolveLocalSlot(nameExpr.Identifier.Value, out var slot))
            {
                return false;
            }

            node.Right.Accept(this);

            if (node.Operator == Operator.CompoundAdd)
            {
                _instructionBuilder.AddLocalFromStack(slot);
                return true;
            }
            if (node.Operator == Operator.CompoundSubtract)
            {
                _instructionBuilder.SubtractLocalFromStack(slot);
                return true;
            }
            if (node.Operator == Operator.CompoundMultiply)
            {
                _instructionBuilder.MultiplyLocalFromStack(slot);
                return true;
            }
            if (node.Operator == Operator.CompoundDivide)
            {
                _instructionBuilder.DivideLocalFromStack(slot);
                return true;
            }
            if (node.Operator == Operator.CompoundModulo)
            {
                _instructionBuilder.ModuloLocalFromStack(slot);
                return true;
            }

            return false;
        }

        private void MoveValueTo(String property)
        {
            if (_scope.Resolve(property, out var declareObject))
            {
                if (declareObject.Type == DeclareType.Captured)
                {
                    _instructionBuilder.StoreCapturedVariable(declareObject.Index);
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
