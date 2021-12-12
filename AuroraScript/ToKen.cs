using AuroraScript.common;
using AuroraScript.Tokens;

namespace AuroraScript
{

    public abstract class Token
    {
        public static Token EOF = new EndOfFileToken(); // end of file

        public Symbols Symbol
        {
            get;
            internal set;
        }

        public string Value
        {
            get;
            internal set;
        }

        public Int32 LineNumber
        {
            get;
            internal set;
        }

        public Int32 ColumnNumber
        {
            get;
            internal set;
        }


        public override string ToString()
        {
            return $"LineNumber:{this.LineNumber.ToString().PadLeft(4, '0')} ColumnNumber:{this.ColumnNumber.ToString().PadLeft(3, '0')} {this.GetType().Name.PadRight(15, ' ')} {this.Value}";
        }

    }

}
