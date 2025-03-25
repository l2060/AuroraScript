using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// variable declaration
    /// </summary>
    public class VariableDeclaration : Expression
    {
        internal VariableDeclaration()
        {
            Variables = new List<Token>();
        }

        /// <summary>
        /// parameter Modifier  ....
        /// </summary>
        public Token Modifier { get; set; }

        /// <summary>
        /// variable names
        /// </summary>
        public List<Token> Variables { get; set; }

        /// <summary>
        /// var initialize statement
        /// </summary>
        public Expression Initializer { get; set; }


        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        /// <summary>
        /// this variable use const declare
        /// </summary>
        public bool IsConst { get; set; }

        public override void GenerateCode(TextCodeWriter writer, int depth = 0)
        {
            var key = IsConst ? Symbols.KW_CONST.Name : Symbols.KW_VAR.Name;
            writer.Write($"{key} ");
            writer.Write(string.Join($"{Symbols.PT_COMMA.Name} ", Variables.Select(e => e.Value)));
            writer.Write($" {Symbols.OP_ASSIGNMENT.Name} ");
            if (Initializer != null) Initializer.GenerateCode(writer);
        }
        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }
    }
}