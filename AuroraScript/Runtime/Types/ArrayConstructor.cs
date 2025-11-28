using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    public class ArrayConstructor : BondingFunction
    {
        public readonly static ArrayConstructor INSTANCE = new ArrayConstructor();

        public ArrayConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ArrayConstructorPrototype;
        }



        public static ScriptObject FROM(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return ScriptObject.Null;
            }

            var iterator = ResolveIterator(args[0]);
            if (iterator == null)
            {
                return ScriptObject.Null;
            }
            var thisArg = args.Length > 2 ? args[2].ToObject() : ScriptObject.Null;
            thisArg ??= ScriptObject.Null;
            var executeOptions = context?.ExecuteOptions ?? ExecuteOptions.Default;
            var result = new ScriptArray();
            var index = 0;

            while (iterator.HasValue())
            {
                var current = iterator.Value();
                result.PushDatum(current);
                iterator.Next();
                index++;
            }

            return result;
        }

        public static ScriptObject OF(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var array = new ScriptArray(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                array.Set(i, args[i]);
            }
            return array;
        }


        public static ScriptObject IS_ARRAY(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return BooleanValue.Of(args.Length > 0 && args[0].Kind == ValueKind.Array);
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

        private static ItemIterator ResolveIterator(ScriptDatum datum)
        {
            if (datum.Kind == ValueKind.Null)
            {
                return null;
            }
            if (datum.Object is IEnumerator enumerator)
            {
                return enumerator.GetIterator();
            }

            if (datum.TryGetObject(out var candidate) && candidate is IEnumerator iterator)
            {
                return iterator.GetIterator();
            }
            return null;
        }

    }
}
