using AuroraScript.Core;
using AuroraScript.Runtime.Base;


namespace AuroraScript.Runtime.Types
{
    public class BooleanConstructor : BondingFunction
    {
        public readonly static BooleanConstructor INSTANCE = new BooleanConstructor();

        public BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.BooleanConstructorPrototype;
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return BooleanValue.False;
            }

            return args[0].IsTrue() ? BooleanValue.True : BooleanValue.False;
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return BooleanValue.False;
            }

            return args[0].IsTrue() ? BooleanValue.True : BooleanValue.False;
        }
    }
}
