using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{

    public abstract class Token
    {
        public static Token EOF = new Token(-1); // end of file

        public static String EOL = "\\n"; // end of line

 





        protected Token(UInt32 line)
        {
            this.LineNumber = line;
        }


        public Boolean isIdentifier()
        {
            return false;
        }

        public Boolean isNumber()
        {
            return false;
        }

        public Boolean isString()
        {
            return false;
        }

        public int getNumber()
        {
            throw new StoneException("not number token");
        }

        public String getText()
        {
            return "";
        }









        public string File
        {
            get;
            private set;
        }


        public UInt32 LineNumber
        {
            get;
            private set;
        }

        public UInt32 LineOffset
        {
            get;
            private set;
        }



    }

}
