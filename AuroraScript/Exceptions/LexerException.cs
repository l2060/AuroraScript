
namespace AuroraScript.Exceptions
{
    public class LexerException : Exception
    {
        public String FileName { get; private set; }
        public Int32 LineNumber { get; private set; }
        public Int32 ColumnNumber { get; private set; }
        public Token Token { get; private set; }

        internal LexerException(String fileName, Int32 lineNumber, Int32 columnNumber, String message) : base(message)
        {
            this.ColumnNumber = columnNumber;
            this.FileName = fileName;
            this.LineNumber = lineNumber;
            this.Token = Token;
        }


        internal LexerException(String fileName, Token token, String message) : base(message)
        {
            this.ColumnNumber = token.ColumnNumber;
            this.FileName = fileName;
            this.LineNumber = token.LineNumber;
            this.Token = token;
        }



        public override string ToString()
        {
            return $"Line:{this.LineNumber} Column:{this.ColumnNumber} {this.GetType().Name.PadRight(15, ' ')} {this.Token.Value}";
        }


    }
}
