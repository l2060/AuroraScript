using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Core
{
    public class CodeScope
    {

        private CodeScope? _parent;

        public int ScopeDepth { get; private set; } = 0;

        private int _variableBaseCount = 0;

        private readonly Dictionary<string, int> variables = new Dictionary<string, int>();

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

        public int DeclareLocalVariable(string name)
        {
            int slot = _variableBaseCount++;
            variables[name] = slot;
            return slot;
        }


        public Boolean TryGetVariablee(string name, out int value)
        {
            if (variables.TryGetValue(name, out value))
            {
                return true;
            }
            if (_parent != null) return _parent.TryGetVariablee(name, out value);
            return false;
        }

    }
}

