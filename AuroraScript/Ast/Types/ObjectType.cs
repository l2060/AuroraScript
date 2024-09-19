using AuroraScript.Ast;
using AuroraScript.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Types
{
    public class ObjectType : AstNode
    {
        internal ObjectType(Token typeToken)
        {
            ElementType = typeToken;
        }



        /// <summary>
        /// function name
        /// </summary>
        public Token ElementType { get; set; }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.Write(ElementType.Value);
        }

    }
}
