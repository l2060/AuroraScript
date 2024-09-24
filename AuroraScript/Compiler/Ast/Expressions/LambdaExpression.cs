using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Stream;

namespace AuroraScript.Ast.Expressions
{
    internal class LambdaExpression : Expression
    {


        public FunctionType Declare;


        public Statement Block { get; set; }




        public override void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {
            this.Declare.GenerateLambdaCode(writer, depth);
            writer.Write($" {Symbols.PT_LAMBDA.Name} ");
            this.Block.GenerateCode(writer);
        }



    }
}
