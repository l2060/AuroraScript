using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast
{
    /// <summary>
    /// function parameter declaration
    /// </summary>
    internal class ParameterDeclaration
    {
        /// <summary>
        /// parameter Modifier  ....
        /// </summary>
        public Token Modifier { get; set; }

        /// <summary>
        /// Parameter
        /// </summary>
        public Token Variable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AstNode DefaultValue { get; set; }


    }
}
