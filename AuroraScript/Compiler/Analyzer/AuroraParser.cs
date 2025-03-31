using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Compiler.Ast.Expressions;
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
            this.root.ModuleName = lexer.FullPath;
        }

        public ModuleDeclaration Parse()
        {
            while (true)
            {
                if (this.lexer.TestNext(Symbols.KW_EOF)) break;
                var node = ParseStatement(this.root.Scope);
                if (node is FunctionDeclaration func)
                {
                    node.IsStateSegment = true;
                    this.root.Functions.Add(func);
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
            if (token == null) throw new ParseException(this.lexer.FullPath, token, "Invalid keywords appear in ");
            //if (token.Symbol == Symbols.KW_EOF) throw new ParseException(this.lexer.FullPath, token, "Unclosed scope ");
            if (token.Symbol == Symbols.PT_SEMICOLON)
            {
                // Ignore empty statements;
                this.lexer.Next();
                return null;
            }
            if (token.Symbol == Symbols.PT_LEFTBRACE) return this.ParseBlock(currentScope);
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_DECLARE) return this.ParseDeclare(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_CONST) return this.ParseVariableDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_ENUM) return this.ParseEnumDeclaration(currentScope, Symbols.KW_INTERNAL);
            //if (token.Symbol == Symbols.KW_TYPE) return this.ParseTypeDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope, MemberAccess.Internal);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_WHILE) return this.ParseWhileBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);
            if (token.Symbol == Symbols.KW_CONTINUE) return this.ParseContinueStatement(currentScope);
            if (token.Symbol == Symbols.KW_BREAK) return this.ParseBreakStatement(currentScope);
            if (token.Symbol == Symbols.KW_RETURN) return this.ParseReturnStatement(currentScope);
            if (token.Symbol == Symbols.KW_CLASS) return this.ParseClass(currentScope, Symbols.KW_INTERNAL);
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
                // next token : Typed
                //this.lexer.NextOfKind(Symbols.PT_COLON);
                // typed
                //var typeds = this.ParseFunctionReturnTypeds(currentScope);
                // ;
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                var declaration = new FunctionDeclaration(access, funcName, arguments, null, FunctionFlags.Declare);
                return declaration;
            }
            throw this.InitParseException("The Declare keyword only allows the declaration of external methods ", this.lexer.LookAtHead());
        }

        /// <summary>
        /// parse Object Typed
        /// </summary>
        /// <returns></returns>
        //private TypeNode ParseObjectType(Scope currentScope)
        //{
        //    IdentifierToken basicType = null;
        //    TypeNode node;
        //    if (this.lexer.TestNext(Symbols.PT_LEFTPARENTHESIS))
        //    {
        //        this.lexer.RollBack();
        //        var type = this.ParseFunctionSignatureExpression(currentScope) as FunctionType;
        //        node = new FunctionTypeNode() { Parameters = type.Parameters, Typeds = type.Typeds };
        //    }
        //    else
        //    {
        //        basicType = this.lexer.TestNextOfKind<IdentifierToken>();
        //        node = new KeywordTypeNode(basicType);
        //    }

        //    if (this.lexer.TestNext(Symbols.PT_LEFTBRACKET))
        //    {
        //        this.lexer.NextOfKind(Symbols.PT_RIGHTBRACKET);

        //        return new ArrayType(node);
        //    }
        //    return node;
        //}

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
                else if (exp != null)
                {
                    result.AddNode(exp);
                    exp.IsStateSegment = true;
                }
            }
            // Consume the end brace.
            this.lexer.NextOfKind(Symbols.PT_RIGHTBRACE);

            //if (result.Length == 1)
            //{
            //    var statement = result.ChildNodes[0];
            //    statement.Remove();
            //    return statement as Statement;
            //}
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

            // next token is : define var typed
            //if (this.lexer.TestNext(Symbols.PT_COLON))
            //{
            //    variables.Typed = this.ParseObjectType(currentScope);
            //}
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
            var exp = new Statements();
            var v = new VariableDeclaration(access, isConst, varNames[0]);
            if (initializer != null) v.AddNode(initializer);
            exp.AddNode(v);
            var preVarName = varNames[0];
            for (int i = 1; i < varNames.Count; i++)
            {
                var @var = new VariableDeclaration(access, isConst, varNames[i]);
                if (initializer != null)
                {
                    var lexp = new NameExpression();
                    lexp.Identifier = preVarName;
                    @var.AddNode(lexp);
                }
                exp.AddNode(@var);
                preVarName = varNames[i];
            }


            return exp;
        }

        private Exception InitParseException(String message, Token token)
        {
            Console.WriteLine($"{token} {message}");
            return new ParseException(this.lexer.FullPath, token, message);
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
            while (node is BlockStatement block && block.Length == 1)
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
            Expression rootExpression = new ExpressionStack();
            Expression lastExpression = null;
            Operator lastOperator = null;
            var startPointer = this.lexer.Position;
            var rollBackLexer = () =>
            {
                while (this.lexer.Position > startPointer) this.lexer.RollBack();
            };

            while (true)
            {
                var pos = this.lexer.Position;
                var token = this.lexer.Next();
                // over statement
                if (token == Token.EOF)
                {
                    this.lexer.RollBack();
                    break;
                }
                if (token.Symbol == Symbols.PT_SEMICOLON)
                {
                    break;
                }
                // encountered the specified symbol, the analysis is complete, jump out
                if (token.Symbol != null && endSymbols.Contains(token.Symbol))
                {
                    break;
                }
                // ===========================================================
                // ==== parse begin
                // ===========================================================
                // expression of Token
                Expression tempExp = null;
                // value Operand
                if (token is ValueToken)
                {
                    var valueExpression = token as ValueToken;
                    if (valueExpression.Type == Tokens.ValueType.Number) tempExp = new LiteralExpression(valueExpression);
                    if (valueExpression.Type == Tokens.ValueType.String) tempExp = new LiteralExpression(valueExpression);
                    if (valueExpression.Type == Tokens.ValueType.Null) tempExp = new LiteralExpression(valueExpression);
                    if (valueExpression.Type == Tokens.ValueType.Boolean) tempExp = new LiteralExpression(valueExpression);
                }

                // identifier Operand
                if (token is IdentifierToken) tempExp = new NameExpression() { Identifier = token };
                // keywords should not appear here
                if (token is KeywordToken) throw this.InitParseException("Keyword appears in the wrong place ", token);
                // punctuator
                if (token is PunctuatorToken)
                {
                    var previousToken = this.lexer.Previous();
                    // I'm running, don't move me
                    var leftExpressionIsOperand = lastExpression != null &&
                        ((lastExpression is OperatorExpression lastOperatorExpression && lastOperatorExpression.IsOperand) ||
                            previousToken is IdentifierToken ||
                            previousToken is ValueToken);
                    var _operator = Operator.FromSymbols(token.Symbol, leftExpressionIsOperand);
                    if (_operator == null)
                    {
                        if (endSymbols.Contains(token.Symbol))
                        {
                            break;
                        }
                    }
                    else
                    {
                        tempExp = this.createExpression(currentScope, _operator, previousToken, lastExpression);
                        if (tempExp != null)
                        {
                            tempExp.Position = pos;
                        }
                    }
                }

                // Lambda Expression &&  Function Signature Type
                if (token.Symbol == Symbols.PT_LAMBDA)
                {
                    //if (lastExpression.Parent is BinaryExpression bin && bin.Operator == Operator.SetMember)
                    //{
                    rollBackLexer();
                    return ParseLamdaExpression(currentScope);
                    //}
                    //else
                    //{
                    //    rollBackLexer();
                    //    return ParseFunctionSignatureExpression(currentScope);
                    //}
                }
                if (tempExp == null) throw this.InitParseException("Invalid token {0} appears in expression", token);
                // ==============================================
                //
                // ==============================================
                // this is Operator
                if (tempExp is OperatorExpression operatorExpression)
                {
                    // this operator has secondary sybmol
                    if (operatorExpression.Operator.SecondarySymbols != null)
                    {
                        // this operator is function call
                        if (tempExp is FunctionCallExpression callExpression)
                        {
                            while (true)
                            {
                                // parse function call arguments
                                var argument = this.ParseExpression(currentScope, operatorExpression.Operator.SecondarySymbols, Symbols.PT_COMMA);
                                if (argument != null)
                                {
                                    callExpression.AddArgument(argument);
                                }
                                var last = this.lexer.Previous(1);
                                // If the symbol ends with a closing parenthesis, the parsing is complete
                                if (last.Symbol == Symbols.PT_RIGHTPARENTHESIS) break;
                            }
                        }
                        // expressions wrapped in parentheses
                        else if (tempExp is GroupExpression)
                        {
                            var scope = currentScope.CreateScope(ScopeType.GROUP);
                            // Parse() block, from here recursively parse expressions to minor symbols
                            while (true)
                            {
                                // parse aa : ddd
                                var exp = this.ParseExpression(scope, operatorExpression.Operator.SecondarySymbols, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                                if (exp != null) tempExp.AddNode(exp);
                                var last = this.lexer.Previous(1);
                                // If the symbol ends with a closing parenthesis, the parsing is complete
                                if (last.Symbol != Symbols.PT_COMMA) break;
                            }
                        }
                        // new Array expression
                        else if (tempExp is ArrayLiteralExpression arrayExpression)
                        {
                            // Parse new array object
                            while (true)
                            {
                                // parse function call arguments
                                var argument = this.ParseExpression(currentScope, operatorExpression.Operator.SecondarySymbols, Symbols.PT_COMMA);
                                if (argument != null) arrayExpression.AddNode(argument);
                                var last = this.lexer.Previous(1);
                                // If the symbol ends with a closing parenthesis, the parsing is complete
                                if (last.Symbol == Symbols.PT_RIGHTBRACKET) break;
                            }
                        }
                        // new Anonymous object expression
                        else if (tempExp is MapExpression constructExpression)
                        {
                            rollBackLexer();
                            return ParseObjectConstructor(currentScope);
                        }
                        // Array Index Access expression
                        else if (tempExp is GetElementExpression indexAccess)
                        {
                            // Parse[] block, from here recursively parse expressions to minor symbols
                            indexAccess.AddNode(this.ParseExpression(currentScope, indexAccess.Operator.SecondarySymbols));
                        }
                        //else if (tempExp is CastTypeExpression typeConvert)
                        //{
                        //    typeConvert.Typed = this.ParseExpression(currentScope, typeConvert.Operator.SecondarySymbols);
                        //}
                    }
                    /**
                     * =====================================================*===========================*
                     *  11 * 22 + 33 * 44 + 55  |  11 + 22 - 33 * 44 / -55  * (11 + 22) * 33 + 44 / 55  *
                     * =====================================================*===========================*
                     *             [+]          |           [-]             *              [+]          *
                     *             / \          |           / \             *              / \          *
                     *            /   \         |          /   \            *             /   \         *
                     *          [+]    55       |         /     \           *            /     \        *
                     *          / \             |        /       \          *           /       \       *
                     *         /   \            |      [+]       [/]        *         [*]       [/]     *
                     *        /     \           |      / \       / \        *         / \       / \     *
                     *       /       \          |     /   \     /   \       *        /   \     /   \    *
                     *     [*]       [*]        |    11   22  [*]   [-]     *      [+]   33   44   55   *
                     *     / \       / \        |             / \     \     *      / \                  *
                     *    /   \     /   \       |            /   \     55   *     /   \                 *
                     *   11   22   33   44      |           33   44         *    11   22                *
                     * =====================================================*===========================*
                     */
                    if (operatorExpression.Operator.Placement == OperatorPlacement.Prefix)
                    {
                        if (lastExpression != null)
                        {
                            if (lastExpression.Length >= 2) throw this.InitParseException("", token);
                            lastExpression.AddNode(tempExp);
                        }
                    }
                    else
                    {
                        // lastExpression is operand
                        if (lastExpression == null) throw this.InitParseException("", token);
                        Expression node;
                        if (lastExpression.Parent is OperatorExpression)
                        {
                            node = lastExpression;
                            while (true)
                            {
                                if (node.Parent == rootExpression) break;
                                var parent = node.Parent as OperatorExpression;
                                if (parent == null) throw this.InitParseException("", token);
                                if (operatorExpression.Precedence > parent.Precedence) break;
                                node = parent;
                            }
                        }
                        else
                        {
                            node = lastExpression;
                        }
                        var pNode = node.Parent;
                        node.Remove();
                        tempExp.AddNode(node);
                        if (tempExp is AssignmentExpression assignmentExpression)
                        {
                            var argument = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                            tempExp.AddNode(argument);
                            return optimize(assignmentExpression);
                        }
                        pNode.AddNode(tempExp);
                    }
                    // == operator the end ==
                }
                // token is not an operator, that is the operand
                else if (lastOperator != null)
                {
                    // Add operands to operator expressions
                    lastExpression.AddNode(tempExp);
                }
                // Is not an operator and the previous token is not an operator
                else
                {
                    if (rootExpression.Length > 0)
                        throw this.InitParseException("Invalid token {token} appears in expression {pos}", token);
                }
                lastOperator = (tempExp is OperatorExpression f) ? f.Operator : null;
                lastExpression = tempExp;
                if (rootExpression.Length == 0 && tempExp != null) rootExpression.AddNode(tempExp);
            }

            return rootExpression.Length > 0 ? optimize(rootExpression.Pop()) : null;
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





        /// <summary>
        /// var fmtString = `load ${num} of ${str}`;
        /// </summary>
        /// <param name="token"></param>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Expression expandStringTemplate(ValueToken token, Scope currentScope)
        {
            var exp = new BinaryExpression(Operator.Add);
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
                    var spread = new DeconstructionExpression();
                    spread.AddNode(value);
                    constructExpression.AddNode(spread);
                    break;
                }
                if (this.lexer.TestNext(Symbols.PT_RIGHTBRACE))
                {
                    break;
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
                    if (this.lexer.Previous(1).Symbol == Symbols.PT_RIGHTBRACE)
                    {
                        if (!(value is MapExpression || value is LambdaExpression))
                        {
                            // TODO
                            this.lexer.RollBack();
                        }
                    }
                    var newExp = new MapKeyValueExpression(varName, value);
                    constructExpression.AddNode(newExp);
                }
                //var token = this.lexer.Previous(1);
                var nameToken = new NameExpression();
                nameToken.Identifier = varName;
                constructExpression.AddNode(new MapKeyValueExpression(varName, nameToken));


                // Encountered comma break ;
                this.lexer.TestNext(Symbols.PT_COMMA);
                // Encountered closing parenthesis break ;
                if (this.lexer.TestNext(Symbols.PT_RIGHTBRACE)) break;
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



        private Expression ParseLamdaExpression(Scope currentScope)
        {
            var func = ParseFunction(null, currentScope, MemberAccess.Internal, FunctionFlags.Lambda);
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
            var declaration = new FunctionDeclaration(access, functionName, arguments, body, flags);
            return declaration;
        }
        private Statement ParseClass(Scope currentScope, Symbols access = null)
        {
            this.lexer.NextOfKind(Symbols.KW_CLASS);
            var className = this.lexer.NextOfKind<IdentifierToken>();
            if (this.lexer.TestNext(Symbols.KW_EXTENDS))
            {
                var parentClass = this.lexer.NextOfKind<IdentifierToken>();
                Console.WriteLine($"extends {parentClass}");
            }

            if (this.lexer.TestNext(Symbols.KW_IMPLEMENTS))
            {
                while (true)
                {
                    /* ] */
                    if (this.lexer.TestSymbol(Symbols.PT_LEFTBRACE)) break;
                    /* , */
                    if (this.lexer.TestNext(Symbols.PT_COMMA)) continue;
                    // typed
                    var _interface = this.lexer.NextOfKind<IdentifierToken>();
                    Console.WriteLine($"interface {_interface}");
                }
            }
            this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);


            return null;
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
            else if (token is KeywordToken && token.Symbol == Symbols.KW_CLASS)
            {
                // get function
                return ParseClass(currentScope, Symbols.KW_EXPORT);
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

            var FileExtension = ".as";
            var fullPath = Path.GetFullPath(Path.Combine(this.lexer.Directory, fileToken.Value));
            var extension = Path.GetExtension(fullPath);
            if (extension.ToLower() != FileExtension) fullPath = fullPath + FileExtension;
            if (!File.Exists(fullPath))
            {
                throw new CompilerException(fullPath, "Import file path not found ");
            }
            return new ImportDeclaration() { Module = module, File = fileToken, FullPath = fullPath };
        }




    }
}