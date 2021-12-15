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



        public Statement Parse(Scope currentScope = null, Token endToken = null)
        {
            if (endToken == null) endToken = Token.EOF;
            if (currentScope == null) currentScope = new Scope(this, null);
            Statement result = new BlockStatement();
            while (true)
            {
                var node = ParseStatement(currentScope, endToken);
                if (node == null) break;
            }
            return result;
        }


        private Statement ParseStatement(Scope currentScope, Symbols endSymbols)
        {
            var token = this.lexer.Next();
            if (token == null) throw new ParseException(this.lexer.FullPath, token, "Invalid keywords appear in ");
            if (token == Token.EOF) throw new ParseException(this.lexer.FullPath, token, "Unclosed scope ");
            if (token.Symbol == endSymbols) return null;
       
            if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            if (token.Symbol == Symbols.KW_EXPORT) return this.ParseExportStatement(currentScope);
            if (token.Symbol == Symbols.KW_FUNCTION) return this.ParseFunctionDeclaration(currentScope, Symbols.KW_SEALED);
            if (token.Symbol == Symbols.KW_VAR) return this.ParseVariableDeclaration(currentScope);
            if (token.Symbol == Symbols.KW_FOR) return this.ParseForBlock(currentScope);
            if (token.Symbol == Symbols.KW_IF) return this.ParseIfBlock(currentScope);


            return this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
        }



        /// <summary>
        /// 伪代码， 待修复
        /// </summary>
        /// <param name="currentScope"></param>
        /// <returns></returns>
        private Statement ParseForBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            // parse for initializer
            var initializer = this.ParseStatement(currentScope, Symbols.PT_SEMICOLON);
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for condition
            var condition = this.ParseStatement(currentScope, Symbols.PT_SEMICOLON);
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            // parse for incrementor
            var incrementor = this.ParseStatement(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            var nextToken = this.lexer.LookAtHead();
            AstNode body = null;
            if(nextToken.Symbol == Symbols.PT_LEFTBRACE)
            {
                // parse body block
                body = this.ParseStatement(currentScope, Symbols.PT_RIGHTBRACE);
                this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
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

            return null;
        }


        private Statement ParseIfBlock(Scope currentScope)
        {
            this.lexer.NextOfKind(Symbols.PT_LEFTPARENTHESIS);
            var condition = this.ParseExpression(currentScope, Symbols.PT_RIGHTPARENTHESIS);
            // Determine whether the body is single-line or multi-line 
            var nextToken = this.lexer.LookAtHead();
            AstNode body = null;
            if (nextToken.Symbol == Symbols.PT_LEFTBRACE)
            {
                // parse body block
                body = this.ParseStatement(currentScope, Symbols.PT_RIGHTBRACE);
                this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
            }
            else
            {
                // parse single expression
                body = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            }
            this.lexer.NextOfKind(Symbols.PT_RIGHTPARENTHESIS);
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
            var nextToken = this.lexer.LookAtHead();
            Statement statement = null;
            if(nextToken.Symbol == Symbols.KW_IF)
            {
                this.lexer.Next();
                statement = this.ParseIfBlock(currentScope);
            }
            else
            {
                if (nextToken.Symbol == Symbols.PT_LEFTBRACE)
                {
                    // parse body block
                    statement = this.ParseStatement(currentScope, Symbols.PT_RIGHTBRACE);
                    this.lexer.NextOfKind(Symbols.PT_LEFTBRACE);
                }
                else
                {
                    // parse single expression
                    statement = this.ParseExpression(currentScope, Symbols.PT_SEMICOLON);
                    this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
                }
            }

            return statement;
        }



        private Expression ParseExpression(Scope currentScope, params Symbols[] endSymbols)
        {
            while (true)
            {
                var token = this.lexer.Next();
                // over statement
 
                if (endSymbols.Contains(token.Symbol)) break;
                // value
                if (token is ValueToken)
                {

                }
                // identifier
                else if (token is IdentifierToken)
                {


                }
                // punctuator
                else if (token is PunctuatorToken)
                {


                }
                // operator
                else if (token is OperatorToken)
                {

                }
            }


            //if (token.Symbol == Symbols.KW_IMPORT) return this.ParseImport();
            //if (token.Symbol == Symbols.KW_EXPORT) return this.ParseImport();
            return null;
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
                if (nexttoken == Token.EOF) throw new LexerException(this.lexer.FullPath, nexttoken, "Parameter declaration is not closed ");
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
        private AstNode ParseFunction(IdentifierToken functionName, Scope currentScope, Symbols access = null)
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

            return this.Parse();
        }







        private Statement ParseFunctionDeclaration(Scope currentScope, Symbols access = null)
        {
            var functionName = this.lexer.NextOfKind<IdentifierToken>();
            // 校验 方法名是否有效
            var func = this.ParseFunction(functionName, currentScope, access);
            // 添加到作用域的声明
            return new EmptyStatement();
        }









        /// <summary>
        /// parse export target
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LexerException"></exception>
        private Statement ParseExportStatement(Scope currentScope)
        {
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
            var token = this.lexer.NextOfKind<StringToken>();
            this.lexer.NextOfKind(Symbols.PT_SEMICOLON);
            return new ImportDeclaration() { File = token };
        }

    }
}
