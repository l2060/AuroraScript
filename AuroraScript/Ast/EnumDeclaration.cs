using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    public class EnumElement {
        public Token Name;
        public Int32 Value;
    }






    public class EnumDeclaration : Statement
    {
        internal EnumDeclaration()
        {

        }

        public Token Identifier { get; set; }
        public List<EnumElement> Elements { get; set; }
    }
}
