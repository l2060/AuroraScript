using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Common;
using AuroraScript.Exceptions;
using AuroraScript.Tokens;
using System.Diagnostics;

namespace AuroraScript.Analyzer
{
    public class AuroraParser
    {
        public AuroraCompiler Compiler { get; private set; }
        public AuroraLexer lexer { get; private set; }
        public BlockStatement root { get; private set; }
        public AuroraParser(AuroraCompiler compiler, AuroraLexer lexer)
        {
            this.lexer = lexer;
            this.Compiler = compiler;
            var scope = new Scope(this, null);
            this.root = new BlockStatement(scope);
        }

        public AstNode Parse()
        {
            while (true)
            {
                if (this.lexer.TestNext(Symbols.KW_EOF)) break;
                var node = ParseStatement(this.root.Scope);
                if (node != null) this.root.AddNode(node);
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

            if (token.Symbol == Symbols.PT_SEMICOLON) return null;
            if (token.Symbol == Symbols.PT_LEFTBRACE) return this.ParseBlock(currentScope);
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_DECLARE) return this.ParseDeclare(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_CONST) return this.ParseVariableDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_ENUM) return this.ParseEnumDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_TYPE) return this.ParseTypeDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope, Symbols.KW_INTERNAL);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);
            if (token.Symbol == Symbols.KW_CONTINUE) return this.ParseContinueStatement(currentScope);
            if (token.Symbol == Symbols.KW_BREAK) return this.ParseBreakStatement(currentScope);
            if (token.Symbol == Symbols.KW_RETURN) return this.ParseReturnStatement(currentScope);
            var statement = new Statement();
            var expression = this.ParseExpression(currentScope);
            statement.AddNode(expression);
            return statement;
        }


        /**
         * parse extend type declare 
         */
        private Statement ParseDeclare(Scope currentScope, Symbols access = null)
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
                this.lexer.NextOfKind(Symbols.PT_COLON);
                // typed
                var typeds = this.ParseFunctionReturnTypeds();
                // ;
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                var declaration = new FunctionDeclaration()
                {
                    Access = access,
                    Identifier = funcName,
                    Parameters = arguments,
                    Typeds = typeds,
                };
                currentScope.DeclareFunction(declaration);
                return declaration;
            }
            throw this.InitParseException("The Declare keyword only allows the declaration of external methods ", this.lexer.LookAtHead());
        }

        /// <summary>
        /// parse Object Typed
        /// </summary>
        /// <returns></returns>
        private ObjectType ParseObjectType()
        {
            var basicType = this.lexer.TestNextOfKind<IdentifierToken>();
            if (this.lexer.TestNext(Symbols.PT_LEFTBRACKET))
            {
                this.lexer.NextOfKind(Symbols.PT_RIGHTBRACKET);
                return new ArrayType(basicType);
            }
            return new ObjectType(basicType);
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
            var declaration = new EnumDeclaration() { Elements = elements, Identifier = enumName };


            return declaration;
        }


        /// <summary>
        /// parse typed define
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private Statement ParseTypeDeclaration(Scope currentScope, Symbols access = null)
        {
            this.lexer.NextOfKind(Symbols.KW_TYPE);
            var name = this.lexer.NextOfKind<IdentifierToken>();
            this.lexer.NextOfKind(Symbols.OP_ASSIGNMENT);
            var typed = this.ParseObjectType();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            var declaration = new TypeDeclaration() { Identifier = name, Typed = typed, Access = access };

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
            var scope = new Scope(this, currentScope);
            var result = new BlockStatement(scope);
            while (true)
            {
                var token = this.lexer.LookAtHead();
                // Check for the end brace (}).
                if (token.Symbol == Symbols.PT_RIGHTBRACE) break;
                // Parse a single statement.
                result.AddNode(this.ParseStatement(scope));
            }
            // Consume the end brace.
            this.lexer.NextOfKind(Symbols.PT_RIGHTBRACE);
            return result;
        }




        /// <summary>
        /// parse for block
        /// starting with “for”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseForBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_FOR);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse for initializer
            var initializer = this.ParseStatement(currentScope);
            // parse for condition
            var condition = this.ParseExpression(currentScope);
            // parse for incrementor
            var incrementor = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            var body = this.ParseStatement(currentScope);
            // parse for body
            return new ForStatement()
            {
                Condition = condition,
                Incrementor = incrementor,
                Initializer = initializer,
                Body = body
            };

        }







        /// <summary>
        /// parse variable declaration
        /// starting with “var”
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseVariableDeclaration(Scope currentScope, Symbols access = null)
        {
            var variables = new VariableDeclaration() { Access = access };
            // const
            if (this.lexer.TestNext(Symbols.KW_CONST))
            {
                variables.IsConst = true;
            }
            else if (this.lexer.TestNext(Symbols.KW_VAR))
            {
                variables.IsConst = false;
            }
            else
            {
                throw this.InitParseException("Variable declaration should be placed after var const ", this.lexer.LookAtHead());
            }

            var varName = this.lexer.NextOfKind<IdentifierToken>();
            variables.Variables.Add(varName);
            // next token is : define var typed
            if (this.lexer.TestNext(Symbols.PT_COLON))
            {
                variables.Typed = this.ParseObjectType();
            }
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
                    variables.Initializer = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                    break;
                }
                // ,
                else if (nextToken.Symbol == Symbols.PT_COMMA)
                {
                    varName = this.lexer.NextOfKind<IdentifierToken>();
                    variables.Variables.Add(varName);
                }
                else
                {
                    throw this.InitParseException("Invalid keywords appear in var declaration.", nextToken);
                }
            }
            // define variables
            currentScope.DefineVariable(variables);
            return variables;
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
            var ifStatement = new IfStatement() { Condition = condition };
            ifStatement.Body = body;
            var nextToken = this.lexer.LookAtHead();
            if (nextToken.Symbol == Symbols.KW_ELSE)
            {
                // parse else
                ifStatement.Else = this.ParseElseBlock(currentScope);
            }
            return ifStatement;
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
            if (this.lexer.TestNext(Symbols.KW_IF))
            {
                var statement = this.ParseIfBlock(currentScope);
                block.AddNode(statement);
            }
            else
            {
                var expression = this.ParseStatement(currentScope);
                block.AddNode(expression);
            }
            return block;
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
            Expression rootExpression = new Expression();
            Expression lastExpression = null;
            Operator lastOperator = null;
            while (true)
            {
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
                if (token is ValueToken) tempExp = new ValueExpression(token);
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
                        tempExp = this.createExpression(_operator, previousToken);
                    }
                }
                if (tempExp == null) throw this.InitParseException("Invalid token {token} appears in expression {pos}", token);
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
                                if (argument != null) callExpression.Arguments.Add(argument);
                                var last = this.lexer.Previous(1);
                                // If the symbol ends with a closing parenthesis, the parsing is complete 
                                if (last.Symbol == Symbols.PT_RIGHTPARENTHESIS) break;
                            }
                        }
                        // expressions wrapped in parentheses 
                        else if (tempExp is GroupExpression)
                        {
                            // Parse() block, from here recursively parse expressions to minor symbols 
                            tempExp = this.ParseExpression(currentScope, operatorExpression.Operator.SecondarySymbols);
                            if (tempExp is OperatorExpression opexp)
                            {
                                opexp.Upgrade(Operator.Grouping);
                            }
                        }
                        // new Array expression 
                        else if (tempExp is ArrayExpression arrayExpression)
                        {
                            // Parse new array object 
                            while (true)
                            {
                                // parse function call arguments
                                var argument = this.ParseExpression(currentScope, operatorExpression.Operator.SecondarySymbols, Symbols.PT_COMMA);
                                if (argument != null) arrayExpression.Elements.Add(argument);
                                var last = this.lexer.Previous(1);
                                // If the symbol ends with a closing parenthesis, the parsing is complete 
                                if (last.Symbol == Symbols.PT_RIGHTBRACKET) break;
                            }
                        }
                        // Array Index Access expression
                        else if (tempExp is ArrayAccessExpression indexAccess)
                        {
                            // Parse[] block, from here recursively parse expressions to minor symbols 
                            indexAccess.Index = this.ParseExpression(currentScope, Symbols.PT_RIGHTBRACKET);
                        }
                    }
                    /**
                     * ==================================================== *
                     *  11 * 22 + 33 * 44 + 55  |  11 + 22 - 33 * 44 / -55  *
                     * ==================================================== *
                     *             [+]          |           [-]             *
                     *             / \          |           / \             *
                     *            /   \         |          /   \            *
                     *          [+]    55       |         /     \           *
                     *          / \             |        /       \          *
                     *         /   \            |      [+]       [/]        *
                     *        /     \           |      / \       / \        *
                     *       /       \          |     /   \     /   \       *
                     *     [*]       [*]        |    11   22  [*]   [-]     *
                     *     / \       / \        |             / \     \     *
                     *    /   \     /   \       |            /   \     55   *
                     *   11   22   33   44      |           33   44         *
                     * =====================================================*
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
                    if (rootExpression.ChildNodes.Count() > 0)
                        throw this.InitParseException("Invalid token {token} appears in expression {pos}", token);
                }
                lastOperator = (tempExp is OperatorExpression f) ? f.Operator : null;
                lastExpression = tempExp;
                if (rootExpression.ChildNodes.Count() == 0) rootExpression.AddNode(tempExp);
            }


            return rootExpression.Length > 0 ? rootExpression.Pop() : null;
        }








        private Expression createExpression(Operator _operator, Token previousToken)
        {
            // Assignment Expression
            if (_operator == Operator.Assignment) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundAdd) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundSubtract) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundDivide) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundModulo) return new AssignmentExpression(_operator);
            if (_operator == Operator.CompoundMultiply) return new AssignmentExpression(_operator);

            // new expression
            if (_operator == Operator.New) return new BinaryExpression(_operator);

            // function call expression
            if (_operator == Operator.FunctionCall && !(previousToken is PunctuatorToken)) return new FunctionCallExpression(_operator);
            // Grouping expression
            if (_operator == Operator.Grouping  /* && previousToken is PunctuatorToken */ ) return new GroupExpression(_operator);
            // member access expression
            if (_operator == Operator.Array) return new ArrayExpression(_operator);
            // member index expression
            if (_operator == Operator.Index) return new ArrayAccessExpression(_operator);
            // member access expression
            if (_operator == Operator.MemberAccess) return new MemberAccessExpression(_operator);

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

            if (_operator == Operator.LogicalOr) return new BinaryExpression(_operator);
            if (_operator == Operator.Modulo) return new BinaryExpression(_operator);
            if (_operator == Operator.Multiply) return new BinaryExpression(_operator);
            if (_operator == Operator.NotEqual) return new BinaryExpression(_operator);
            if (_operator == Operator.SignedRightShift) return new BinaryExpression(_operator);
            if (_operator == Operator.Subtract) return new BinaryExpression(_operator);

            // Prefix expression
            if (_operator == Operator.BitwiseNot) return new PrefixUnaryExpression(_operator);
            if (_operator == Operator.Minus) return new PrefixUnaryExpression(_operator);
            if (_operator == Operator.PreDecrement) return new PrefixUnaryExpression(_operator);
            if (_operator == Operator.PreIncrement) return new PrefixUnaryExpression(_operator);
            if (_operator == Operator.TypeOf) return new PrefixUnaryExpression(_operator);
            if (_operator == Operator.LogicalNot) return new PrefixUnaryExpression(_operator);
            // Postfix expression
            if (_operator == Operator.PostDecrement) return new PostfixExpression(_operator);
            if (_operator == Operator.PostIncrement) return new PostfixExpression(_operator);
            return null;
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
                var varname = this.lexer.NextOfKind<IdentifierToken>();
                this.lexer.NextOfKind(Symbols.PT_COLON);
                var typed = this.ParseObjectType();
                Expression defaultValue = null;
                // argument default value 
                if (this.lexer.TestNext(Symbols.OP_ASSIGNMENT))
                {
                    defaultValue = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                }
                var declaration = new ParameterDeclaration()
                {
                    Variable = varname,
                    DefaultValue = defaultValue,
                    Typed = typed
                };
                arguments.Add(declaration);
                // Encountered comma break ;
                this.lexer.TestNext(Symbols.PT_COMMA);
                // Encountered closing parenthesis break ;
                if (this.lexer.TestNext(Symbols.PT_RIGHTPARENTHESIS)) break;
            }
            return arguments;
        }





        /// <summary>
        /// parse function block
        /// starting with “(” arguments declarations
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private FunctionDeclaration ParseFunction(IdentifierToken functionName, Scope currentScope, Symbols access = null)
        {
            // next token (
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse arguments
            var arguments = this.ParseFunctionArguments(currentScope);
            // next token : Typed
            this.lexer.NextOfKind(Symbols.PT_COLON);
            var typeds = this.ParseFunctionReturnTypeds();
            // create function scope
            var scope = new Scope(this, currentScope);
            // declare arguments variable
            scope.DeclareVariable(arguments);
            // parse function body
            var body = this.ParseBlock(scope);

            var declaration = new FunctionDeclaration()
            {
                Access = access,
                Body = body,
                Identifier = functionName,
                Parameters = arguments,
                Typeds = typeds
            };
            // define functions in scope 
            currentScope.DefineFunction(declaration);
            return declaration;
        }


        private List<ObjectType> ParseFunctionReturnTypeds()
        {
            List<ObjectType> result = new List<ObjectType>();
            Boolean m = false;
            if (this.lexer.TestAtHead<IdentifierToken>())
            {
                var returnType = this.ParseObjectType();
                result.Add(returnType);
            }
            else
            {
                m = true;
                /* [ */
                this.lexer.NextOfKind(Symbols.PT_LEFTBRACKET);
                while (true)
                {
                    /* ] */
                    if (this.lexer.TestNext(Symbols.PT_RIGHTBRACKET)) break;
                    /* , */
                    if (this.lexer.TestNext(Symbols.PT_COMMA)) continue;
                    // typed
                    var typed = this.ParseObjectType();
                    result.Add(typed);
                }
            }

            if (m && result.Count == 0)
            {
                throw this.InitParseException("定义多返回类型时，最少指定一个返回类型", this.lexer.Previous());
            }
            return result;
        }




        /// <summary>
        /// analyze function declarations 
        /// starting with “function” 
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private Statement ParseFunctionDeclaration(Scope currentScope, Symbols access = null)
        {
            this.lexer.NextOfKind(Symbols.KW_FUNCTION);
            var functionName = this.lexer.NextOfKind<IdentifierToken>();
            // 校验 方法名是否有效
            return this.ParseFunction(functionName, currentScope, access);
        }

        /// <summary>
        /// parse export object
        /// starting with “export”
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LexerException"></exception>
        private Statement ParseExportStatement(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_EXPORT);
            var token = this.lexer.LookAtHead();
            if (token is KeywordToken && token.Symbol == Symbols.KW_FUNCTION)
            {
                // function
                return ParseFunctionDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_VAR)
            {
                // var
                return ParseVariableDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_CONST)
            {
                // const
                return ParseVariableDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_ENUM)
            {
                // enum
                return ParseEnumDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_TYPE)
            {
                // type
                return ParseTypeDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            else if (token is KeywordToken && token.Symbol == Symbols.KW_DECLARE)
            {
                // type
                return ParseDeclare(currentScope, Symbols.KW_EXPORT);
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
            Token module = null;
            Token fileToken = this.lexer.TestNextOfKind<StringToken>();
            if (fileToken != null)
            {
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            }
            else
            {
                module = this.lexer.NextOfKind<IdentifierToken>();
                this.lexer.NextOfKind(Symbols.KW_FROM);
                fileToken = this.lexer.NextOfKind<StringToken>();
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            }


            // import ast
            var moduleAst = this.Compiler.buildAst(fileToken.Value, this.lexer.Directory);
            // 这个地方不应该由这里加载引入模块，而是由其他线程加载。
            //最终由link-module 链接起来 



            return new ImportDeclaration() { Module = module, File = fileToken };
        }

    }
}
