using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public abstract partial class Callable : ScriptObject
    {
        protected Callable(ClrMethodDelegate method)
        {
            Method = method;
            _prototype = Prototypes.CallablePrototype;
        }

        public String Name => Method.Method.DeclaringType.Name + "." + Method.Method.Name;

        public readonly ClrMethodDelegate Method;
        public abstract BoundFunction Bind(ScriptObject target);
        public abstract ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args);




        public static ScriptObject BIND(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var callable = thisObject as Callable;
            return new BoundFunction(callable, (args.Length > 0) ? args[0] : ScriptObject.Null);
        }
    }
}
