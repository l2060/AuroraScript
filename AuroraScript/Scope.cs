using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Exceptions;

namespace AuroraScript
{
    /// <summary>
    /// statemtnt scope
    /// </summary>
    public class Scope
    {
        public Scope Parent { get; private set; }
        internal AuroraParser Parser { get; private set; }
        public IReadOnlyList<Scope> Childrens { get; private set; } = new List<Scope>();
        public Dictionary<string, ParameterDeclaration> Variables { get; private set; } = new Dictionary<string, ParameterDeclaration>();



        internal Scope(AuroraParser parser, Scope parent)
        {
            this.Parser = parser;
            this.Parent = parent;
            if(this.Parent != null && this.Parent.Childrens is List<Scope> list)
            {
                list.Add(this);
            }
        }


        public void FindToken(Token token)
        {



        }








        internal void DeclareVariable(ParameterDeclaration parameter)
        {
            var declarationScope = this;
            while (declarationScope != null)
            {
                if (declarationScope.Variables.ContainsKey(parameter.Variable.Value))
                {
                    throw new ParseException(this.Parser.lexer.FullPath, parameter.Variable, "Duplicate variable declaration in ");
                }
                declarationScope = declarationScope.Parent;
            }
            this.Variables.Add(parameter.Variable.Value, parameter);
        }

        internal void DefineVariable(VariableDeclaration parameter)
        {


        }

        internal void DeclareFunction(FunctionDeclaration parameter)
        {


        }

        internal void DefineFunction(FunctionDeclaration parameter)
        {


        }



    }
}
