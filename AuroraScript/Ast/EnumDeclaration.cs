using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            return $"{this.Access.Name} {Symbols.KW_ENUM.Name} {Identifier.Value} {{\r\n {text} \r\n}}";
        }





        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.WriteLine($"{this.Access.Name} {Symbols.KW_ENUM.Name} {Identifier.Value} {{");
            foreach (EnumElement element in Elements)
            {
                writer.WriteLine($"    {element.Name.Value} = {element.Value},");
            }
            writer.WriteLine("}}");
        }
    }
}
