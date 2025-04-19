using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{

    public class StringConstructor : ClrFunction
    {

        public readonly static StringConstructor INSTANCE = new StringConstructor();


        public StringConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.StringConstructorPrototype;
        }


        public static ScriptObject FROMCHARCODE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0 && args[0] is NumberValue numberValue)
            {
                Char _char = (Char)numberValue.Int64Value;
                return StringValue.Of(_char.ToString());
            }
            return StringValue.Empty;
        }




        public static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                return StringValue.Of(args[0].ToString());
            }
            return StringValue.Empty;
        }
    }
}
