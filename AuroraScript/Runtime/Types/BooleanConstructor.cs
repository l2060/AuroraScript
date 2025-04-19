using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{

    public class BooleanConstructor : ClrFunction
    {

        public readonly static BooleanConstructor INSTANCE = new BooleanConstructor();

        public BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.BooleanConstructorPrototype;
        }

        public static ScriptObject PARSE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = bool.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }


        public static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = bool.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }

    }
}
