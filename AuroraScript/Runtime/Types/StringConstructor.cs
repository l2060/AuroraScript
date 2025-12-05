using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class StringConstructor : BondingFunction
    {
        public readonly static Lazy<StringConstructor> INSTANCE = new Lazy<StringConstructor>();

        public StringConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.StringConstructorPrototype;
        }

        public static ScriptObject FROMCHARCODE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.Length == 0)
            {
                return StringValue.Empty;
            }

            var datum = args[0];
            Int32 codePoint = 0;
            if (datum.Kind == ValueKind.Number)
            {
                codePoint = (Int32)datum.Number;
            }
            return StringValue.Of(((Char)codePoint).ToString());
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.Length == 0)
            {
                return StringValue.Empty;
            }
            var datum = args[0];
            if (datum.Kind == ValueKind.String && datum.String != null)
            {
                return datum.String;
            }
            return StringValue.Of(datum.ToString());
        }
    }
}
