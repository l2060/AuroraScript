using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{

    public delegate ScriptObject ClrMethodDelegate(ScriptDomain domain, ScriptObject module, ScriptObject[] args);


    public class ClrFunction : Callable
    {


        public ClrFunction(ClrMethodDelegate callback) : base(callback)
        {
        }

        public override ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            return Method.Invoke(domain, thisObject, args);
        }
        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this, target);
        }

        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }

    }
}
