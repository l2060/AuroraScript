using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class BooleanConstructor : BondingFunction
    {
        public readonly static BooleanConstructor INSTANCE = new BooleanConstructor();

        public BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.BooleanConstructorPrototype;
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return BooleanValue.False;
            }

            return GetBoolean(args[0]) ? BooleanValue.True : BooleanValue.False;
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return BooleanValue.False;
            }

            return GetBoolean(args[0]) ? BooleanValue.True : BooleanValue.False;
        }

        private static Boolean GetBoolean(ScriptDatum datum)
        {
            return datum.Kind switch
            {
                ValueKind.Boolean => datum.Boolean,
                ValueKind.Number => Math.Abs(datum.Number) > Double.Epsilon,
                ValueKind.String => Boolean.TryParse(datum.String.Value, out var parsed) && parsed,
                ValueKind.Null => false,
                ValueKind.Object => datum.Object != null,
                _ => datum.ToObject()?.IsTrue() ?? false
            };
        }
    }
}
