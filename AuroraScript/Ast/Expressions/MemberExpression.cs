using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// 成员函数表达式
    /// </summary>
    internal class MemberExpression : NameExpression
    {
        public MemberExpression(IdentifierToken identifierToken, Expression _base)
        {
            this.Base = _base;
            this.Identifier = identifierToken;
        }
        /// <summary>
        /// super object name
        /// </summary>
        public AstNode Base { get; private set; }
    }

}
