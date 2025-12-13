using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Pool;
using System;


namespace AuroraScript.Runtime.Types
{
    internal class ScriptRegexConstructor : BondingFunction
    {

        internal static ScriptRegexConstructor INSTANCE = new ScriptRegexConstructor();

        internal ScriptRegexConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.Length == 0)
            {
                throw new Exception();
            }

            var flags = "";
            var pattern = "";

            if (args.Length == 1)
            {
                if (args[0].Kind == ValueKind.String)
                {
                    pattern = args[0].String.Value;
                }
                else if (args[0].Kind == ValueKind.Regex)
                {
                    result = args[0];
                    return;
                }
            }
            if (args.Length == 2 && args[1].Kind == ValueKind.String)
            {
                flags = args[1].String.Value;
            }
            result = ScriptDatum.FromRegex(RegexManager.Resolve(pattern, flags));
        }

    }
}
