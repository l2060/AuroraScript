using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    internal class StringConstructor : BondingFunction
    {
        internal readonly static StringConstructor INSTANCE = new StringConstructor();

        internal StringConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.StringConstructorPrototype;
        }

        internal static void FROMCHARCODE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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
