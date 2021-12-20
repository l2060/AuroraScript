using AuroraScript.Common;
using AuroraScript.Tokens;

namespace AuroraScript
{

    public abstract class Token
    {
        public static Token EOF = new EndOfFileToken(); // end of file

        /// <summary>
        /// get token symbol object
        /// </summary>
        public Symbols Symbol
        {
            get;
            internal set;
        }

        /// <summary>
        /// get token string value
        /// </summary>
        public string Value
        {
            get;
            internal set;
        }

        /// <summary>
        /// get token line number
        /// </summary>
        internal Int32 LineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// get token column number
        /// </summary>
        internal Int32 ColumnNumber
        {
            get;
            set;
        }


        public override string ToString()
        {
            return $"LineNumber:{this.LineNumber.ToString().PadLeft(4, '0')} ColumnNumber:{this.ColumnNumber.ToString().PadLeft(3, '0')} {this.GetType().Name.PadRight(15, ' ')} {this.Value}";
        }

    }

}
