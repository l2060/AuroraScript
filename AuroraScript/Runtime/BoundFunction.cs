using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    public class BoundFunction : Callable
    {
        public ScriptObject Target;



        private readonly ClrMethodDelegate _callback;
        public readonly String Name;

        public BoundFunction(ClrMethodDelegate callback, ScriptObject thisObject)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            this._callback = callback;
            this.Target = thisObject;
        }


        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this._callback, target);
        }


        public override ScriptObject Invoke(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            var target = (thisObject == null) ? Target : thisObject;
            return _callback.Invoke(engine, target, args);
        }


    }
}
