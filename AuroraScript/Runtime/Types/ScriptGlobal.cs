using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;

namespace AuroraScript.Runtime.Types
{
    public class ScriptGlobal : ScriptObject
    {
        private ClrTypeRegistry _clrRegistry;

        internal void AttachRegistry(ClrTypeRegistry registry)
        {
            _clrRegistry = registry;
        }

        private ClrTypeRegistry ResolveRegistry()
        {
            if (_clrRegistry != null)
            {
                return _clrRegistry;
            }

            if (_prototype is ScriptGlobal globalPrototype)
            {
                return globalPrototype.ResolveRegistry();
            }

            return null;
        }

        private ScriptObject ConvertClrValue(object value)
        {
            if (value is ScriptObject scriptObject)
            {
                return scriptObject;
            }

            var registry = ResolveRegistry();
            if (registry == null)
            {
                throw new AuroraException("CLR type registry is not available. Ensure the ScriptGlobal is attached to an engine before setting CLR values.");
            }

            return ClrMarshaller.ToScript(value, registry);
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
