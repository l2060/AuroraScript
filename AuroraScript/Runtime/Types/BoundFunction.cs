using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime
{
    public class BoundFunction : Callable
    {
        public ScriptObject Target;



        private readonly ClrMethodDelegate _callback;
        public readonly String Name;

        public BoundFunction(ClrMethodDelegate callback, ScriptObject _prototype, ScriptObject thisObject)
        {
            this._prototype = _prototype;
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            this._callback = callback;
            this.Target = thisObject;
        }


        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this._callback, _prototype, target);
        }


        public override ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var target = (thisObject == null) ? Target : thisObject;
            return _callback.Invoke(domain, target, args);
        }


    }
}
