using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime
{
    public class NumberConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(double.NaN);
        public readonly static NumberValue MAX_VALUE = new NumberValue(double.MaxValue);
        public readonly static NumberValue MIN_VALUE = new NumberValue(double.MinValue);
        public readonly static NumberConstructor INSTANCE = new NumberConstructor();

        public NumberConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.NumberConstructorPrototype;
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return NumberValue.NaN;
            }

            if (TryGetDouble(args[0], out var value))
            {
                return NumberValue.Of(value);
            }

            return NumberValue.NaN;
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return NumberValue.Zero;
            }

            if (TryGetDouble(args[0], out var value))
            {
                return NumberValue.Of(value);
            }

            return NumberValue.NaN;
        }

        private static Boolean TryGetDouble(ScriptDatum datum, out Double value)
        {
            switch (datum.Kind)
            {
                case ValueKind.Number:
                    value = datum.Number;
                    return true;
                case ValueKind.Boolean:
                    value = datum.Boolean ? 1d : 0d;
                    return true;
                case ValueKind.String:
                    return Double.TryParse(datum.String.Value, out value);
                case ValueKind.Object:
                    break;
            }
            value = Double.NaN;
            return false;
        }
    }
}
