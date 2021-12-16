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
            return this.ParseBlockStatement(null, Symbols.KW_EOF);
        }





        public Statement ParseBlockStatement(Scope currentScope = null, Symbols endSymbol = null)
        {
            var scope = new Scope(this, currentScope);
            Statement result = new BlockStatement();
            while (true)
            {
                var token = this.lexer.LookAtHead();
                if (token.Symbol == endSymbol)
                {
                    this.lexer.Next();
                    break;
                }
                var node = ParseStatement(scope, endSymbol);
                result.AddNode(node);
                if (node == null) break;
            }
            return result;
        }


        private AstNode ParseStatement(Scope currentScope, Symbols endSymbols)
        {
            var token = this.lexer.Next();
            if (token == null) throw new ParseException(this.lexer.FullPath, token, "Invalid keywords appear in ");
            //if (token.Symbol == Symbols.KW_EOF) throw new ParseException(this.lexer.FullPath, token, "Unclosed scope ");
            if (token.Symbol == endSymbols) return null;
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, Symbols.KW_SEALED);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);
            if (token.Symbol == Symbols.KW_CONTINUE) return new ContinueStatement();

            this.lexer.RollBack();
            return this.ParseExpression(currentScope, Symbols.PT_SEMICOLON, endSymbols);
        }



        /// <summary>
        /// 伪代码， 待修复
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseForBlock(Scope currentScope)
        {
            //this.lexer.NextOfKind(Symbols.KW_FOR);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse for initializer
            var initializer = this.ParseStatement(currentScope, Symbols.PT_SEMICOLON);
            //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for condition
            var condition = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
            //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for incrementor
            var incrementor = this.ParseStatement(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            var nextToken = this.lexer.LookAtHead();
            AstNode body = null;
            if (nextToken.Symbol == Symbols.PT_LEFTBRACE)
            {
                // parse body block
                this.lexer.Next();
                body = this.ParseBlockStatement(currentScope, Symbols.PT_RIGHTBRACE);
                //this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
            }
            else
            {
                // parse single expression
                body = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            }
            // parse for body
            return new ForStatement()
            {
                Condition = condition,
                Incrementor = incrementor,
                Initializer = initializer,
                Body = body
            };

        }








        private Statement ParseVariableDeclaration(Scope currentScope)
        {
            var varName = this.lexer.NextOfKind<IdentifierToken>();
            this.lexer.NextOfKind(Symbols.OP_ASSIGNMENT);

            var exp = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);

            var expression = new VariableDeclaration();
            expression.Variable = varName;
            expression.Init = exp;
            //this.lexer.NextOfKind(Symbols.KW_VAR);
            return expression;
        }


        private Statement ParseIfBlock(Scope currentScope)
        {
            //this.lexer.NextOfKind(Symbols.KW_IF);
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            var condition = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            var nextToken = this.lexer.LookAtHead();
            AstNode body = null;
            if (nextToken.Symbol == Symbols.PT_LEFTBRACE)
            {
                // parse body block
                body = this.ParseBlockStatement(currentScope, Symbols.PT_RIGHTBRACE);
                //this.lexer.NextOfKind(Symbols.PT_RIGHTBRACE);
            }
            else
            {
                // parse single-line expression
                body = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            }
            //this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
            var ifStatement = new IfStatement()
            {
                Condition = condition,
                Body = body,
            };
            nextToken = this.lexer.LookAtHead();
            // 处理 else
            if (nextToken.Symbol == Symbols.KW_ELSE)
            {

                ifStatement.Else = this.ParseElseBlock(currentScope);

            }
            return ifStatement;
        }


        private Statement ParseElseBlock(Scope currentScope)
        {

            this.lexer.NextOfKind(Symbols.KW_ELSE);
            var nextToken = this.lexer.LookAtHead();
            BlockStatement block = new BlockStatement();
            if (nextToken.Symbol == Symbols.KW_IF)
            {
                this.lexer.Next();
                var statement = this.ParseIfBlock(currentScope);
                block.AddNode(statement);
            }
            else
            {
                if (nextToken.Symbol == Symbols.PT_LEFTBRACE)
                {
                    // parse body block
                    var statement = this.ParseStatement(currentScope, Symbols.PT_RIGHTBRACE);
                    block.AddNode(statement);
                    this.lexer.NextOfKind(Symbols.PT_RIGHTBRACE);
                }
                else
                {
                    // parse single expression
                    var expression = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                    block.AddNode(expression);
                    //this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                }
            }

            return block;
        }



        private Expression ParseExpression(Scope currentScope, params Symbols[] endSymbols)
        {
            Expression expression = null;


            while (true)
            {
                var token = this.lexer.Next();
                // over statement

                if (endSymbols.Contains(token.Symbol))
                {
                    break;
                }
                // value
                if (token is ValueToken)
                {

                    if (expression is BinaryExpression exp)
                    {
                        if(exp.Right == null)
                        {
                            exp.Right = new Expression();
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
                // identifier
                else if (token is IdentifierToken)
                {

                    expression = new NameExpression() { Identifier = token };
                }

                // operator
                else if (token is OperatorToken)
                {
                    Console.WriteLine(token);
                    var binary = new BinaryExpression();
                    binary.Left = expression;
                    binary.Operator = token;

                    expression = binary;

                }

                // punctuator
                else if (token is PunctuatorToken)
                {
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
                        var paramlist = this.ParseCallFunctionExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);

                        var exp = new CallExpression();
                        exp.Arguments = paramlist;
                        exp.Base = expression;

                        expression = exp;
                    }
                }

                // keyword
                else if (token is KeywordToken)
                {
                    if (token.Symbol == Symbols.KW_BREAK)
                    {
                        this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                        return new BreakStatement();
                    }
                    if (token.Symbol == Symbols.KW_CONTINUE)
                    {
                        this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                        return new ContinueStatement();
                    }

                    if (token.Symbol == Symbols.KW_RETURN)
                    {
                        var exp = this.ParseExpression(currentScope,Symbols.PT_SEMICOLON);

                        return new ReturnStatement(exp);
                    }


                    Console.WriteLine();
                }
            }


            //if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            //if (token.Symbol == Symbols.KW_EXPORT) return this.ParseImport();
            return expression;
        }



        private List<Expression> ParseCallFunctionExpression(Scope currentScope, Symbols endSymbol)
        {
            List<Expression> expressions = new List<Expression>();
            //this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            while (this.lexer.LookAtHead().Symbol != endSymbol)
            {
                var exp = this.ParseExpression(currentScope, Symbols.PT_COMMA, endSymbol);
                expressions.Add(exp);
                var nextToken = this.lexer.LookAtHead();

                if (nextToken.Symbol == Symbols.PT_COMMA) this.lexer.Next();
                if (nextToken.Symbol == Symbols.PT_SEMICOLON)
                {
                    break;
                }
            }
            //this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
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
                if (nexttoken.Symbol == Symbols.KW_EOF) throw new LexerException(this.lexer.FullPath, nexttoken, "Parameter declaration is not closed ");
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
            this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);

            // create function scope
            var scope = new Scope(this, currentScope);

            // declare arguments variable
            scope.DeclareVariable(arguments);

            // parse function body
            var body = this.ParseFunctionBody(scope);

            return new FunctionDeclaration()
            {
                Access = access,
                Body = body,
                Identifier = functionName,
                Parameters = arguments,
                Type = returnType
            };
        }


        /// <summary>
        /// parse function body block after “{”
        /// </summary>
        /// <param name="parentScope"></param>
        /// <returns></returns>
        private Statement ParseFunctionBody(Scope currentScope)
        {

            return this.ParseBlockStatement(currentScope, Symbols.PT_RIGHTBRACE);
        }







        private Statement ParseFunctionDeclaration(Scope currentScope, Symbols access = null)
        {
            //this.lexer.NextOfKind(Symbols.KW_FUNCTION);
            var functionName = this.lexer.NextOfKind<IdentifierToken>();
            // 校验 方法名是否有效
            var func = this.ParseFunction(functionName, currentScope, access);

            return func;

            // 添加到作用域的声明
            // return new EmptyStatement();
        }









        /// <summary>
        /// parse export target
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LexerException"></exception>
        private Statement ParseExportStatement(Scope currentScope)
        {
            //this.lexer.NextOfKind(Symbols.KW_EXPORT);
            var token = this.lexer.LookAtHead();
            if (token is KeywordToken && token.Symbol == Symbols.KW_FUNCTION)
            {
                this.lexer.NextOfKind(Symbols.KW_FUNCTION);
                return ParseFunctionDeclaration(currentScope, Symbols.KW_EXPORT);
            }
            throw new LexerException(this.lexer.FullPath, token, "Wrong export target.");
        }


        /// <summary>
        /// parse import module
        /// </summary>
        /// <returns></returns>
        private Statement ParseImport()
        {
            //this.lexer.NextOfKind(Symbols.KW_IMPORT);
            var token = this.lexer.NextOfKind<StringToken>();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            return new ImportDeclaration() { File = token };
        }

    }
}
