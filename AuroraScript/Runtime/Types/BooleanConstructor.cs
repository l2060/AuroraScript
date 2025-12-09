using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;


namespace AuroraScript.Runtime.Types
{
    public class BooleanConstructor : BondingFunction
    {
        public readonly static BooleanConstructor INSTANCE = new BooleanConstructor();

        public BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.BooleanConstructorPrototype;
        }

        public static void PARSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGet(0, out var scriptDatum) && scriptDatum.IsTrue());
        }

        public static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGet(0, out var scriptDatum) && scriptDatum.IsTrue());
        }
    }
}
