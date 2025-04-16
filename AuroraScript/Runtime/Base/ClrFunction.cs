namespace AuroraScript.Runtime.Base
{

    public delegate ScriptObject ClrMethodDelegate(AuroraEngine engine, ScriptObject module, ScriptObject[] args);


    public class ClrFunction : ScriptObject
    {

        private readonly ClrMethodDelegate _callback;
        public readonly String Name;

        public ClrFunction(ClrMethodDelegate callback)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            this._callback = callback;
        }

        public ScriptObject Invoke(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            return _callback.Invoke(engine, thisObject, args);
        }


        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }

    }
}
