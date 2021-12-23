using AuroraScript.Ast.Statements;
using AuroraScript.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    public class TypeDeclaration : Statement
    {
        internal TypeDeclaration()
        {

        }

        public Token Identifier { get; set; }
        public ObjectType Typed { get; set; }
    }
}
