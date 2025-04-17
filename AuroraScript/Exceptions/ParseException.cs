using System;

namespace AuroraScript.Compiler.Exceptions
{
    public class ParseException : Exception
    {
        public string FileName { get; private set; }
        public int LineNumber { get; private set; }
        public int ColumnNumber { get; private set; }
        public Token Token { get; private set; }


        internal ParseException(string fileName, Token token, string message) : base(message)
        {
            ColumnNumber = token.ColumnNumber;
            this.FileName = fileName;
            LineNumber = token.LineNumber;
            this.Token = token;
        }

        public override string ToString()
        {
            return $"Line:{LineNumber} Column:{ColumnNumber} {GetType().Name.PadRight(15, ' ')} {Token.Value} ${Message}";
        }
    }
}