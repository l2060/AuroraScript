using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Globalization;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptDateConstructor : BondingFunction
    {

        public static ScriptDateConstructor INSTANCE = new ScriptDateConstructor();


        public ScriptDateConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.DateConstructorPrototype;
        }


        public static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            PARSE(context, thisObject, args, ref result);
        }


        static string[] formats =
          {
                "yyyy-MM-dd",
                "yyyy/MM/dd",
                "yyyyMMdd",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss",
                "yyyyMMddHHmmss",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "MM/dd/yyyy",
                "MM-dd-yyyy",
                "dd/MM/yyyy",
                "dd-MM-yyyy"
            };



        public static void NOW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromDate(new ScriptDate(DateTime.Now));
        }


        public static void UTC_NOW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromDate(new ScriptDate(DateTime.UtcNow));
        }


        public new static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptDate date)
            {
                if (args.TryGetString(0, out var value))
                {
                    result = ScriptDatum.FromString(date.DateTime.ToString(value));
                }
                else
                {
                    result = ScriptDatum.FromString(date.DateTime.ToString());
                }
            }
        }

        public static void PARSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetInteger(0, out var value)) // ticks
            {
                result = ScriptDatum.FromDate(new ScriptDate(value));
            }
            else if (args.TryGetString(0, out var strValue)) // yyyyMMdd ....
            {
                if (DateTime.TryParseExact(strValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    result = ScriptDatum.FromDate(new ScriptDate(dt));
                }
            }
        }

    }
}
