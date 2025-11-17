using AuroraScript.Core;
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

        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var capacity = 0;
            if (args != null && args.Length == 1)
            {
                var datum = args[0];
                if (datum.Kind == ValueKind.Number)
                {
                    capacity = (int)datum.Number;
                }
            }

            return new ScriptArray(capacity);
        }
    }
}
