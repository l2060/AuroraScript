using AuroraScript.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Core
{
    public enum DeclareType
    {
        Function,
        Variable
    }

    public class DeclareObject
    {
        public DeclareObject(DeclareType type,  Int32 index)
        {
            Type = type;
            Index = index;
        }
        public readonly DeclareType Type;
        public readonly int Index;
    }




    public class CodeScope
    {

        private CodeScope? _parent;

        public int ScopeDepth { get; private set; } = 0;

        private int _variableBaseCount = 0;

        private readonly Dictionary<string, DeclareObject> variables = new Dictionary<string, DeclareObject>();

        public CodeScope(CodeScope current)
        {
            _parent = current;

            if (_parent != null)
            {
                ScopeDepth = _parent.ScopeDepth + 1;
                _variableBaseCount = _parent._variableBaseCount;
            }
        }

        /// <summary>
        /// 提升
        /// </summary>
        /// <returns></returns>

        public CodeScope Promotion()
        {
            var scope = new CodeScope(this);
            return scope;
        }

        /// <summary>
        /// 降级
        /// </summary>
        /// <returns></returns>
        public CodeScope Demotion()
        {
            return _parent;
        }




        public int Declare(DeclareType type, string name)
        {
            int slot = _variableBaseCount++;
            var declare = new DeclareObject(type, slot);
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
            if (_parent != null) return _parent.Resolve(type,name, out value);
            return false;
        }


    }
}

