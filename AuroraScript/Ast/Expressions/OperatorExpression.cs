using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    internal class OperatorExpression : Expression
    {
        public OperatorExpression(Operator @operator)
        {
            this.Operator = @operator;
        }

        private Expression[] operands;







        /// <summary>
        /// Gets or sets the operator this expression refers to.
        /// </summary>
        public Operator Operator
        {
            get;
            private set;
        }





    }




}
