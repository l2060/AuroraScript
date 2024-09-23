using AuroraScript.Ast.Statements;
using AuroraScript.Ast.Types;
using AuroraScript.Stream;


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
        /// parameters
        /// </summary>
        public List<ParameterDeclaration> Parameters { get; set; }

        /// <summary>
        /// function code
        /// </summary>
        public Statement Body { get; set; }

        /// <summary>
        /// function result types
        /// </summary>
        public List<TypeNode> Typeds { get; set; }


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


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
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

            writer.Write($"{Access.Name} {kw} {Identifier.Value}{Symbols.PT_LEFTPARENTHESIS.Name}");
            this.writeParameters(writer, Parameters, ", ");
            writer.Write("{0}{1} ", Symbols.PT_RIGHTPARENTHESIS.Name, Symbols.PT_COLON.Name);

            if (Typeds.Count > 0)
            {
                if (Typeds.Count == 1)
                {
                    this.writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                }
                else
                {
                    writer.Write(Symbols.PT_LEFTBRACKET.Name);
                    this.writeParameters(writer, Typeds, Symbols.PT_COMMA.Name + " ");
                    writer.Write(Symbols.PT_RIGHTBRACKET.Name);
                }
            }
            writer.WriteLine();
            if (Body != null) this.Body.GenerateCode(writer);
            writer.WriteLine();

        }




    }
}
