using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    public class EnumElement
    {
        public Token Name;
        public Int32 Value;
    }






    public class EnumDeclaration : Statement
    {
        internal EnumDeclaration()
        {
            this.Access = Symbols.KW_INTERNAL;
        }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        public Token Identifier { get; set; }
        public List<EnumElement> Elements { get; set; }




        public override String ToString()
        {
            var elements = Elements.Select(e => $"{e.Name.Value}={e.Value},");

            var text = String.Join("\r\n", elements);

            return $"{this.Access.Name} enum {Identifier.Value} {{\r\n {text} \r\n}}";
        }


    }
}
