using AuroraScript.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Common
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



        public override String ToString()
        {
            return $"{ElementType.Value}";
        }


    }
}
