using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Types
{
    public class FunctionTypeNode : TypeNode
    {
        public AstNode Name { get; set; }
        public List<ParameterDeclaration> Parameters { get; set; }

        public TypeNode ResultTyped;

    }
}
