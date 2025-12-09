using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptObjectConstructor : BondingFunction
    {
        public static ScriptObjectConstructor INSTANCE = new ScriptObjectConstructor();

        public ScriptObjectConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        public static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetObject(0, out var scriptObject))
            {
                result = ScriptDatum.FromObject(new ScriptObject(scriptObject));
            }
            else
            {
                result = ScriptDatum.FromObject(new ScriptObject());
            }
        }


        public static void KEYS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetObject(0, out var scriptObject))
            {
                result = ScriptDatum.FromArray(scriptObject.GetKeys());
            }
            else
            {
                result = ScriptDatum.FromArray(new ScriptArray());
            }
        }

    }
}
