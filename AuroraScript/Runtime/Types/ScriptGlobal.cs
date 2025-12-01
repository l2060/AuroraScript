using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;

namespace AuroraScript.Runtime.Types
{
    public class ScriptGlobal : ScriptObject
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

        private ScriptObject ConvertClrValue(object value)
        {
            if (value is ScriptObject scriptObject)
            {
                return scriptObject;
            }
            return ClrValueConverter.ToScriptObject(value);
        }

        public void SetPropertyValue(string key, object value)
        {
            base.SetPropertyValue(key, ConvertClrValue(value));
        }

        public void Define(string key, object value, bool writeable = true, bool readable = true, bool enumerable = true)
        {
            base.Define(key, ConvertClrValue(value), writeable, readable, enumerable);
        }

        public override string ToString()
        {
            return "global";
        }
    }
}
