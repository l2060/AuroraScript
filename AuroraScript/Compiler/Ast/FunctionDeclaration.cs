using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Compiler.Ast;
using AuroraScript.Compiler.Emits;
using System.Collections.Generic;


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
    public class FunctionDeclaration : Statement, INamedStatement
    {

        internal FunctionDeclaration(MemberAccess access, Token identifier, List<ParameterDeclaration> parameters, Statement body, FunctionFlags flags)
        {
            Access = access;
            Name = identifier;
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
        public Token Name { get; private set; }

        public FunctionFlags Flags { get; private set; }

        internal ClosureCaptured[] CapturedVariables { get; set; } = System.Array.Empty<ClosureCaptured>();

        public override void Accept(IAstVisitor visitor)
        {
            visitor.AcceptFunction(this);
        }
    }
}