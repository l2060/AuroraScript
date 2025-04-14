using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Compiler.Ast.Expressions;
using AuroraScript.Compiler.Ast.Statements;
using AuroraScript.Compiler.Exceptions;
using AuroraScript.Tokens;


namespace AuroraScript.Analyzer
{
    public class AuroraParser
    {
        public ScriptCompiler Compiler { get; private set; }
        public AuroraLexer lexer { get; private set; }
        public ModuleDeclaration root { get; private set; }
        public AuroraParser(ScriptCompiler compiler, AuroraLexer lexer)
        {
            this.lexer = lexer;
            this.Compiler = compiler;
            this.root = new ModuleDeclaration(new Scope(this), this.lexer.Directory);
            this.root.ModulePath = lexer.FullPath;
            this.root.ModuleName = lexer.FullPath.Replace(lexer.Directory, "").Replace("\\", "/");
            this.root.MetaInfos.Add("module", this.root.ModuleName);
        }

        public ModuleDeclaration Parse()
        {
            while (true)
            {
                if (this.lexer.TestNext(Symbols.KW_EOF)) break;
                var node = ParseStatement(this.root.Scope);

                if (node is ModuleMetaStatement meta)
                {
                    this.root.MetaInfos[meta.Name.Value] = meta.Value?.Value;
                }

                if (node is VariableDeclaration variable)
                {
                    node.IsStateSegment = true;
                    this.root.Members.Add(variable);
                    this.root.AddNode(node);
                }
                else if (node is FunctionDeclaration func)
                {
                    node.IsStateSegment = true;
                    this.root.Functions.Add(func);
                    //this.root.Members.Add(func);
                }
                else if (node is ImportDeclaration importDeclaration)
                {
                    this.root.Imports.Add(importDeclaration);
                }
                else if (node != null)
                {
                    node.IsStateSegment = true;
                    this.root.AddNode(node);
                }
            }
            return this.root;
        }

