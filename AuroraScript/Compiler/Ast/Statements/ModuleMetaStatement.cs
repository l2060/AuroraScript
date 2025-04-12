using AuroraScript.Ast.Statements;
using AuroraScript.Tokens;

namespace AuroraScript.Compiler.Ast.Statements
{
    public class ModuleMetaStatement : Statement
    {
        public IdentifierToken Name { get; private set; }
        public Token Value { get; private set; }



        internal ModuleMetaStatement(IdentifierToken metaName, Token metaValue)
        {
            Name = metaName;
            Value = metaValue;
        }

        public override void Accept(IAstVisitor visitor)
        {

        }

    }
}
