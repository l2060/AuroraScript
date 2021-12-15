using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    internal class Scope
    {
        public Scope Parent { get; private set; }

        public AuroraParser Parser { get; private set; }


        public Scope(AuroraParser parser,  Scope parent )
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
