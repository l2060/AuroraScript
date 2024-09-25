using AuroraScript.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuroraScript.Scanning
{
    internal struct RuleTestResult
    {
        public Boolean Success;
        public Int32 LineCount;
        public Int32 ColumnNumber;
        public String Value;
        public Int32 Length;
        public TokenTyped Type;
    }

    internal abstract class TokenRules
    {
        public static readonly TokenRules NewLine = new NewLineRule();
        public static readonly TokenRules WhiteSpace = new WhiteSpaceRule();
        public static readonly TokenRules RowComment = new RowCommentRule();
        public static readonly TokenRules BlockComment = new BlockCommentRule();
        public static readonly TokenRules HexNumber = new HexNumberCommentRule();
        public static readonly TokenRules Number = new NumberCommentRule();
        public static readonly TokenRules StringTemplate = new StringBlockRule();
        public static readonly TokenRules Identifier = new IdentifierRule();
        public static readonly TokenRules Punctuator = new PunctuatorRule();

        public abstract RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Boolean IsNumber(char lpChar)
        {
            return (lpChar >= '0' && lpChar <= '9');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Boolean IsChinese(char lpChar)
        {
            return (lpChar >= 0x4e00 && lpChar <= 0x9fbb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Boolean IsLetter(char lpChar)
        {
            return (lpChar >= 'a' && lpChar <= 'z') || (lpChar >= 'A' && lpChar <= 'Z');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe Boolean canEscape(char lpChar, out char outchar)
        {
            Char? c = null;
            if (lpChar == 'a') c = '\a';
            if (lpChar == 'b') c = '\b';
            if (lpChar == 'f') c = '\f';
            if (lpChar == 'n') c = '\n';
            if (lpChar == 'r') c = '\r';
            if (lpChar == 't') c = '\t';
            if (lpChar == 'v') c = '\v';
            if (lpChar == '0') c = '\0';
            if (lpChar == '\\') c = '\\';
            if (lpChar == '\'') c = '\'';
            if (lpChar == '"') c = '"';
            if (c.HasValue)
            {
                outchar = c.Value;
                return true;
            }
            else
            {
                outchar = '\0';
                return false;
            }
        }
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
            if (codeSpan.Length >= 3)
            {
                if (codeSpan[0] == '.' && codeSpan[1] == '.' && codeSpan[2] == '.')
                {
                    result.ColumnNumber += 3;
                    result.Length = 3;
                    result.Value = codeSpan.Slice(0, 3).ToString();
                    result.Success = true;
                    return result;
                }
            }
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
                   (codeSpan[0] == '<' && codeSpan[1] == '<') ||
                   (codeSpan[0] == '=' && codeSpan[1] == '>'))

                {
                    result.ColumnNumber += 2;
                    result.Length = 2;
                    result.Value = codeSpan.Slice(0, 2).ToString();
                    result.Success = true;
                }
                else if (
                    codeSpan[0] == '+' || codeSpan[0] == '-' || codeSpan[0] == '*' || codeSpan[0] == '/' ||
                    codeSpan[0] == '=' || codeSpan[0] == '%' || codeSpan[0] == '<' || codeSpan[0] == '>' ||
                    codeSpan[0] == '.' || codeSpan[0] == ',' || codeSpan[0] == ';' || codeSpan[0] == ':' ||
                    codeSpan[0] == '?' || codeSpan[0] == '!' || codeSpan[0] == '^' || codeSpan[0] == '{' ||
                    codeSpan[0] == '}' || codeSpan[0] == '[' || codeSpan[0] == ']' || codeSpan[0] == '(' ||
                    codeSpan[0] == ')' || codeSpan[0] == '|' || codeSpan[0] == '~' || codeSpan[0] == '&'
                    )
                {
                    result.ColumnNumber += 1;
                    result.Length = 1;
                    result.Value = codeSpan[0].ToString();
                    result.Success = true;
                }
            }
            else if (
                    codeSpan[0] == '+' || codeSpan[0] == '-' || codeSpan[0] == '*' || codeSpan[0] == '/' ||
                    codeSpan[0] == '=' || codeSpan[0] == '%' || codeSpan[0] == '<' || codeSpan[0] == '>' ||
                    codeSpan[0] == '.' || codeSpan[0] == ',' || codeSpan[0] == ';' || codeSpan[0] == ':' ||
                    codeSpan[0] == '?' || codeSpan[0] == '!' || codeSpan[0] == '^' || codeSpan[0] == '{' ||
                    codeSpan[0] == '}' || codeSpan[0] == '[' || codeSpan[0] == ']' || codeSpan[0] == '(' ||
                    codeSpan[0] == ')' || codeSpan[0] == '|' || codeSpan[0] == '~' || codeSpan[0] == '&'
                    )
            {
                result.ColumnNumber += 1;
                result.Length = 1;
                result.Value = codeSpan[0].ToString();
                result.Type = TokenTyped.Punctuator;
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
        public override unsafe RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if ((codeSpan[0] >= 'a' && codeSpan[0] <= 'z') ||
                (codeSpan[0] >= 'A' && codeSpan[0] <= 'Z') ||
                (codeSpan[0] >= 0x4e00 && codeSpan[0] <= 0x9fbb) ||
                  codeSpan[0] == '_' ||
                  codeSpan[0] == '$')
            {
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if ((codeSpan[i] >= 'a' && codeSpan[i] <= 'z') ||
                    (codeSpan[i] >= 'A' && codeSpan[i] <= 'Z') ||
                    (codeSpan[i] >= 0x4e00 && codeSpan[i] <= 0x9fbb) ||
                    (codeSpan[i] >= '0' && codeSpan[i] <= '9') ||
                     codeSpan[i] == '_')
                    {
                    }
                    else
                    {
                        result.ColumnNumber += i;
                        result.Length = i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTyped.Identifier;
                        break;
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    ///  format string block
    /// </summary>
    internal class StringBlockRule : TokenRules
    {
        private static StringBuilder sb = new StringBuilder();

        public override RuleTestResult Test(in ReadOnlySpan<Char> codeSpan, in Int32 LineNumber, in Int32 ColumnNumber)
        {
            var result = new RuleTestResult();
            result.ColumnNumber = ColumnNumber;
            if (codeSpan[0] == '`' || codeSpan[0] == '"' || codeSpan[0] == '\'')
            {
                sb.Clear();
                var keychar = codeSpan[0];
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    var viewChar = codeSpan[i];
                    // parse escape char
                    if (viewChar == '\\')
                    {
                        result.ColumnNumber++;
                        if (!base.canEscape(codeSpan[i + 1], out viewChar))
                        {
                            throw new Exception("xxxxxx");
                        }
                        result.ColumnNumber++;
                        i++;
                    }
                    else if (viewChar == '\n')
                    {
                        result.ColumnNumber = 0;
                        result.LineCount += 1;
                    }
                    else
                    {
                        result.ColumnNumber++;
                        if (viewChar == keychar)
                        {
                            result.Length = i + 1;
                            result.Value = sb.ToString();// codeSpan.Slice(1, i - 1).ToString();
                            result.Success = true;
                            result.Type = TokenTyped.String;
                            break;
                        }
                    }
                    sb.Append(viewChar);
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
            Int32 dot = -1;
            Char? lastChar = null;
            if ((codeSpan[0] >= '0' && codeSpan[0] <= '9') || codeSpan[0] == '-')
            {
                lastChar = codeSpan[0];
                for (int i = 1; i < codeSpan.Length; i++)
                {
                    if (codeSpan[i] >= '0' && codeSpan[i] <= '9')
                    {
                    }
                    else if (codeSpan[i] == '_')
                    {
                        if (lastChar == '-' || lastChar == '.' || lastChar == '_') return result;
                    }
                    else if (codeSpan[i] == '.')
                    {
                        if (lastChar == '-' || lastChar == '.' || lastChar == '_') return result;
                        if (lastChar == '-') return result;
                        if (dot > -1) return result;
                        dot = i;
                    }
                    else if (codeSpan[0] == '-' && i == 1)
                    {
                        return result;
                    }
                    else
                    {
                        result.ColumnNumber += i;
                        result.Length = i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTyped.Number;
                        break;
                    }
                    lastChar = codeSpan[i];
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
                        result.Length = i;
                        result.Value = codeSpan.Slice(0, i).ToString();
                        result.Success = true;
                        result.Type = TokenTyped.Number;
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
                            result.Length = i + 2;
                            result.Value = codeSpan.Slice(0, i + 2).ToString();
                            result.Type = TokenTyped.Comment;
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
                        result.Length = i + 1;
                        result.Value = codeSpan.Slice(0, i + 1).ToString();
                        result.Type = TokenTyped.Comment;
                        result.Success = true;
                        break;
                    }
                }
                if (!result.Success)
                {
                    result.ColumnNumber = 0;
                    result.LineCount = 1;
                    result.Length = codeSpan.Length;
                    result.Value = codeSpan.Slice(0, codeSpan.Length).ToString();
                    result.Type = TokenTyped.Comment;
                    result.Success = true;
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
                result.Length = Index;
                result.Value = codeSpan.Slice(0, Index).ToString();
                result.Type = TokenTyped.WhiteSpace;
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
                result.Length = 1;
                result.Type = TokenTyped.NewLine;
                result.Success = true;
            }
            return result;
        }
    }
}