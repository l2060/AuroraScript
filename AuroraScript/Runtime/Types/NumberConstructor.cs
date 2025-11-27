using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime
{

    public class NumberConstructor : BondingFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(double.NaN);
        public readonly static NumberValue MAX_VALUE = new NumberValue(double.MaxValue);
        public readonly static NumberValue MIN_VALUE = new NumberValue(double.MinValue);
        public static readonly NumberValue MAX_SAFE_INTEGER = new NumberValue(+9_007_199_254_740_991);
        public static readonly NumberValue MIN_SAFE_INTEGER = new NumberValue(-9_007_199_254_740_991);
        public readonly static NumberConstructor INSTANCE = new NumberConstructor();

        public NumberConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.NumberConstructorPrototype;
        }

        public static ScriptObject PARSE_FLOAT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
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
        public static ScriptObject PARSE_INTEGER(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return NumberValue.NaN;
            }

            if (TryGetInteger(args[0], out var value))
            {
                return NumberValue.Of(value);
            }

            return NumberValue.NaN;
        }


        public static ScriptObject IS_NAN(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return BooleanValue.Of((args.Length > 0 && args[0].Kind == ValueKind.Number && Double.IsNaN(args[0].Number)));
        }

        public static ScriptObject IS_INTEGER(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return BooleanValue.Of((args.Length > 0 && args[0].Kind == ValueKind.Number && Double.IsInteger(args[0].Number)));
        }


        public static ScriptObject IS_INFINITY(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return BooleanValue.Of((args.Length > 0 && args[0].Kind == ValueKind.Number && Double.IsInfinity(args[0].Number)));
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
        private static Boolean TryGetInteger(ScriptDatum datum, out Int64 value)
        {
            switch (datum.Kind)
            {
                case ValueKind.Number:
                    value = (Int64)datum.Number;
                    return true;
                case ValueKind.Boolean:
                    value = datum.Boolean ? 1L : 0L;
                    return true;
                case ValueKind.String:
                    return Int64.TryParse(datum.String.Value, out value);
                case ValueKind.Object:
                    break;
            }
            value = 0;
            return false;
        }


    }
}
