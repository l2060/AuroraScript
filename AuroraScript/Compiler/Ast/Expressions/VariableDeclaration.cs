using AuroraScript.Compiler;


namespace AuroraScript.Ast.Expressions
{
    /// <summary>
    /// variable declaration
    /// </summary>
    public class VariableDeclaration : Expression
    {
        internal VariableDeclaration(Token nameToken, Expression initializer)
        {
            Variables = new List<Token>() { nameToken };
            Initializer = initializer;
        }
        internal VariableDeclaration(Symbols access, Boolean isConst, List<Token> nameTokens, Expression initializer)
        {
            Access = access;
            IsConst = isConst;
            Variables = nameTokens;
            Initializer = initializer;
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

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitVarDeclaration(this);
        }

        public override string ToString()
        {
            return $"var {string.Join($"{Symbols.PT_COMMA.Name} ", Variables.Select(e => e.Value))} = {Initializer}";
        }


    }
}