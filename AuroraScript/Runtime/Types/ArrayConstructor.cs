using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    public class ArrayConstructor : ClrFunction
    {

        public readonly static ArrayConstructor INSTANCE = new ArrayConstructor();


        public ArrayConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ArrayConstructorPrototype;
        }


        public static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            int capacity = 0;
            if (args.Length == 1 && args[0] is NumberValue numberValue)
            {
                capacity = numberValue.Int32Value;
            }
            var array = new ScriptArray(capacity);
            return array;
        }
    }
}
