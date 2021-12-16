using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class ValueExpression: Expression
    {

        public ValueExpression(Token value)
        {
            this.Value = value;
        }
        public Token Value { get; set; }

    }
}
