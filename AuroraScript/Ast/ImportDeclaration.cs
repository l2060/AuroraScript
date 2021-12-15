using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    internal class ImportDeclaration: Statement
    {
        public Token File { get; set; }
    }
}
