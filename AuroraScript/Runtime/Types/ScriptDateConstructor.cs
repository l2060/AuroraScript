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


        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return PARSE(context, thisObject, args);
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



        public static ScriptObject NOW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return new ScriptDate(DateTime.Now);
        }


        public static ScriptObject UTC_NOW(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return new ScriptDate(DateTime.UtcNow);
        }


        public new static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptDate date)
            {
                if (args.TryGetString(0, out var value))
                {
                    return StringValue.Of(date.DateTime.ToString(value));
                }
                else
                {
                    return StringValue.Of(date.DateTime.ToString());
                }
            }
            return ScriptObject.Null;
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.TryGetInteger(0, out var value)) // ticks
            {
                return new ScriptDate(value);
            }
            else if (args.TryGetString(0, out var strValue)) // yyyyMMdd ....
            {
                if (DateTime.TryParseExact(strValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    return new ScriptDate(dt);
                }
            }
            return ScriptObject.Null;
        }

    }
}
