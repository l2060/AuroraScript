using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;


namespace AuroraScript.Ast
{
    public enum FunctionFlags
    {
        /// <summary>
        /// 普通方法
        /// </summary>
        General = 0,

        /// <summary>
        /// Lambda 方法
        /// </summary>
        Lambda = 1,
    }

    /// <summary>
    /// 函数定义
    /// </summary>
    public class FunctionDeclaration : Statement
    {
        internal FunctionDeclaration()
        {
        }

        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function code
        /// </summary>
        public Statement Body { get; set; }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        /// <summary>
        /// Export ....
        /// </summary>
        public List<Token> Modifiers { get; set; }

        /// <summary>
        /// function name
        /// </summary>
        public Token Identifier { get; set; }

        public FunctionFlags Flags { get; set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitFunction(this);
        }
    }
}