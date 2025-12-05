using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;


namespace AuroraScript.Runtime.Types
{
    internal class ScriptRegexConstructor : BondingFunction
    {

        public static ScriptRegexConstructor INSTANCE = new ScriptRegexConstructor();

        public ScriptRegexConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
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
                    return args[0].Object;
                }
            }
            if (args.Length == 2 && args[1].Kind == ValueKind.String)
            {
                flags = args[1].String.Value;
            }
            return RegexManager.Resolve(pattern, flags);
        }

    }
}
