using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{

    public delegate ScriptObject ClrMethodDelegate(ExecuteContext context, ScriptObject module, ScriptObject[] args);


    public class ClrFunction : Callable
    {


        public ClrFunction(ClrMethodDelegate callback) : base(callback)
        {
        }

        public override ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var converted = ConvertArgs(args, out var rented);
            try
            {
                return Method.Invoke(context, thisObject, converted);
            }
            finally
            {
                //ReturnArgs(rented);
            }
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
