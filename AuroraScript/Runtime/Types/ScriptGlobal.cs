using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using System.Linq;

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
            base.Define(key, ClrMarshaller.ToScript(value), true, true);
        }

        public void Define(string key, object value, bool writeable = true, bool enumerable = true)
        {
            base.Define(key, ClrMarshaller.ToScript(value), writeable, enumerable);
        }

        public override string ToString()
        {
            return "global";
        }
    }
}
