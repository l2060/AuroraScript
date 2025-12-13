using AuroraScript.Runtime.Base;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AuroraScript.Runtime.Types
{

    public delegate void ClrDatumDelegate([NotNull] ExecuteContext context, ScriptObject module, [NotNull] Span<ScriptDatum> args, ref ScriptDatum result);

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
                var methodInfo = _metadataSource.Method;
                if (methodInfo == null)
                {
                    return "<anonymous>";
                }
                var declaringType = methodInfo.DeclaringType.Name ?? "<anonymous>";
                return $"{declaringType}.{methodInfo.Name}";
            }
        }

        public readonly ClrDatumDelegate DatumMethod;
        public abstract void Invoke(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result);
    }
}
