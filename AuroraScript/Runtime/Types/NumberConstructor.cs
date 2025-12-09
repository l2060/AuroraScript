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

        public static void PARSE_FLOAT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var number))
            {
                result = ScriptDatum.FromNumber(number);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }
        public static void PARSE_INTEGER(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetInteger(0, out var number))
            {
                result = ScriptDatum.FromNumber(number);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }


        public static void IS_NAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsNaN(num));
        }

        public static void IS_INTEGER(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsInteger(num));
        }


        public static void IS_INFINITY(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsInfinity(num));
        }

        public static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var number))
            {
                result = ScriptDatum.FromNumber(number);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

    }
}
