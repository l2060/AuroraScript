using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{

    public delegate ScriptObject ClrMethodDelegate(ScriptDomain domain, ScriptObject module, ScriptObject[] args);


    public class ClrFunction : Callable
    {

        private readonly ClrMethodDelegate _callback;
        public readonly string Name;

        public ClrFunction(ClrMethodDelegate callback)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            _callback = callback;
        }

        public override ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            return _callback.Invoke(domain, thisObject, args);
        }
        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(_callback, target);
        }

        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }

    }
}
