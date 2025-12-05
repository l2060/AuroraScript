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

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.Length == 1)
            {
                return new ScriptObject(args[0].ToObject());
            }
            return new ScriptObject();
        }


        public static ScriptObject KEYS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.Length == 1)
            {
                if (args[0].Object is ScriptObject scriptObject)
                {
                    return scriptObject.GetKeys();
                }
            }
            return new ScriptArray();
        }

    }
}
