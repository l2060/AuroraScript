using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;

namespace AuroraScript.Runtime
{
    public class BoundFunction : Callable
    {
        public ScriptObject Target;

        public BoundFunction(Callable callable, ScriptObject thisObject)
            : base(callable.DatumMethod, callable.MetadataSource)
        {
            this._prototype = callable._prototype;
            this.Target = thisObject;
        }


        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this, target);
        }


        public override ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var target = (thisObject == null) ? Target : thisObject;
            return DatumMethod(context, target, args);
        }


    }
}
