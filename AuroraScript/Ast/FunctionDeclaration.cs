using AuroraScript.Ast.Statements;
using AuroraScript.Common;

namespace AuroraScript.Ast
{
    public enum FunctionFlags
    {
        /// <summary>
        /// 普通方法
        /// </summary>
        General = 0,

        /// <summary>
        /// Get 方法
        /// </summary>
        GetMethod = 1,

        /// <summary>
        /// Set 方法
        /// </summary>
        SetMethod = 2,
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
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }
        /// <summary>
        /// Export ....
        /// </summary>
        public List<Token> Modifiers { get; set; }

        /// <summary>
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function name
        /// </summary>
        public Token Identifier { get; set; }

        /// <summary>
        /// function code
        /// </summary>
        public Statement Body { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<ObjectType> Typeds { get; set; }


        public FunctionFlags Flags { get; set; }


        public override String ToString()
        {
            var kw = "";
            if (Flags == FunctionFlags.General)
            {
                kw = Symbols.KW_FUNCTION.Name;
            }
            else if (Flags == FunctionFlags.GetMethod)
            {
                kw = Symbols.KW_GET.Name;
            }
            else if (Flags == FunctionFlags.SetMethod)
            {
                kw = Symbols.KW_SET.Name;
            }
            var declare = $"{Access.Name} {kw} {Identifier.Value}({String.Join(',', Parameters.Select(e => e.ToString()))}): {String.Join(',', Typeds.Select(e => e.ToString()))}";
            if (Body != null)
            {
                declare += $"{this.Body}";
            }
            return declare;
        }









    }
}
