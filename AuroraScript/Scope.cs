using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Exceptions;

namespace AuroraScript
{
    public class Scope
    {
        public Scope Parent { get; private set; }

        internal AuroraParser Parser { get; private set; }

        internal Scope(AuroraParser parser,  Scope parent )
        {
            this.Parent = parent;
            this.Parser = parser;
            this.Variables = new Dictionary<string, ParameterDeclaration>();
        }

        public Dictionary<string, ParameterDeclaration> Variables { get; private set; }





        public void DeclareVariable(List<ParameterDeclaration> parameters)
        {
            foreach (var parameter in parameters)
            {
                this.DeclareVariable(parameter);
            }
        }




        public void DeclareVariable(ParameterDeclaration parameter)
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



        public void DeclareFunction(ParameterDeclaration parameter)
        {
          

        }

    }
}
