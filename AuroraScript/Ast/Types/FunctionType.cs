using AuroraScript.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Types
{
    internal class FunctionType : ObjectType
    {
        internal FunctionType(Token typeToken) : base(typeToken)
        {

        }
    }
}
