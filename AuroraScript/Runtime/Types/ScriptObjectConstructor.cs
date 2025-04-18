using AuroraScript.Runtime.Base;


namespace AuroraScript.Runtime.Types
{
    internal class ScriptObjectConstructor : ClrFunction
    {
        public static ScriptObjectConstructor INSTANCE = new ScriptObjectConstructor();

        public ScriptObjectConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }


        public static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1)
            {
                return new ScriptObject(args[0]);
            }
            return new ScriptObject();
        }

    }
}
