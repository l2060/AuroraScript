using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Exceptions
{
    public class ParseException : Exception
    {
        public String fileName { get; private set; }
        public Int32 lineNumber { get; private set; }
        public Int32 columnNumber { get; private set; }
        public Token token { get; private set; }


        internal ParseException(String fileName, Token token, String message) : base(message)
        {
            this.columnNumber = token.ColumnNumber;
            this.fileName = fileName;
            this.lineNumber = token.LineNumber;
            this.token = token;
        }



        public override string ToString()
        {
            return $"Line:{this.lineNumber} Column:{this.columnNumber} {this.GetType().Name.PadRight(15, ' ')} {this.token.Value} ${this.Message}";
        }


    }
}
