using System;

namespace AuroraScript.Compiler.Exceptions
{
    public class AuroraLexerException : Exception
    {
        public string FileName { get; private set; }
        public int LineNumber { get; private set; }
        public int ColumnNumber { get; private set; }
        public Token Token { get; private set; }

        internal AuroraLexerException(string fileName, int lineNumber, int columnNumber, string message) : base(message)
        {
            ColumnNumber = columnNumber;
            FileName = fileName;
            LineNumber = lineNumber;
            Token = Token;
        }

        internal AuroraLexerException(string fileName, Token token, string message) : base(message)
        {
            ColumnNumber = token.ColumnNumber;
            FileName = fileName;
            LineNumber = token.LineNumber;
            Token = token;
        }

        public override string ToString()
        {
            return $"Line:{LineNumber} Column:{ColumnNumber} {GetType().Name.PadRight(15, ' ')} {Token.Value}";
        }
    }
}