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

        public static void FROMCHARCODE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {

            if (args.TryGetInteger(0, out var codePoint))
            {
                result = ScriptDatum.FromString(((Char)codePoint).ToString());
            }
            else
            {
                result = ScriptDatum.FromString(String.Empty);
            }
        }

        public static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetString(0, out var str))
            {
                result = ScriptDatum.FromString(str);
            }
            else
            {
                result = ScriptDatum.FromString(String.Empty);
            }
        }
    }
}
