using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class StringConstructor : ClrFunction
    {
        public readonly static StringConstructor INSTANCE = new StringConstructor();

        public StringConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.StringConstructorPrototype;
        }

        public static ScriptObject FROMCHARCODE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return StringValue.Empty;
            }

            var datum = args[0];
            Int32 codePoint;
            if (datum.Kind == ValueKind.Number)
            {
                codePoint = (Int32)datum.Number;
            }
            else
            {
                var number = datum.ToObject() as NumberValue;
                codePoint = number?.Int32Value ?? 0;
            }

            return StringValue.Of(((Char)codePoint).ToString());
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return StringValue.Empty;
            }

            var datum = args[0];
            if (datum.Kind == ValueKind.String && datum.String != null)
            {
                return datum.String;
            }

            var strObj = datum.ToObject() as StringValue;
            if (strObj != null)
            {
                return strObj;
            }

            return StringValue.Of(datum.ToObject()?.ToString() ?? String.Empty);
        }
    }
}
