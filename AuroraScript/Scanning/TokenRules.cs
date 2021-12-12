using AuroraScript.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Scanning
{

    internal struct RuleTestResult
    {
        public Boolean Success = false;
        public Int32 LineCount;
        public Int32 ColumnNumber;
        public String Value;
        public TokenTypes Type;

    }




    internal abstract class TokenRules
    {
        public readonly static TokenRules NewLine = new NewLineRule();
        public readonly static TokenRules WhiteSpace = new WhiteSpaceRule();
        public readonly static TokenRules RowComment = new RowCommentRule();
        public readonly static TokenRules BlockComment = new BlockCommentRule();
        public readonly static TokenRules HexNumber = new HexNumberCommentRule();
        public readonly static TokenRules Number = new NumberCommentRule();
        public readonly static TokenRules StringTemplate = new StringTemplateRule();
        public readonly static TokenRules StringSingle = new StringSingleRule();
        public readonly static TokenRules StringDouble = new StringDoubleRule();
        public readonly static TokenRules Identifier = new IdentifierRule();
        public readonly static TokenRules Punctuator = new PunctuatorRule();
        public abstract RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber);

    }





    /// <summary>
    ///  \n new line
    /// </summary>
    internal class PunctuatorRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;

            if (codeSpan.Length >= 2)
            {
                if ((codeSpan[0] == '+' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '-' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '*' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '/' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '=' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '!' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '>' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '<' && codeSpan[1] == '=') ||
                   (codeSpan[0] == '+' && codeSpan[1] == '+') ||
                   (codeSpan[0] == '-' && codeSpan[1] == '-') ||
                   (codeSpan[0] == '|' && codeSpan[1] == '|') ||
                   (codeSpan[0] == '&' && codeSpan[1] == '&') ||
                   (codeSpan[0] == '>' && codeSpan[1] == '>') ||
                   (codeSpan[0] == '<' && codeSpan[1] == '<')
                    )
                {
                    result.ColumnNumber += 2;
                    result.Value = codeSpan.Slice(0, 2).ToString();
                    result.Success = true;
                }
                else if (codeSpan[0] == '+' ||
                    codeSpan[0] == '+' || codeSpan[0] == '-' || codeSpan[0] == '*' || codeSpan[0] == '/' ||
                    codeSpan[0] == '=' || codeSpan[0] == '%' || codeSpan[0] == '<' || codeSpan[0] == '>' ||
                    codeSpan[0] == '.' || codeSpan[0] == ',' || codeSpan[0] == ';' || codeSpan[0] == ':' ||
                    codeSpan[0] == '?' || codeSpan[0] == '!' || codeSpan[0] == '^' || codeSpan[0] == '{' ||
                    codeSpan[0] == '}' || codeSpan[0] == '[' || codeSpan[0] == ']' || codeSpan[0] == '(' ||
                    codeSpan[0] == ')' || codeSpan[0] == '|' || codeSpan[0] == '~' || codeSpan[0] == '&'
                    )
                {
                    result.ColumnNumber += 1;
                    result.Value = codeSpan[0].ToString();
                    result.Success = true;
                }
            }
            else if (codeSpan[0] == '+' ||
                    codeSpan[0] == '+' || codeSpan[0] == '-' || codeSpan[0] == '*' || codeSpan[0] == '/' ||
                    codeSpan[0] == '=' || codeSpan[0] == '%' || codeSpan[0] == '<' || codeSpan[0] == '>' ||
                    codeSpan[0] == '.' || codeSpan[0] == ',' || codeSpan[0] == ';' || codeSpan[0] == ':' ||
                    codeSpan[0] == '?' || codeSpan[0] == '!' || codeSpan[0] == '^' || codeSpan[0] == '{' ||
                    codeSpan[0] == '}' || codeSpan[0] == '[' || codeSpan[0] == ']' || codeSpan[0] == '(' ||
                    codeSpan[0] == ')' || codeSpan[0] == '|' || codeSpan[0] == '~' || codeSpan[0] == '&'
                    )
            {
                result.ColumnNumber += 1;
                result.Value = codeSpan[0].ToString();
                result.Type = TokenTypes.Punctuator;
                result.Success = true;
            }
            return result;
        }
    }





    /// <summary>
    ///  \n new line
    /// </summary>
    internal class IdentifierRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if ((codeSpan[0] >= 'a' && codeSpan[0] <= 'z') ||
                (codeSpan[0] == 'A' && codeSpan[0] == 'Z') ||
                  codeSpan[0] == '_' ||
                  codeSpan[0] == '$')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if ((codeSpan[i] >= 'a' && codeSpan[i] <= 'z') ||
                        (codeSpan[i] == 'A' && codeSpan[i] == 'Z') ||
                         codeSpan[i] == '_' ||
                         codeSpan[i] == '$')
                    {

                    }
                    else
                    {
                        result.ColumnNumber += i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTypes.Identifier;
                        break;
                    }
                }
            }
            return result;
        }
    }



    /// <summary>
    ///  quotes
    /// </summary>
    internal class StringDoubleRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan[0] == '"')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount += 1;
                    }
                    else
                    {
                        result.ColumnNumber++;
                        if (codeSpan[i] == '"')
                        {
                            result.Value = codeSpan.Slice(0, i).ToString();
                            result.Success = true;
                            result.Type = TokenTypes.String;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }




    /// <summary>
    ///  \n new line
    /// </summary>
    internal class StringSingleRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan[0] == '\'')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount += 1;
                    }
                    else
                    {
                        result.ColumnNumber++;
                        if (codeSpan[i] == '\'')
                        {
                            result.Value = codeSpan.Slice(0, i + 1).ToString();
                            result.Success = true;
                            result.Type = TokenTypes.String;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }



    /// <summary>
    ///  \n new line
    /// </summary>
    internal class StringTemplateRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan[0] == '`')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount += 1;
                    }
                    else
                    {
                        result.ColumnNumber++;
                        if (codeSpan[i] == '`')
                        {
                            result.Value = codeSpan.Slice(0, i).ToString();
                            result.Success = true;
                            result.Type = TokenTypes.String;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }






    internal class NumberCommentRule : TokenRules
    {

        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            Int32 dot = 0;
            if (codeSpan[0] >= '0' && codeSpan[0] <= '9')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] >= '0' && codeSpan[i] <= '9')
                    {
                    }
                    else if (codeSpan[i] == '.' && dot == 0)
                    {
                        dot++;
                    }
                    else
                    {
                        result.ColumnNumber += i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTypes.Number;
                        break;
                    }
                }
            }
            return result;
        }
    }











    internal class HexNumberCommentRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan.Length > 2 && codeSpan[0] == '0' && (codeSpan[1] == 'x' || codeSpan[1] == 'X'))
            {
                for (int i = 2; i < codeSpan.Length; i++)
                {
                    if ((codeSpan[i] >= 'a' && codeSpan[i] <= 'f') ||
                        (codeSpan[i] >= 'A' && codeSpan[i] <= 'F') ||
                        (codeSpan[i] >= '0' && codeSpan[i] <= '9'))
                    {

                    }
                    else
                    {
                        result.ColumnNumber += i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTypes.Number;
                        break;
                    }
                }
            }
            return result;
        }
    }

















    /// <summary>
    /// Block Comment
    /// </summary>
    internal class BlockCommentRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan.Length >= 2 && codeSpan[0] == '/' && codeSpan[1] == '*')
            {
                for (int i = 0; i < codeSpan.Length - 1; i++)
                {
                    if (codeSpan[i] == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount += 1;
                    }
                    else
                    {
                        result.ColumnNumber++;
                        if (codeSpan[i] == '*' && codeSpan[i + 1] == '/')
                        {
                            result.ColumnNumber++;
                            result.Value = codeSpan.Slice(0, i + 2).ToString();
                            result.Type = TokenTypes.Comment;
                            result.Success = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }









    /// <summary>
    /// Row Comment
    /// </summary>
    internal class RowCommentRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan.Length >= 2 && codeSpan[0] == '/' && codeSpan[1] == '/')
            {
                for (int i = 0; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount = 1;
                        result.Value = codeSpan.Slice(0, i + 1).ToString();
                        result.Type = TokenTypes.Comment;
                        result.Success = true;
                        break;
                    }
                }
            }
            return result;
        }
    }


    /// <summary>
    /// white space
    /// </summary>
    internal class WhiteSpaceRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            Int32 Index = 0;
            while (Char.IsWhiteSpace(codeSpan[Index]))
            {
                Index++;
            }
            if (Index > 0)
            {
                result.ColumnNumber += Index;
                result.Value = codeSpan.Slice(0, Index).ToString();
                result.Type = TokenTypes.WhiteSpace;
                result.Success = true;
            }
            return result;
        }
    }





    /// <summary>
    ///  \n new line
    /// </summary>
    internal class NewLineRule : TokenRules
    {
        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            if (codeSpan[0] == '\n')
            {
                result.Value = "\n";
                result.LineCount = 1;
                result.ColumnNumber = 0;
                result.Type = TokenTypes.NewLine;
                result.Success = true;
            }
            return result;
        }
    }






}
