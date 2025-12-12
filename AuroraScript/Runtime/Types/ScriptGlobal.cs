using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;

namespace AuroraScript.Runtime.Types
{
    public sealed class ScriptGlobal : ScriptObject
    {
        public AuroraEngine Engine { get; private set; }


        internal ScriptGlobal(AuroraEngine engine, ScriptObject prototype = null)
        {
            Engine = engine;
            _prototype = prototype;
        }


        internal static ScriptGlobal With(AuroraEngine engine, ScriptObject prototype)
        {
            return new ScriptGlobal(engine, prototype);
        }

        public void SetValue(string key, object value)
        {
            SetPropertyValue(key, ClrMarshaller.ToScript(value));
        }

        public void Define(string key, object value, bool writeable = true, bool readable = true, bool enumerable = true)
        {
            base.Define(key, ClrMarshaller.ToScript(value), writeable, readable, enumerable);
        }

        public override string ToString()
        {
            return "global";
        }
    }
}
