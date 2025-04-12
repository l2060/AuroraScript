using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Ast
{
    public interface INamedStatement
    {
        public Token Name { get; }
    }
}
