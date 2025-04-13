using System.Xml.Linq;

namespace AuroraScript.Runtime.Base
{

    public delegate ScriptObject ClrGetterDelegate(ScriptObject @object);


    public class ClrGetter : ScriptObject
    {

        private readonly ClrGetterDelegate _callback;
        public readonly String Name;

        public ClrGetter(ClrGetterDelegate callback)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            this._callback = callback;
        }


        public ScriptObject Invoke(ScriptObject @object)
        {
            return _callback.Invoke(@object);
        }


        public override string ToString()
        {
            return "ClrGetter: " + Name;
        }


    }
}
