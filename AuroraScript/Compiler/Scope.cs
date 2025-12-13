using AuroraScript.Analyzer;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuroraScript.Compiler
{
    internal enum ScopeType
    {
        UNKNOWN,
        MODULE,
        FOR,
        BLOCK,
        GROUP,
        FUNCTION,
        CONSTRUCTOR
    }

    /// <summary>
    /// statemtnt scope
    /// </summary>
    internal class Scope
    {
        [JsonIgnore]
        public Scope Parent { get; private set; }

        internal AuroraParser Parser { get; private set; }
        public List<Scope> Childrens { get; private set; } = new List<Scope>();
        public ScopeType Type { get; private set; }

        internal Scope(AuroraParser parser)
        {
            Parser = parser;
            Type = ScopeType.MODULE;
        }

        public Scope CreateScope(ScopeType scopeType)
        {
            var scope = new Scope(Parser);
            scope.Parent = this;
            scope.Type = scopeType;
            Childrens.Add(scope);
            return scope;
        }
    }
}