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
                result = ScriptDatum.FromNumber(Math.Floor(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void ROUND(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Round(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }




        #region MyRegion

        public static void COS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Cos(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }
        public static void ACOS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Acos(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void SIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Sin(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void ASIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Asin(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void TAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Tan(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void ATAN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Atan(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }





        #endregion






        public static void RANDOM(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromNumber(Random.Shared.NextDouble());
        }


        public static void POW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num1) && args.TryGetNumber(1, out var num2))
            {
                result = ScriptDatum.FromNumber(Math.Pow(num1, num2));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void EXP(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Exp(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void LOG(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(Math.Log(num));
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }

        public static void ABS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetNumber(0, out var num))
            {
                result = ScriptDatum.FromNumber(num < 0 ? -num : num);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
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
                result = ScriptDatum.FromNumber(num);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
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
                result = ScriptDatum.FromNumber(num);
            }
            else
            {
                result = ScriptDatum.FromNumber(Double.NaN);
            }
        }


    }
}
