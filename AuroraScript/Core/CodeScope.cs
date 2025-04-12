namespace AuroraScript.Core
{
    public enum DeclareType
    {
        Variable,
        Property
    }

    public class DeclareObject
    {
        public DeclareObject(CodeScope scope, DeclareType type, Int32 index)
        {
            Type = type;
            Index = index;
            Scope = scope;
        }
        public readonly CodeScope Scope;
        public readonly DeclareType Type;
        public readonly int Index;
    }


    public enum DomainType
    {
        Global,
        Module,
        Function,
        Code
    }


    public class CodeScope
    {

        private CodeScope _parent;

        public int ScopeDepth { get; private set; } = 0;

        private int _variableBaseCount = 0;

        private readonly Dictionary<string, DeclareObject> variables = new Dictionary<string, DeclareObject>();

        public DomainType Domain { get; private set; }


        public CodeScope(CodeScope current, DomainType domain   )
        {
            _parent = current;

            if (_parent != null)
            {
                ScopeDepth = _parent.ScopeDepth + 1;
                _variableBaseCount = _parent._variableBaseCount;
            }

            Domain = domain;
        }

        /// <summary>
        /// 进入新的范围
        /// </summary>
        /// <returns></returns>

        public CodeScope Enter(DomainType domain)
        {
            var scope = new CodeScope(this, domain);
            return scope;
        }

        /// <summary>
        /// 离开范围
        /// </summary>
        /// <returns></returns>
        public CodeScope Leave()
        {
            return _parent;
        }




        public int Declare(DeclareType type, string name)
        {
            int slot = _variableBaseCount++;
            var declare = new DeclareObject(this, type, slot);
            variables[name] = declare;
            return slot;
        }


        public Boolean Resolve(string name, out DeclareObject value)
        {
            if (variables.TryGetValue(name, out value))
            {
                return true;
            }
            if (_parent != null) return _parent.Resolve(name, out value);
            return false;
        }


        public Boolean Resolve(DeclareType type, string name, out int value)
        {
            value = 0;
            if (variables.TryGetValue(name, out var declareObject))
            {
                value = declareObject.Index;
                return true;
            }
            if (_parent != null) return _parent.Resolve(type, name, out value);
            return false;
        }


    }
}

