using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    public class ClrFunction : Callable
    {
        private static ClrDatumDelegate CreateLegacyAdapter(ClrMethodDelegate callback)
        {
            return (context, module, args) =>
            {
                var converted = ConvertArgsForLegacy(args, out var rented);
                try
                {
                    return callback(context, module, converted);
                }
                finally
                {
                    ReturnLegacyArgs(rented);
                }
            };
        }

        public ClrFunction(ClrDatumDelegate callback) : base(callback)
        {
        }

        public ClrFunction(ClrMethodDelegate callback) : base(CreateLegacyAdapter(callback), callback)
        {
        }

        public override ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return DatumMethod(context, thisObject, args);
        }

        public override BoundFunction Bind(ScriptObject target)
        {
            return new BoundFunction(this, target);
        }

        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }
    }
}
