using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;

namespace AuroraScript.Runtime
{
    public class BoundFunction : Callable
    {
        public ScriptObject Target;

        public BoundFunction(Callable callable, ScriptObject thisObject) : base(callable.Method)
        {
            this._prototype = callable._prototype;
            this.Target = thisObject;
        }


        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this, target);
        }


        public override ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            var target = (thisObject == null) ? Target : thisObject;
            return Method.Invoke(context, target, args);
        }


    }
}
