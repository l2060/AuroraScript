using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Exceptions;
using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Analyzer
{
    internal class AuroraParser
    {
        public AuroraLexer lexer { get; private set; }
        public AuroraParser(AuroraLexer lexer)
        {
            this.lexer = lexer;
        }

        public AstNode Parse()
        {
            var scope = new Scope(this, null);
            Statement result = new BlockStatement(scope);
            while (true)
            {
                var token = this.lexer.LookAtHead();
                if (token.Symbol == Symbols.KW_EOF)
                {
                    this.lexer.Next();
                    break;
                }
                var node = ParseStatement(scope);
                result.AddNode(node);
                if (node == null) break;
            }
            return result;
        }



        /// <summary>
        /// parse single statement or block statement
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
            if (token.Symbol == Symbols.PT_LEFTBRACE) return this.ParseBlock(currentScope);
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, Symbols.KW_SEALED);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);
            if (token.Symbol == Symbols.KW_CONTINUE) return this.ParseContinueStatement(currentScope);
            if (token.Symbol == Symbols.KW_BREAK) return this.ParseBreakStatement(currentScope);
            if (token.Symbol == Symbols.KW_RETURN) return this.ParseReturnStatement(currentScope);
            //this.lexer.RollBack();
            var statement = new Statement();
            statement.AddNode(this.ParseExpression(currentScope));
            return statement;
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
            var statement = new ContinueStatement();

            return statement;
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
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseForBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_FOR);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse for initializer
            var initializer = this.ParseStatement(currentScope);
            //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for condition
            var condition = this.ParseExpression(currentScope);
            //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for incrementor
            var incrementor = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            //var nextToken = this.lexer.LookAtHead();
            AstNode body = this.ParseBlock(currentScope);
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
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseVariableDeclaration(Scope currentScope)
        {
            Expression initializer = null;
            this.lexer.NextOfKind(Symbols.KW_VAR);
            var varName = this.lexer.NextOfKind<IdentifierToken>();
            List<Token> varNames = new List<Token>() { varName };

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
                    initializer = this.ParseExpression(currentScope);
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
                    throw this.InitLexerException("Invalid keywords appear in var declaration.", nextToken);
                }

            }
            //this.lexer.NextOfKind(Symbols.OP_ASSIGNMENT);
            var expression = new VariableDeclaration();
            expression.Variables.AddRange(varNames);
            expression.Initializer = initializer;
            //this.lexer.NextOfKind(Symbols.KW_VAR);
            return expression;
        }


        private Exception InitLexerException(String message, Token token)
        {
            return new ParseException(this.lexer.FullPath, token, message);
        }





        /// <summary>
        /// parse if block
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
            ifStatement.Body.Add(body);
            var nextToken = this.lexer.LookAtHead();
            // 处理 else
            if (nextToken.Symbol == Symbols.KW_ELSE)
            {
                ifStatement.Else = this.ParseElseBlock(currentScope);
            }
            return ifStatement;
        }

        /// <summary>
        /// parse else block
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseElseBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_ELSE);
            var nextToken = this.lexer.LookAtHead();
            BlockStatement block = new BlockStatement(currentScope);
            if (nextToken.Symbol == Symbols.KW_IF)
            {
                this.lexer.Next();
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
        /// parse an expression ending in endSymbols; 
        /// </summary>
        /// <param name="currentScope"></param>
        /// <param name="endSymbols"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Expression ParseExpression(Scope currentScope, params Symbols[] endSymbols)
        {
            OperatorExpression currentOperator = null;
            Expression expression = null;
            while (true)
            {
                var token = this.lexer.Next();
                // over statement
                if (token.Symbol == Symbols.PT_SEMICOLON)
                {
                    break;
                }
                if (token.Symbol != null && endSymbols.Contains(token.Symbol))
                {
                    break;
                }

                // value Operand
                if (token is ValueToken)
                {

                    if (expression is BinaryExpression exp)
                    {
                        if (exp.Right == null)
                        {
                            exp.Right = new ValueExpression(token);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        expression = new ValueExpression(token);
                    }

                }
                // identifier  Operand
                else if (token is IdentifierToken)
                {
                    var exp = new NameExpression() { Identifier = token };

                    if (expression == null)
                    {
                        expression = new NameExpression() { Identifier = token };
                    }
                    else if (expression is BinaryExpression exp2)
                    {
                        exp2.Right = exp;
                    }
                }

                // operator
                //else if (token is OperatorToken)
                //{

                //    var binary = new BinaryExpression();
                //    binary.Left = expression;
                //    binary.Operator = token;
                //    expression = binary;
                //}

                // punctuator
                else if (token is PunctuatorToken)
                {

                    //  Operator newOperator = OperatorFromToken(this.nextToken, postfixOrInfix: this.expressionState == ParserExpressionState.Operator);
                    var previousToken = this.lexer.Previous();

                    var _operator = Operator.FromSymbols(token.Symbol, previousToken is IdentifierToken || previousToken is ValueToken);

                    if(_operator == null)
                    {
                        if (endSymbols.Contains(token.Symbol))
                        {
                            break;
                        }
                        if (endSymbols.Contains(Symbols.PT_SEMICOLON))
                        {
                            break;
                        }
                        throw this.InitLexerException("Invalid token appears in expression", token);
                    }

                    if (_operator == Operator.FunctionCall)
                    {

                    }




                    OperatorExpression curExpression = null;
                    if((_operator.placement & OperatorPlacement.Prefix) == OperatorPlacement.Prefix)
                    {
                        curExpression = new PrefixUnaryExpression(_operator);
                    }else if((_operator.placement & OperatorPlacement.Binary) == OperatorPlacement.Binary)
                    {
                        curExpression = new BinaryExpression(_operator);
                    }
                    else if ((_operator.placement & OperatorPlacement.Postfix) == OperatorPlacement.Postfix)
                    {
                        curExpression = new PostfixExpression(_operator);
                    }

                    if(curExpression == null)
                    {
                        throw new Exception("asdad");
                    }


                    expression = new OperatorExpression(_operator);




                    

                    //currentOperator = expression;
                    Console.WriteLine(token);




                    if (_operator == Operator.MemberAccess)
                    {
                        var memberIdentifier = this.lexer.NextOfKind<IdentifierToken>();
                        if (expression == null) throw new InvalidOperationException();
                        var member = new MemberExpression(memberIdentifier, expression);
                        expression = member;
                    }
                    if (_operator == Operator.FunctionCall)
                    {
                        var paramlist = this.ParseCallFunctionExpression(currentScope);
                        var exp = new CallExpression();
                        exp.Arguments = paramlist;
                        exp.Base = expression;
                        expression = exp;
                    }


                    // is object member
                    if (token.Symbol == Symbols.PT_DOT)
                    {
                        var memberIdentifier = this.lexer.NextOfKind<IdentifierToken>();
                        if (expression == null) throw new InvalidOperationException();
                        var member = new MemberExpression(memberIdentifier, expression);
                        expression = member;
                    }
                    // is function call 
                    if (token.Symbol == Symbols.PT_LEFTPARENTHESIS)
                    {
                        var paramlist = this.ParseCallFunctionExpression(currentScope);
                        var exp = new CallExpression();
                        exp.Arguments = paramlist;
                        exp.Base = expression;
                        expression = exp;
                    }
                }

                // keyword
                else if (token is KeywordToken)
                {
                    Console.WriteLine(token);
                }

                if (expression == null) throw this.InitLexerException("Invalid keywords appear in .", token);
            }



            return expression;
        }


        /// <summary>
        /// parse function call
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private List<Expression> ParseCallFunctionExpression(Scope currentScope)
        {
            List<Expression> expressions = new List<Expression>();
            //this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            while (this.lexer.LookAtHead().Symbol != Symbols.PT_RIGHTPARENTHESIS)
            {
                var exp = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                expressions.Add(exp);
                var nextToken = this.lexer.LookAtHead();

                if (this.lexer.Previous().Symbol == Symbols.PT_RIGHTPARENTHESIS)
                {
                    break;
                }
                if (nextToken.Symbol == Symbols.PT_COMMA) this.lexer.Next();
                if (nextToken.Symbol == Symbols.PT_RIGHTPARENTHESIS)
                {
                    this.lexer.Next();
                    break;
                }
            }
            return expressions;
        }




        /// <summary>
        /// Parse Function Arguments
        /// start => fs: number, name: string)
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
                var varname = this.lexer.NextOfKind<IdentifierToken>();
                this.lexer.NextOfKind(Symbols.PT_COLON);
                var typed = this.lexer.NextOfKind<TypedToken>();
                var nexttoken = this.lexer.LookAtHead();
                Expression defaultValue = null;
                if (nexttoken != null && nexttoken.Symbol == Symbols.OP_ASSIGNMENT)
                {
                    this.lexer.Next();
                    defaultValue = this.ParseExpression(currentScope, Symbols.PT_COMMA, Symbols.PT_RIGHTPARENTHESIS);
                }
                var declaration = new ParameterDeclaration()
                {
                    Variable = varname,
                    DefaultValue = defaultValue,
                    Typed = typed
                };
                arguments.Add(declaration);
                nexttoken = this.lexer.LookAtHead();
                if (nexttoken.Symbol == Symbols.KW_EOF)
                {
                    throw this.InitLexerException("Parameter declaration is not closed ", nexttoken);
                }
                if (nexttoken is PunctuatorToken && nexttoken.Symbol == Symbols.PT_COMMA)
                {
                    // Drop the comma
                    this.lexer.Next();
                }
                if (nexttoken is PunctuatorToken && nexttoken.Symbol == Symbols.PT_RIGHTPARENTHESIS)
                {
                    this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
                    // Parameter parsing is complete 
                    break;
                }
            }
            return arguments;
        }





        /// <summary>
        /// parse function block
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parentScope"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        private FunctionDeclaration ParseFunction(IdentifierToken functionName, Scope currentScope, Symbols access = null)
        {
            // 验证 下一个Token (
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);

            // 解析方法参数
            var arguments = this.ParseFunctionArguments(currentScope);

            // 验证参数
            // ........

            // 验证 下一个Token :
            this.lexer.NextOfKind(Symbols.PT_COLON);

            // 验证 下一个Token Typed
            var returnType = this.lexer.NextOfKind<TypedToken>();

            // 验证 下一个Token {
            //this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);

            // create function scope
            var scope = new Scope(this, currentScope);

            // declare arguments variable
            scope.DeclareVariable(arguments);

            // parse function body
            var body = this.ParseBlock(scope);

            return new FunctionDeclaration()
            {
                Access = access,
                Body = body,
                Identifier = functionName,
                Parameters = arguments,
                Type = returnType
            };
        }

        private Statement ParseFunctionDeclaration(Scope currentScope, Symbols access = null)
        {
            this.lexer.NextOfKind(Symbols.KW_FUNCTION);
            var functionName = this.lexer.NextOfKind<IdentifierToken>();
            // 校验 方法名是否有效
            return this.ParseFunction(functionName, currentScope, access);
        }

        /// <summary>
        /// parse export target
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LexerException"></exception>
        private Statement ParseExportStatement(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.KW_EXPORT);
            var token = this.lexer.LookAtHead();
            if (token is KeywordToken && token.Symbol == Symbols.KW_FUNCTION)
            {
                //this.lexer.NextOfKind(Symbols.KW_FUNCTION);
                return ParseFunctionDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            throw this.InitLexerException("Invalid keywords appear in export declaration .", token);
        }


        /// <summary>
        /// parse import module
        /// </summary>
        /// <returns></returns>
        private Statement ParseImport()
        {
            this.lexer.NextOfKind(Symbols.KW_IMPORT);
            var token = this.lexer.NextOfKind<StringToken>();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            return new ImportDeclaration() { File = token };
        }

    }
}
