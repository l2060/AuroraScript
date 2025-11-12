using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Buffers;

namespace AuroraScript.Runtime.Types
{
    public delegate ScriptObject ClrDatumDelegate(ExecuteContext context, ScriptObject module, ScriptDatum[] args);
    public delegate ScriptObject ClrMethodDelegate(ExecuteContext context, ScriptObject module, ScriptObject[] args);

    public abstract partial class Callable : ScriptObject
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

        public abstract BoundFunction Bind(ScriptObject target);
        public abstract ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args);

        protected static ScriptObject[] ConvertArgsForLegacy(ScriptDatum[] args, out ScriptObject[] rented)
        {
            rented = null;
            if (args == null || args.Length == 0)
            {
                return Array.Empty<ScriptObject>();
            }

            var array = ArrayPool<ScriptObject>.Shared.Rent(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                array[i] = args[i].ToObject();
            }

            if (array.Length == args.Length)
            {
                rented = array;
                return array;
            }

            var exact = new ScriptObject[args.Length];
            Array.Copy(array, exact, args.Length);
            ArrayPool<ScriptObject>.Shared.Return(array, clearArray: true);
            return exact;
        }

        protected static void ReturnLegacyArgs(ScriptObject[] rented)
        {
            if (rented != null)
            {
                ArrayPool<ScriptObject>.Shared.Return(rented, clearArray: true);
            }
        }

        public static ScriptObject BIND(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var callable = thisObject as Callable;
            var target = (args != null && args.Length > 0) ? args[0].ToObject() : ScriptObject.Null;
            return new BoundFunction(callable, target);
        }
    }
}
