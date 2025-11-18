using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Buffers;

namespace AuroraScript.Runtime.Types
{

    public delegate ScriptObject ClrDatumDelegate(ExecuteContext context, ScriptObject module, ScriptDatum[] args);

    public abstract class Callable : ScriptObject
    {
        private readonly Delegate _metadataSource;

        protected Callable(ClrDatumDelegate datumMethod, Delegate metadataSource = null)
        {
            DatumMethod = datumMethod ?? throw new ArgumentNullException(nameof(datumMethod));
            _metadataSource = metadataSource ?? datumMethod;
            _prototype = Prototypes.CallablePrototype;
        }

        internal Delegate MetadataSource => _metadataSource;

        public String Name
        {
            get
            {
                var methodInfo = _metadataSource?.Method;
                if (methodInfo == null)
                {
                    return "<anonymous>";
                }
                var declaringType = methodInfo.DeclaringType?.Name ?? "<anonymous>";
                return $"{declaringType}.{methodInfo.Name}";
            }
        }

        public readonly ClrDatumDelegate DatumMethod;
        public abstract ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args);
    }
}