        /// <summary>
        /// Parse a statement ending in “;” or a statement block wrapped by “{}”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="endSymbols"></param>
        /// <returns></returns>
        /// <exception cref="ParseException"></exception>
        private Statement ParseStatement(Scope currentScope)
        {
            var token = this.lexer.LookAtHead();
            // 改进错误处理
            if (token == null)
            {
                throw new ParseException(this.lexer.FullPath, this.lexer.Previous(), "Unexpected end of input while parsing statement");
            }
            if (token.Symbol == Symbols.PT_SEMICOLON)
            {
                // Ignore empty statements;
                this.lexer.Next();
                return null;
            }
            if (token.Symbol == Symbols.PT_METAINFO) return this.ParseMetaInfo(currentScope);
            if (token.Symbol == Symbols.PT_LEFTBRACE) return this.ParseBlock(currentScope);
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_DECLARE) return this.ParseDeclare(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_CONST) return this.ParseVariableDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_ENUM) return this.ParseEnumDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_WHILE) return this.ParseWhileBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);
            if (token.Symbol == Symbols.KW_CONTINUE) return this.ParseContinueStatement(currentScope);
            if (token.Symbol == Symbols.KW_BREAK) return this.ParseBreakStatement(currentScope);
            if (token.Symbol == Symbols.KW_RETURN) return this.ParseReturnStatement(currentScope);

            var exp = this.ParseExpression(currentScope);
            exp.IsStateSegment = true;
            return new ExpressionStatement(exp);
        }






        /**
         * parse extend type declare
         */
        private Statement ParseDeclare(Scope currentScope, MemberAccess access = MemberAccess.Internal)
        {
            this.lexer.NextOfKind(Symbols.KW_DECLARE);
            if (this.lexer.TestNext(Symbols.KW_FUNCTION))
            {
                // extend function
                var funcName = this.lexer.NextOfKind<IdentifierToken>();
                // next token (
                this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
                // parse arguments
                var arguments = this.ParseFunctionArguments(currentScope);

                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                var declaration = new FunctionDeclaration(access, funcName, arguments, null, FunctionFlags.Declare);
                return declaration;
            }
            throw this.InitParseException("The Declare keyword only allows the declaration of external methods ", this.lexer.LookAtHead());
        }


        /// <summary>
        /// parse enum declare list
        /// </summary>
        /// <returns></returns>
        private List<EnumElement> ParseEnumBody()
        {
            this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
            var result = new List<EnumElement>();
            var elementValue = 0;
            while (true)
            {
                if (this.lexer.TestNext(Symbols.PT_RIGHTBRACE)) break;
                var elementName = this.lexer.NextOfKind<IdentifierToken>();
                if (this.lexer.TestNext(Symbols.OP_ASSIGNMENT))
                {
                    var token = this.lexer.NextOfKind<ValueToken>();
                    if (token.Type != Tokens.ValueType.Number) throw this.InitParseException("Enumeration types only apply to integers", token);
                    elementValue = Int32.Parse(token.Value);
                }
                result.Add(new EnumElement() { Name = elementName, Value = elementValue });
                elementValue++;
                this.lexer.TestNext(Symbols.PT_COMMA);
            }
            return result;
        }

        /// <summary>
        /// parse enum type
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private Statement ParseEnumDeclaration(Scope currentScope, Symbols access = null)
        {
            this.lexer.NextOfKind(Symbols.KW_ENUM);
            var enumName = this.lexer.NextOfKind<IdentifierToken>();
            var elements = this.ParseEnumBody();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            var declaration = new EnumDeclaration() { Elements = elements, Identifier = enumName, Access = access };

            return declaration;
        }

        /// <summary>
        /// parse return expression
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseReturnStatement(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_RETURN);
            var exp = this.ParseExpression(currentScope);
            return new ReturnStatement(exp);
        }

        /// <summary>
        /// parse continue expression
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseContinueStatement(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_CONTINUE);
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            return new ContinueStatement();
        }

        /// <summary>
        /// parse break expression
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseBreakStatement(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_BREAK);
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            var statement = new BreakStatement();
            return statement;
        }



        private Statement ParseMetaInfo(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.PT_METAINFO);
            var metaName = this.lexer.NextOfKind<IdentifierToken>();
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            //var token = this.lexer.LookAtHead();
            Token metaValue = null;
            var defaultValue = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            var statement = new ModuleMetaStatement(metaName, metaValue);
            return statement;
        }



        /// <summary>
        /// Parse a block surrounded by {  }
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
            var scope = currentScope.CreateScope(ScopeType.BLOCK);
            var result = new BlockStatement(scope);
            result.IsNewScope = true;
            while (true)
            {
                var token = this.lexer.LookAtHead();
                // Check for the end brace (}).
                if (token.Symbol == Symbols.PT_RIGHTBRACE) break;
                // Parse a single statement.
                var exp = this.ParseStatement(scope);
                if (exp is FunctionDeclaration functionDeclaration)
                {
                    result.Functions.Add(functionDeclaration);
                    functionDeclaration.IsStateSegment = true;
                }
                else
                if (exp != null)
                {
                    result.AddNode(exp);
                    exp.IsStateSegment = true;
                }
            }
            // Consume the end brace.
            this.lexer.NextOfKind(Symbols.PT_RIGHTBRACE);
            return this.opaimizeStatement(result);
        }

        /// <summary>
        /// parse for block
        /// starting with “for”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseForBlock(Scope currentScope)
        {
            var scope = currentScope.CreateScope(ScopeType.FOR);
            this.lexer.NextOfKind(Symbols.KW_FOR);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse for initializer
            AstNode initializer = this.ParseStatement(scope);
            // remove expression layout
            if (initializer is ExpressionStatement es) initializer = es.Expression;
            // parse for condition
            var condition = this.ParseExpression(scope);
            // parse for incrementor
            var incrementor = this.ParseExpression(scope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line
            var body = this.ParseStatement(scope);
            if (body == null) throw new ParseException(this.lexer.FullPath, this.lexer.Previous(), "for body statement should not be empty");

            // parse for body
            return new ForStatement(condition, initializer, incrementor, body);
        }

        /// <summary>
        /// parse while block
        /// starting with “for”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseWhileBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_WHILE);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse while condition
            var condition = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line
            var body = this.ParseStatement(currentScope);
            if (body == null) throw new ParseException(this.lexer.FullPath, this.lexer.Previous(), "while body statement should not be empty");
            // parse while body
            return new WhileStatement(condition, body);
        }

        /// <summary>
        /// parse variable declaration
        /// starting with “var”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseVariableDeclaration(Scope currentScope, MemberAccess access = MemberAccess.Internal)
        {
            var isConst = false;
            // const
            if (this.lexer.TestNext(Symbols.KW_CONST))
            {
                isConst = true;
            }
            else if (this.lexer.TestNext(Symbols.KW_VAR))
            {
                isConst = false;
            }
            else
            {
                throw this.InitParseException("Variable declaration should be placed after var const ", this.lexer.LookAtHead());
            }

            var varName = this.lexer.NextOfKind<IdentifierToken>();

            var varNames = new List<Token>() { varName };
            Expression initializer = null;
            while (true)
            {
                var nextToken = this.lexer.Next();
                // ;
                if (nextToken.Symbol == Symbols.PT_SEMICOLON)
                {
                    break;
                }
                // =
                else if (nextToken.Symbol == Symbols.OP_ASSIGNMENT)
                {
                    initializer = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                    break;
                }
                // ,
                else if (nextToken.Symbol == Symbols.PT_COMMA)
                {
                    varName = this.lexer.NextOfKind<IdentifierToken>();
                    varNames.Add(varName);
                }
                else
                {
                    throw this.InitParseException("Invalid keywords appear in var declaration.", nextToken);
                }
            }
            // define variables
            var var = new VariableDeclaration(access, isConst, varNames[0]);
            if (initializer != null) var.AddNode(initializer);
            return var;
        }

        private Exception InitParseException(String message, Token token)
        {

            Console.WriteLine($"{token} {message}");
            return new ParseException(this.lexer.FullPath, token, String.Format(message, token));
        }

        /// <summary>
        /// parse if block
        /// starting with “if”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseIfBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_IF);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);

            var condition = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line
            // parse if body
            Statement body = this.ParseStatement(currentScope);
            if (body == null) throw new ParseException(this.lexer.FullPath, this.lexer.Previous(), "if body statement should not be empty");

            Statement elseStatement = null;
            var nextToken = this.lexer.LookAtHead();
            if (nextToken.Symbol == Symbols.KW_ELSE)
            {
                // parse else
                elseStatement = this.ParseElseBlock(currentScope);
            }

            var ifStatement = new IfStatement(condition, body, elseStatement);

            return ifStatement;
        }

        private Statement opaimizeStatement(Statement statement)
        {
            var node = statement;
            var parent = statement.Parent;
            while (node is BlockStatement block && block.Length == 1 && block.Functions.Count == 0)
            {
                node = (Statement)block.ChildNodes[0];
                node.Remove();
                if (parent != null) parent.AddNode(node);
            }
            return node;
        }

        /// <summary>
        /// parse else block
        /// starting with “else”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseElseBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_ELSE);
            BlockStatement block = new BlockStatement(currentScope);
            block.IsNewScope = true;
            if (this.lexer.TestSymbol(Symbols.KW_IF))
            {
                var statement = this.ParseIfBlock(currentScope);

                block.AddNode(statement);
            }
            else
            {
                var expression = this.ParseStatement(currentScope);
                if (expression == null) throw new ParseException(this.lexer.FullPath, this.lexer.Previous(), "else body statement should not be empty");
                expression.IsStateSegment = true;
                block.AddNode(expression);
            }
            return this.opaimizeStatement(block);
        }

        /// <summary>
        /// parse an expression
        /// ending symbol in endSymbols or “;”;
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="endSymbols"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Expression ParseExpression(Scope currentScope, params Symbols[] endSymbols)
        {
            var token = lexer.LookAtHead();
            if (endSymbols.Contains(token.Symbol))
            {
                return null;
            }

            // 检查是否是箭头函数
            if (IsArrowFunctionStart())
            {
                return ParseArrowFunction(currentScope);
            }

            return ParseAssignment(currentScope, endSymbols);
        }

        private bool IsArrowFunctionStart()
        {
            var token = lexer.LookAtHead();

            // 检查 () => 形式
            if (token.Symbol == Symbols.PT_LEFTPARENTHESIS)
            {
                var snapshot = lexer.CreateSnapshot();
                lexer.Next(); // 消费 (

                // 跳过参数列表
                while (!lexer.TestNext(Symbols.PT_RIGHTPARENTHESIS))
                {
                    if (!(lexer.Next() is IdentifierToken))
                    {
                        lexer.RestoreSnapshot(snapshot);
                        return false;
                    }
                    lexer.TestNext(Symbols.PT_COMMA);
                }

                // 检查是否后面跟着 =>
                var isArrow = lexer.TestNext(Symbols.PT_LAMBDA);
                lexer.RestoreSnapshot(snapshot);
                return isArrow;
            }

            // 检查 单参数 => 形式
            if (token is IdentifierToken)
            {
                var snapshot = lexer.CreateSnapshot();
                lexer.Next(); // 消费标识符
                var isArrow = lexer.TestNext(Symbols.PT_LAMBDA);
                lexer.RestoreSnapshot(snapshot);
                return isArrow;
            }

            return false;
        }

        private Expression ParseArrowFunction(Scope currentScope)
        {
            var name = new IdentifierToken
            {
                Value = $"$arrow_{lexer.LineNumber}_{lexer.ColumnNumber}",
                LineNumber = lexer.LineNumber,
                ColumnNumber = lexer.ColumnNumber
            };

            List<ParameterDeclaration> parameters = new List<ParameterDeclaration>();

            // 解析参数
            if (lexer.LookAtHead().Symbol == Symbols.PT_LEFTPARENTHESIS)
            {
                lexer.Next(); // 消费 (
                parameters = ParseFunctionArguments(currentScope);
            }
            else
            {
                // 单参数形式
                var param = lexer.NextOfKind<IdentifierToken>();
                parameters.Add(new ParameterDeclaration((byte)0, param, null));
            }

            // 消费 =>
            lexer.NextOfKind(Symbols.PT_LAMBDA);

            // 创建函数作用域
            var functionScope = currentScope.CreateScope(ScopeType.FUNCTION);

            // 解析函数体
            Statement body;
            if (lexer.TestSymbol(Symbols.PT_LEFTBRACE))
            {
                body = ParseBlock(functionScope);
            }
            else
            {
                // 单表达式箭头函数
                var expr = ParseExpression(functionScope);
                var returnStmt = new ReturnStatement(expr);
                var blockStmt = new BlockStatement(functionScope);
                blockStmt.AddNode(returnStmt);
                body = blockStmt;
            }

            var func = new FunctionDeclaration(MemberAccess.Internal, name, parameters, body, FunctionFlags.Lambda);
            var lambda = new LambdaExpression { Function = func };
            return lambda;
        }

        private Expression ParseAssignment(Scope currentScope, Symbols[] endSymbols)
        {
            var left = ParseLogicalOr(currentScope);

            var token = lexer.LookAtHead();
            if (endSymbols.Contains(token.Symbol))
            {
                return left;
            }
            if (token.Symbol == null) return left;
            var op = Operator.FromSymbols(token.Symbol, true);
            if (op != null && op.Symbol == Symbols.OP_ASSIGNMENT)
            {
                lexer.Next(); // consume operator
                var right = ParseAssignment(currentScope, endSymbols);

                // 处理特殊的赋值表达式
                if (left is GetPropertyExpression getter)
                {
                    var setter = new SetPropertyExpression();
                    setter.AddNode(getter.Object);
                    setter.AddNode(getter.Property);
                    setter.AddNode(right);
                    return setter;
                }
                else if (left is GetElementExpression elementGetter)
                {
                    var setter = new SetElementExpression();
                    setter.AddNode(elementGetter.Object);
                    setter.AddNode(elementGetter.Index);
                    setter.AddNode(right);
                    return setter;
                }

                var assignment = new AssignmentExpression(op);
                assignment.AddNode(left);
                assignment.AddNode(right);
                return assignment;
            }

            return left;
        }

        private Expression ParseLogicalOr(Scope currentScope)
        {
            var left = ParseLogicalAnd(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);

                if (op != null && op == Operator.LogicalOr)
                {
                    lexer.Next();
                    var right = ParseLogicalAnd(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseLogicalAnd(Scope currentScope)
        {
            var left = ParseEquality(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);

                if (op != null && op == Operator.LogicalAnd)
                {
                    lexer.Next();
                    var right = ParseEquality(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseEquality(Scope currentScope)
        {
            var left = ParseComparison(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);

                if (op != null && (op == Operator.Equal || op == Operator.NotEqual))
                {
                    lexer.Next();
                    var right = ParseComparison(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseComparison(Scope currentScope)
        {
            var left = ParseAdditive(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);

                if (op != null && IsComparisonOperator(op))
                {
                    lexer.Next();
                    var right = ParseAdditive(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseAdditive(Scope currentScope)
        {
            var left = ParseMultiplicative(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);

                if (op != null && (op == Operator.Add || op == Operator.Subtract))
                {
                    lexer.Next();
                    var right = ParseMultiplicative(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseMultiplicative(Scope currentScope)
        {
            var left = ParseUnary(currentScope);

            while (true)
            {
                var token = lexer.LookAtHead();
                if (token.Symbol == null) break;
                var op = Operator.FromSymbols(token.Symbol, true);
                if (op != null && (op == Operator.Multiply || op == Operator.Divide || op == Operator.Modulo))
                {
                    lexer.Next();
                    var right = ParseUnary(currentScope);
                    var binary = new BinaryExpression(op);
                    binary.AddNode(left);
                    binary.AddNode(right);
                    left = binary;
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private Expression ParseUnary(Scope currentScope)
        {
            var token = lexer.LookAtHead();

            // 处理前缀运算符 (++x, --x, !x, ~x, -x)
            if (token.Symbol != null)
            {
                var op = Operator.FromSymbols(token.Symbol, false);
                if (op != null && (
                    op == Operator.PreIncrement ||
                    op == Operator.PreDecrement ||
                    op == Operator.LogicalNot ||
                    op == Operator.BitwiseNot ||
                    op == Operator.Negate))
                {
                    lexer.Next(); // 消费运算符
                    var operand = ParsePrimary(currentScope); // 使用ParsePrimary而不是ParseUnary避免递归问题

                    // 验证操作数是否是可赋值的表达式（对于++/--）
                    if ((op == Operator.PreIncrement || op == Operator.PreDecrement) &&
                        !(operand is NameExpression || operand is GetPropertyExpression || operand is GetElementExpression))
                    {
                        throw InitParseException("Invalid increment/decrement operand", token);
                    }

                    var unary = new UnaryExpression(op, UnaryType.Prefix);
                    unary.AddNode(operand);
                    return unary;
                }
            }

            // 解析基本表达式
            var expr = ParsePrimary(currentScope);

            // 处理后缀运算符 (x++, x--)
            token = lexer.LookAtHead();
            if (token.Symbol != null)
            {
                var op = Operator.FromSymbols(token.Symbol, true);
                if (op != null && (op == Operator.PostIncrement || op == Operator.PostDecrement))
                {
                    // 验证操作数是否是可赋值的表达式
                    if (!(expr is NameExpression || expr is GetPropertyExpression || expr is GetElementExpression))
                    {
                        throw InitParseException("Invalid increment/decrement operand", token);
                    }

                    lexer.Next(); // 消费运算符
                    var unary = new UnaryExpression(op, UnaryType.Post);
                    unary.AddNode(expr);
                    return unary;
                }
            }

            return expr;
        }

        // 添加辅助方法来检查表达式是否可赋值
        private bool IsAssignable(Expression expr)
        {
            return expr is NameExpression ||
                   expr is GetPropertyExpression ||
                   expr is GetElementExpression;
        }

        private Expression ParsePrimary(Scope currentScope)
        {
            var token = lexer.Next();
            Expression expr;

            if (token is ValueToken valueToken)
            {
                expr = new LiteralExpression(valueToken);
            }
            else if (token is IdentifierToken)
            {
                expr = new NameExpression { Identifier = token };
            }
            else if (token.Symbol == Symbols.PT_LEFTPARENTHESIS)
            {
                expr = ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
                lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
            }
            else if (token.Symbol == Symbols.PT_LEFTBRACKET)
            {
                expr = ParseArrayLiteral(currentScope);
            }
            else if (token.Symbol == Symbols.PT_LEFTBRACE)
            {
                expr = ParseObjectLiteral(currentScope);
            }
            else
            {
                throw InitParseException("Unexpected token in expression", token);
            }

            // 解析后缀表达式（属性访问、数组索引、函数调用等）
            while (true)
            {
                token = lexer.LookAtHead();

                // 处理属性访问 (a.b)
                if (token.Symbol == Symbols.PT_DOT)
                {
                    lexer.Next(); // 消费点操作符
                    var property = lexer.NextOfKind<IdentifierToken>();
                    if (property == null)
                    {
                        throw InitParseException("Expected property name after '.'", token);
                    }

                    var propertyExpr = new GetPropertyExpression(Operator.MemberAccess);
                    propertyExpr.AddNode(expr);
                    propertyExpr.AddNode(new NameExpression { Identifier = property });
                    expr = propertyExpr;
                }
                // 处理数组索引 (a[b])
                else if (token.Symbol == Symbols.PT_LEFTBRACKET)
                {
                    lexer.Next(); // 消费左方括号
                    var index = ParseExpression(currentScope, Symbols.PT_RIGHTBRACKET);
                    lexer.NextOfKind(Symbols.PT_RIGHTBRACKET);

                    var elementExpr = new GetElementExpression(Operator.Index);
                    elementExpr.AddNode(expr);
                    elementExpr.AddNode(index);
                    expr = elementExpr;
                }
                // 处理函数调用 (a())
                else if (token.Symbol == Symbols.PT_LEFTPARENTHESIS)
                {
                    lexer.Next(); // 消费左括号
                    var callExpr = new FunctionCallExpression(Operator.FunctionCall);
                    callExpr.AddNode(expr);
                    // 解析参数列表
                    while (true)
                    {
                        var arg = ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                        if(arg != null) callExpr.AddArgument(arg);

                        if (!lexer.TestNext(Symbols.PT_COMMA))
                        {
                            break;
                        }
                    }
                    lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
                    expr = callExpr;
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private bool IsComparisonOperator(Operator op)
        {
            return op == Operator.LessThan ||
                   op == Operator.LessThanOrEqual ||
                   op == Operator.GreaterThan ||
                   op == Operator.GreaterThanOrEqual;
        }

        private bool IsUnaryOperator(Operator op)
        {
            return op == Operator.LogicalNot ||
                   op == Operator.Negate ||
                   op == Operator.PreIncrement ||
                   op == Operator.PreDecrement;
        }


        private Expression optimize(Expression exp)
        {
            if (exp is AssignmentExpression assignment)
            {
                if (assignment.Left is GetPropertyExpression getter)
                {
                    var setter = new SetPropertyExpression();
                    setter.AddNode(getter.Object);
                    setter.AddNode(getter.Property);
                    setter.AddNode(assignment.Right);
                    return setter;
                }
                else if (assignment.Left is GetElementExpression eleGetter)
                {
                    var setter = new SetElementExpression();
                    setter.AddNode(eleGetter.Index);
                    setter.AddNode(eleGetter.Object);
                    setter.AddNode(assignment.Right);
                    return setter;
                }
            }
            else if (exp is CompoundExpression compound)
            {
                if (compound.Left is GetPropertyExpression getter)
                {
                    var setter = new SetPropertyExpression();
                    setter.AddNode(getter.Object);
                    setter.AddNode(getter.Property);
                    var binary = new BinaryExpression(compound.Operator.SimplerOperator);
                    binary.AddNode(getter);
                    binary.AddNode(compound.Right);
                    setter.AddNode(binary);
                    return setter;
                }
                else if (compound.Left is GetElementExpression eleGetter)
                {
                    var setter = new SetElementExpression();
                    setter.AddNode(eleGetter.Index);
                    setter.AddNode(eleGetter.Object);
                    var binary = new BinaryExpression(compound.Operator.SimplerOperator);
                    binary.AddNode(eleGetter);
                    binary.AddNode(compound.Right);
                    setter.AddNode(binary);
                    return setter;
                }
            }
            return exp;
        }


        private Expression createExpression(Scope currentScope, Operator _operator, Token previousToken, Expression lastExpression)
        {
            // Assignment Expression
            if (_operator == Operator.Assignment) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundAdd) return new CompoundExpression(_operator);
            if (_operator == Operator.CompoundSubtract) return new CompoundExpression(_operator);
            if (_operator == Operator.CompoundDivide) return new CompoundExpression(_operator);
            if (_operator == Operator.CompoundModulo) return new CompoundExpression(_operator);
            if (_operator == Operator.CompoundMultiply) return new CompoundExpression(_operator);
            // new expression
            if (_operator == Operator.New) return new BinaryExpression(_operator);
            if (_operator == Operator.ObjectLiteral) return new MapExpression(_operator);
            // function call expression
            if (_operator == Operator.FunctionCall)
            {
                if (!(previousToken is PunctuatorToken) ||
                    lastExpression is GetElementExpression)
                {
                    return new FunctionCallExpression(_operator);
                }
            }
            // Grouping expression
            if (_operator == Operator.Grouping  /* && previousToken is PunctuatorToken */ ) return new GroupExpression(_operator);
            // member access expression
            if (_operator == Operator.ArrayLiteral) return new ArrayLiteralExpression();
            // member index expression
            if (_operator == Operator.Index) return new GetElementExpression(_operator);

            // member access expression
            if (_operator == Operator.MemberAccess) return new GetPropertyExpression(_operator);
            // binary expression
            if (_operator == Operator.Add) return new BinaryExpression(_operator);
            if (_operator == Operator.Divide) return new BinaryExpression(_operator);
            if (_operator == Operator.Equal) return new BinaryExpression(_operator);
            if (_operator == Operator.LeftShift) return new BinaryExpression(_operator);
            if (_operator == Operator.LessThan) return new BinaryExpression(_operator);
            if (_operator == Operator.LessThanOrEqual) return new BinaryExpression(_operator);
            if (_operator == Operator.GreaterThan) return new BinaryExpression(_operator);
            if (_operator == Operator.GreaterThanOrEqual) return new BinaryExpression(_operator);
            if (_operator == Operator.BitwiseAnd) return new BinaryExpression(_operator);
            if (_operator == Operator.BitwiseOr) return new BinaryExpression(_operator);
            if (_operator == Operator.BitwiseXor) return new BinaryExpression(_operator);
            if (_operator == Operator.LogicalAnd) return new BinaryExpression(_operator);
            //if (_operator == Operator.CastType) return new CastTypeExpression(_operator);


            //if (_operator == Operator.SetMember) return new MapKeyValueExpression(_operator);
            if (_operator == Operator.LogicalOr) return new BinaryExpression(_operator);
            if (_operator == Operator.Modulo) return new BinaryExpression(_operator);
            if (_operator == Operator.Multiply) return new BinaryExpression(_operator);
            if (_operator == Operator.NotEqual) return new BinaryExpression(_operator);
            if (_operator == Operator.SignedRightShift) return new BinaryExpression(_operator);
            if (_operator == Operator.Subtract) return new BinaryExpression(_operator);

            // Prefix expression
            if (_operator == Operator.PreSpread) return new DeconstructionExpression();


            if (_operator == Operator.BitwiseNot) return new UnaryExpression(_operator, UnaryType.Prefix);
            if (_operator == Operator.Negate) return new UnaryExpression(_operator, UnaryType.Prefix);
            if (_operator == Operator.PreDecrement) return new UnaryExpression(_operator, UnaryType.Prefix);
            if (_operator == Operator.PreIncrement) return new UnaryExpression(_operator, UnaryType.Prefix);
            if (_operator == Operator.TypeOf) return new UnaryExpression(_operator, UnaryType.Prefix);
            if (_operator == Operator.LogicalNot) return new UnaryExpression(_operator, UnaryType.Prefix);
            // Postfix expression
            if (_operator == Operator.PostDecrement) return new UnaryExpression(_operator, UnaryType.Post);
            if (_operator == Operator.PostIncrement) return new UnaryExpression(_operator, UnaryType.Post);
            return null;
        }

        private Expression ParseObjectConstructor(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);

            var constructExpression = new MapExpression(Operator.ObjectLiteral);
            while (true)
            {
                if (this.lexer.TestNext(Symbols.PT_RIGHTBRACE))
                {
                    break;
                }
                if (this.lexer.TestNext(Symbols.OP_SPREAD))
                {
                    var value = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACE);
                    this.lexer.RollBack();
                    var spread = new DeconstructionExpression();
                    spread.AddNode(value);
                    constructExpression.AddNode(spread);
                    continue;
                }
                Token varName = this.lexer.TestNextOfKind<IdentifierToken>();
                if (varName == null) varName = this.lexer.TestNextOfKind<StringToken>();
                if (varName == null) varName = this.lexer.TestNextOfKind<NumberToken>();
                if (varName == null) varName = this.lexer.TestNextOfKind<BooleanToken>();
                if (varName == null) varName = this.lexer.TestNextOfKind<NullToken>();
                if (varName == null)
                {
                    throw new Exception("无效的Map构建语法");
                }

                if (this.lexer.TestNext(Symbols.PT_COLON))
                {
                    var value = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACE);
                    var newExp = new MapKeyValueExpression(varName, value);
                    constructExpression.AddNode(newExp);
                    if (value is not MapExpression && value is not LambdaExpression)
                    {
                        this.lexer.RollBack();
                    }
                }
                else
                {
                    var nameToken = new NameExpression();
                    nameToken.Identifier = varName;
                    constructExpression.AddNode(new MapKeyValueExpression(varName, nameToken));
                }
                // Encountered comma break ;
                this.lexer.TestNext(Symbols.PT_COMMA);
            }
            return constructExpression;
        }

        /// <summary>
        /// Parse Function Arguments
        /// starting with “arg1: number, argn: string)”
        /// </summary>
        /// <returns></returns>
        private List<ParameterDeclaration> ParseFunctionArguments(Scope currentScope)
        {
            var arguments = new List<ParameterDeclaration>();
            while (true)
            {
                // Parsing modifier
                // .......
                // ==================

                if (this.lexer.TestNext(Symbols.PT_RIGHTPARENTHESIS))
                {
                    break;
                }
                var spreadOperator = false;
                // 扩展运算符
                if (this.lexer.TestNext(Symbols.OP_SPREAD))
                {
                    spreadOperator = true;
                }
                var varname = this.lexer.NextOfKind<IdentifierToken>();
                //this.lexer.NextOfKind(Symbols.PT_COLON);
                //var typed = this.ParseObjectType(currentScope);
                Expression defaultValue = null;
                // argument default value
                if (this.lexer.TestNext(Symbols.OP_ASSIGNMENT))
                {
                    defaultValue = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                    this.lexer.RollBack();
                }
                var declaration = new ParameterDeclaration((Byte)arguments.Count, varname, defaultValue);
                arguments.Add(declaration);
                // Encountered comma break ;
                this.lexer.TestNext(Symbols.PT_COMMA);
                // Encountered closing parenthesis break ;
                if (this.lexer.TestNext(Symbols.PT_RIGHTPARENTHESIS)) break;
            }
            return arguments;
        }



        private LambdaExpression ParseLamdaExpression(Scope currentScope)
        {
            var name = new IdentifierToken();

            var position = this.lexer.LookAtHead();

            name.Value = "$lambda_" + position.LineNumber + "_" + position.ColumnNumber;
            var func = ParseFunction(name, currentScope, MemberAccess.Internal, FunctionFlags.Lambda);
            var lambda = new LambdaExpression();
            lambda.Function = func;
            return lambda;
        }

        /// <summary>
        /// parse function block
        /// starting with “(” arguments declarations
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private FunctionDeclaration ParseFunction(IdentifierToken functionName, Scope currentScope, MemberAccess access = MemberAccess.Internal, FunctionFlags flags = FunctionFlags.General)
        {
            // next token (
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse arguments
            var arguments = this.ParseFunctionArguments(currentScope);

            if (flags == FunctionFlags.Lambda)
            {
                this.lexer.NextOfKind(Symbols.PT_LAMBDA);
            }
            // create function scope
            var scope = currentScope.CreateScope(ScopeType.FUNCTION);
            // parse function body
            var body = this.ParseBlock(scope);

            if (!(body is BlockStatement))
            {
                var newBody = new BlockStatement(scope);
                newBody.AddNode(body);

                body = newBody;
            }
            ((BlockStatement)body).IsNewScope = false;
            var declaration = new FunctionDeclaration(access, functionName, arguments, body, flags);
            return declaration;
        }



        /// <summary>
        /// analyze function declarations
        /// starting with “function”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private Statement ParseFunctionDeclaration(Scope currentScope, MemberAccess access = MemberAccess.Internal)
        {
            this.lexer.NextOfKind(Symbols.KW_FUNCTION);
            var functionName = this.lexer.NextOfKind<IdentifierToken>();
            // 校验 方法名是否有效
            return this.ParseFunction(functionName, currentScope, access, FunctionFlags.General);
        }


        /// <summary>
        /// parse export object
        /// starting with “export”
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LexerException"></exception>
        private Statement ParseExportStatement(Scope currentScope)
        {
            var exportToken = this.lexer.NextOfKind(Symbols.KW_EXPORT);
            if (currentScope.Type != ScopeType.MODULE)
            {
                throw new Exception(String.Format("Invalid “export” keyword in row {0}, column {1}, scope not supported.", exportToken.LineNumber, exportToken.ColumnNumber));
            }


            var token = this.lexer.LookAtHead();
            if (token is KeywordToken && token.Symbol == Symbols.KW_FUNCTION)
            {
                // function
                return ParseFunctionDeclaration(currentScope, MemberAccess.Export);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_VAR)
            {
                // var
                return ParseVariableDeclaration(currentScope, MemberAccess.Export);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_CONST)
            {
                // const
                return ParseVariableDeclaration(currentScope, MemberAccess.Export);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_ENUM)
            {
                // enum
                return ParseEnumDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_DECLARE)
            {
                // type
                return ParseDeclare(currentScope);
            }
            throw this.InitParseException("Invalid keywords appear in export declaration .", token);
        }

        /// <summary>
        /// parse import module
        /// starting with “import”
        /// </summary>
        /// <returns></returns>
        private Statement ParseImport()
        {
            this.lexer.NextOfKind(Symbols.KW_IMPORT);
            var module = this.lexer.NextOfKind<IdentifierToken>();
            this.lexer.NextOfKind(Symbols.KW_FROM);
            Token fileToken = this.lexer.NextOfKind<StringToken>();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);


            var fullPath = ResolveImportPath(fileToken.Value);
            if (!File.Exists(fullPath))
            {
                throw new CompilerException(fullPath, $"Import file not found: {fileToken.Value}");
            }
            return new ImportDeclaration() { Module = module, File = fileToken, FullPath = fullPath };
        }


        private string ResolveImportPath(string path)
        {
            var fullPath = Path.GetFullPath(Path.Combine(this.lexer.Directory, path));
            var extension = Path.GetExtension(fullPath).ToLower();
            if (extension != ".as")
            {
                fullPath = Path.ChangeExtension(fullPath, ".as");
            }
            return fullPath;
        }

        private Expression ParseArrayLiteral(Scope currentScope)
        {
            var arrayExpression = new ArrayLiteralExpression();

            // 已经消费了左方括号 [，现在开始解析数组元素
            while (true)
            {
                // 检查是否到达数组结尾
                if (lexer.TestNext(Symbols.PT_RIGHTBRACKET))
                {
                    break;
                }

                // 检查展开运算符 ...
                if (lexer.TestNext(Symbols.OP_SPREAD))
                {
                    var spreadValue = ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACKET);
                    var spread = new DeconstructionExpression();
                    spread.AddNode(spreadValue);
                    arrayExpression.AddNode(spread);
                }
                else
                {
                    // 解析普通数组元素
                    var element = ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACKET);
                    arrayExpression.AddNode(element);
                }

                // 如果下一个符号不是逗号，回退一步（可能是右方括号）
                lexer.TestNext(Symbols.PT_COMMA);
            }

            return arrayExpression;
        }

        private Expression ParseObjectLiteral(Scope currentScope)
        {
            var objectExpression = new MapExpression(Operator.ObjectLiteral);
            //lexer.NextOfKind(Symbols.PT_LEFTBRACE);

            while (!lexer.TestNext(Symbols.PT_RIGHTBRACE))
            {
                // 处理展开运算符
                if (lexer.TestNext(Symbols.OP_SPREAD))
                {
                    var spreadValue = ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACE);
                    var spread = new DeconstructionExpression();
                    spread.AddNode(spreadValue);
                    objectExpression.AddNode(spread);
                    lexer.TestNext(Symbols.PT_COMMA);
                    continue;
                }

                // 解析属性键
                Token propertyKey = ParseObjectPropertyKey();
                if (propertyKey == null)
                {
                    throw InitParseException("Invalid object literal property key", lexer.LookAtHead());
                }

                // 检查是否是简写属性
                if (lexer.TestNext(Symbols.PT_COLON))
                {
                    var propertyValue = ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTBRACE);
                    objectExpression.AddNode(new MapKeyValueExpression(propertyKey, propertyValue));
                }
                else
                {
                    // 处理简写属性
                    var nameExp = new NameExpression { Identifier = propertyKey };
                    objectExpression.AddNode(new MapKeyValueExpression(propertyKey, nameExp));
                }

                lexer.TestNext(Symbols.PT_COMMA);
            }

            return objectExpression;
        }

        private Token ParseObjectPropertyKey()
        {
            // 尝试解析各种可能的属性键类型
            Token key = lexer.TestNextOfKind<IdentifierToken>();
            if (key != null) return key;

            key = lexer.TestNextOfKind<StringToken>();
            if (key != null) return key;

            key = lexer.TestNextOfKind<NumberToken>();
            if (key != null) return key;

            key = lexer.TestNextOfKind<BooleanToken>();
            if (key != null) return key;

            key = lexer.TestNextOfKind<NullToken>();
            if (key != null) return key;

            return null;
        }
    }
}
