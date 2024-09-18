using AuroraScript.Ast.Expressions;
using AuroraScript.Ast.Statements;
using AuroraScript.Common;
using System;

namespace AuroraScript.Ast
{
    /// <summary>
    /// variable declaration
    /// </summary>
    public class VariableDeclaration : Expression
    {
        internal VariableDeclaration()
        {
            this.Variables = new List<Token>();
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
        /// get / set variable typed
        /// </summary>
        public ObjectType Typed { get; set; }

        /// <summary>
        /// Function Access
        /// </summary>
        public Symbols Access { get; set; }

        /// <summary>
        /// this variable use const declare
        /// </summary>
        public Boolean IsConst { get; set; }



        public override String ToString()
        {
            var key = IsConst ? Symbols.KW_CONST.Name : Symbols.KW_VAR.Name;
            return $"{key} {String.Join(',', Variables.Select(e => e.Value))}:{Typed} {Symbols.OP_ASSIGNMENT.Name} {Initializer};";
        }

        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            var key = IsConst ? Symbols.KW_CONST.Name : Symbols.KW_VAR.Name;
            //writer.Write($"{key} {String.Join(',', Variables.Select(e => e.Value))}:{Typed} {Symbols.OP_ASSIGNMENT.Name} {Initializer}");
            writer.Write($"{key} ");
            writer.Write(String.Join(',', Variables.Select(e => e.Value)));
            writer.Write($":");
            if (Typed != null) Typed.WriteCode(writer);
            writer.Write($" {Symbols.OP_ASSIGNMENT.Name} ");
            if(Initializer != null) Initializer.WriteCode(writer);


        }

    }
}
