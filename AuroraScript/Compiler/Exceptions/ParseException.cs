namespace AuroraScript.Compiler.Exceptions
{
    public class ParseException : Exception
    {
        public string fileName { get; private set; }
        public int lineNumber { get; private set; }
        public int columnNumber { get; private set; }
        public Token token { get; private set; }

        internal ParseException(string fileName, Token token, string message) : base(message)
        {
            columnNumber = token.ColumnNumber;
            this.fileName = fileName;
            lineNumber = token.LineNumber;
            this.token = token;
        }

        public override string ToString()
        {
            return $"Line:{lineNumber} Column:{columnNumber} {GetType().Name.PadRight(15, ' ')} {token.Value} ${Message}";
        }
    }
}