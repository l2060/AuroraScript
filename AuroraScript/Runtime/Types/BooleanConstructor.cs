using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;


namespace AuroraScript.Runtime.Types
{
    internal class BooleanConstructor : BondingFunction
    {
        internal readonly static BooleanConstructor INSTANCE = new BooleanConstructor();

        internal BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.BooleanConstructorPrototype;
        }

        internal static void PARSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGet(0, out var scriptDatum) && scriptDatum.IsTrue());
        }

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.TryGet(0, out var scriptDatum) && scriptDatum.IsTrue());
        }
    }
}
