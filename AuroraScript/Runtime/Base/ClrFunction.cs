using System;

namespace AuroraScript.Runtime.Base
{

    public delegate ScriptObject ClrMethodDelegate(ScriptDomain domain, ScriptObject module, ScriptObject[] args);


    public class ClrFunction : Callable
    {

        private readonly ClrMethodDelegate _callback;
        public readonly String Name;

        public ClrFunction(ClrMethodDelegate callback)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            this._callback = callback;
        }

        public override ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            return _callback.Invoke(domain, thisObject, args);
        }
        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this._callback, target);
        }

        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }

    }
}
