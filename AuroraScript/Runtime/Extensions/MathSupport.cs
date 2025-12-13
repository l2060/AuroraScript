using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime.Extensions
{
    internal class MathSupport : ScriptObject
    {
        public MathSupport()
        {
            Define("PI", new NumberValue(Math.PI), writeable: false, enumerable: false);
            Define("E", new NumberValue(Math.E), writeable: false, enumerable: false);
            Define("Tau", new NumberValue(Math.Tau), writeable: false, enumerable: false);
            Define("DEG_PER_RAD", new NumberValue(Math.PI / 180D), writeable: false, enumerable: false);





            Define("abs", new BondingFunction(ABS), writeable: false, enumerable: false);
            Define("max", new BondingFunction(MAX), writeable: false, enumerable: false);
            Define("min", new BondingFunction(MAX), writeable: false, enumerable: false);

            Define("random", new BondingFunction(RANDOM), writeable: false, enumerable: false);
            Define("log", new BondingFunction(LOG), writeable: false, enumerable: false);
            Define("pow", new BondingFunction(POW), writeable: false, enumerable: false);
            Define("exp", new BondingFunction(EXP), writeable: false, enumerable: false);

            Define("cos", new BondingFunction(COS), writeable: false, enumerable: false);
            Define("sin", new BondingFunction(SIN), writeable: false, enumerable: false);
            Define("tan", new BondingFunction(TAN), writeable: false, enumerable: false);
            Define("acos", new BondingFunction(ACOS), writeable: false, enumerable: false);
            Define("asin", new BondingFunction(ASIN), writeable: false, enumerable: false);
            Define("atan", new BondingFunction(ATAN), writeable: false, enumerable: false);



            Define("floor", new BondingFunction(FLOOR), writeable: false, enumerable: false);
            Define("round", new BondingFunction(ROUND), writeable: false, enumerable: false);

        }


        public static void FLOOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Floor(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void ROUND(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Round(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }




        #region MyRegion

        public static void COS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Cos(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }
        public static void ACOS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Acos(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void SIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Sin(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void ASIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Asin(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void TAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Tan(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void ATAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Atan(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }





        #endregion






        public static void RANDOM(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            ScriptDatum.NumberOf(Random.Shared.NextDouble(), out result);
        }


        public static void POW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num1) && args.TryGetNumber(1, out var num2))
            {
                ScriptDatum.NumberOf(Math.Pow(num1, num2), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void EXP(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Exp(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void LOG(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(Math.Log(num), out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void ABS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                ScriptDatum.NumberOf(num < 0 ? -num : num, out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }


        public static void MAX(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                var index = 1;
                while (args.TryGetNumber(index++, out var num2))
                {
                    if (num2 > num) num = num2;
                }
                ScriptDatum.NumberOf(num, out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }

        public static void MIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                var index = 1;
                while (args.TryGetNumber(index++, out var num2))
                {
                    if (num2 < num) num = num2;
                }
                ScriptDatum.NumberOf(num, out result);
            }
            else
            {
                ScriptDatum.NumberOf(Double.NaN, out result);
            }
        }


    }
}
