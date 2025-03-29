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

        /// <summary>
        /// 仅声明
        /// </summary>
        Declare = 2
    }





    /// <summary>
    /// 函数定义
    /// </summary>
    public class FunctionDeclaration : Statement
    {

        internal FunctionDeclaration(MemberAccess access, Token identifier, List<ParameterDeclaration> parameters, Statement body, FunctionFlags flags)
        {
            Access = access;
            Identifier = identifier;
            Parameters = parameters;
            Body = body;
            Flags = flags;
        }




        /// <summary>
        /// parameters
        /// </summary>
        public IReadOnlyList<ParameterDeclaration> Parameters { get; private set; }

        /// <summary>
        /// function code
        /// </summary>
        public Statement Body { get; private set; }

        /// <summary>
        /// Function Access
        /// </summary>
        public MemberAccess Access { get; private set; }

        /// <summary>
        /// Export ....
        /// </summary>
        public List<Token> Modifiers { get; private set; }

        /// <summary>
        /// function name
        /// </summary>
        public Token Identifier { get; private set; }

        public FunctionFlags Flags { get; private set; }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitFunction(this);
        }
    }
}