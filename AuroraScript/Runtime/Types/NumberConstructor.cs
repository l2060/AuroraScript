using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime
{

    internal class NumberConstructor : BondingFunction
    {
        internal readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        internal readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        internal readonly static NumberValue NaN = new NumberValue(double.NaN);
        internal readonly static NumberValue MAX_VALUE = new NumberValue(double.MaxValue);
        internal readonly static NumberValue MIN_VALUE = new NumberValue(double.MinValue);
        internal static readonly NumberValue MAX_SAFE_INTEGER = new NumberValue(+9_007_199_254_740_991);
        internal static readonly NumberValue MIN_SAFE_INTEGER = new NumberValue(-9_007_199_254_740_991);
        internal readonly static NumberConstructor INSTANCE = new NumberConstructor();

        internal NumberConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.NumberConstructorPrototype;
        }

        internal static void PARSE_FLOAT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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
        internal static void PARSE_INTEGER(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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


        internal static void IS_NAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsNaN(num));
        }

        internal static void IS_INTEGER(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsInteger(num));
        }


        internal static void IS_INFINITY(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGetStrictNumber(0, out var num) && Double.IsInfinity(num));
        }

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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
