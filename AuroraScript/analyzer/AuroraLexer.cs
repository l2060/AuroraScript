using AuroraScript.common;
using AuroraScript.Exceptions;
using AuroraScript.Scanning;
using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroraScript.analyzer
{
    public class AuroraLexer
    {
        public Int32 LineNumber { get; private set; } = 1;
        public Int32 ColumnNumber { get; private set; } = 1;

        private Int32 ReadOffset { get; set; } = 0;
        private Int32 BufferLength { get; set; } = 0;

        public String FullPath { get; private set; }
        public String FileName { get; private set; }
        public String InputData { get; private set; }


        private List<TokenRules> _TokenRules { get; set; }


        public AuroraLexer(String file, Encoding encoding) : this(File.ReadAllText(file, encoding), file)
        {
        }

        public AuroraLexer(String text, String file)
        {
            this.FullPath = file;
            this.FileName = Path.GetFileName(file);
            this.InputData = text.Replace("\r\n", "\n");
            this.BufferLength = this.InputData.Length;
            this.InitRegexs();
        }

        private void AddRegex(TokenRules rule, Boolean skip = false)
        {
            this._TokenRules.Add(rule);
        }

        private void InitRegexs()
        {
            this._TokenRules = new List<TokenRules>();

            //// line break
            //this.AddRegex(new Regex("^[\\n]", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String), true, true);
            //// space 
            //this.AddRegex(new Regex("^[\\s]*", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String), true, false);
            //// single line Comment
            //this.AddRegex(new Regex("^//[^\n]*", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String), true, false);

            //// block Comment
            //this.AddRegex(new Regex("^/\\*([^\\*]|(\\*)*[^\\*/])*(\\*)*\\*\\/", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String), true, true);

            //// hex number
            //this.AddRegex(new Regex("^0[xX][0-9a-fA-F]+", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));

            //// number
            //this.AddRegex(new Regex("^(\\-|\\+)?\\d+(\\.\\d+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));

            //// String "" Support line break 
            //this.AddRegex(new Regex("^\"[^\"]*\"", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));
            //// String '' Support line break
            //this.AddRegex(new Regex("^\'[^\']*\'", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));
            //// String `` Support line break
            //this.AddRegex(new Regex("^`[^`]*`", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));

            //// id name  key words
            //this.AddRegex(new Regex("^[a-zA-Z_$][a-zA-Z0-9_$]*", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));

            //// += -= *= /= == != >= <=
            //this.AddRegex(new Regex("^\\+=|^-=|^\\*=|^\\/=|^==|^!=|^>=|^<=", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));
            //// ++ -- || && >> <<
            //this.AddRegex(new Regex("^\\+\\+|^--|^\\|\\||^&&|^>>|^<<", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));
            //// + - * / = % > < . , : ; ? ! ^ { } ( ) | ~ &
            //this.AddRegex(new Regex("^[+\\-*/=%<>.,;:?!^{}()|~&]", RegexOptions.IgnoreCase | RegexOptions.Compiled), typeof(String));

            this.AddRegex(TokenRules.NewLine);
            this.AddRegex(TokenRules.WhiteSpace);
            this.AddRegex(TokenRules.RowComment);
            this.AddRegex(TokenRules.BlockComment);
            this.AddRegex(TokenRules.HexNumber);
            this.AddRegex(TokenRules.Identifier);
            this.AddRegex(TokenRules.Number);
            this.AddRegex(TokenRules.Punctuator);
            this.AddRegex(TokenRules.StringDouble);
            this.AddRegex(TokenRules.StringSingle);
            this.AddRegex(TokenRules.StringTemplate);
        }


        public Token Next()
        {
            if (this.BufferLength <= 0) return Token.EOF;
            ReadOnlySpan<Char> span = this.InputData.AsSpan(this.ReadOffset, this.BufferLength);
            foreach (var rule in this._TokenRules)
            {
                var result = rule.Test(span, this.LineNumber, this.ColumnNumber);
                if (result.Success)
                {
                    if (result.Type == TokenTypes.Comment || result.Type == TokenTypes.NewLine || result.Type == TokenTypes.WhiteSpace)
                    {
                        this.ReadOffset += result.Value.Length;
                        this.BufferLength -= result.Value.Length;
                        this.LineNumber += result.LineCount;
                        this.ColumnNumber = result.ColumnNumber;
                        return this.Next();
                    }
                    var token = this.CreateToken(result);
                    Console.WriteLine(token);
                    this.ReadOffset += result.Value.Length;
                    this.BufferLength -= result.Value.Length;
                    this.LineNumber += result.LineCount;
                    this.ColumnNumber = result.ColumnNumber;
                    return token;
                }
            }
            throw new LexerException(this.FileName, this.LineNumber, this.ColumnNumber, "Invalid keywords 。");
        }




        private Token CreateToken(in RuleTestResult result)
        {
            Token token = null;
            var symbol = Symbols.FromString(result.Value);
            if (symbol != null)
            {
                if (symbol.Type == SymbolTypes.KeyWord) token = new KeywordToken();
                if (symbol.Type == SymbolTypes.Punctuator) token = new PunctuatorToken();
                if (symbol.Type == SymbolTypes.Operator) token = new OperatorToken();
                if (symbol.Type == SymbolTypes.Typed) token = new TypedToken();
                if (symbol.Type == SymbolTypes.Value) token = new ValueToken();
                token.Symbol = symbol;
            }
            else
            {
                if (result.Type == TokenTypes.String) token = new ValueToken();
                if (result.Type == TokenTypes.Number) token = new ValueToken();
                if (result.Type == TokenTypes.Identifier) token = new IdentifierToken();
            }
            if (token == null) throw new LexerException(this.FileName, this.LineNumber, this.ColumnNumber, $"Invalid Identifier {result.Value}");
            token.LineNumber = this.LineNumber;
            token.ColumnNumber = this.ColumnNumber;
            token.Value = result.Value;
            return token;
        }




    }







}



