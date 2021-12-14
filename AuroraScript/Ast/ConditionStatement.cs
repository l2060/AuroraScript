using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    internal class ConditionStatement
    {
        public Expression Condition { get; set; }

        public BlockStatement Body { get; set; }


    }
}
