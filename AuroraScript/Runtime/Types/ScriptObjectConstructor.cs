using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptObjectConstructor : BondingFunction
    {
        public static ScriptObjectConstructor INSTANCE = new ScriptObjectConstructor();

        public ScriptObjectConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args != null && args.Length == 1)
            {
                return new ScriptObject(args[0].ToObject());
            }
            return new ScriptObject();
        }
    }
}
