using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types.Internal;
using System;

namespace AuroraScript.Runtime.Types
{
    internal class ArrayConstructor : BondingFunction
    {
        internal readonly static ArrayConstructor INSTANCE = new ArrayConstructor();

        internal ArrayConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ArrayConstructorPrototype;
        }



        internal static void FROM(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.Length == 0)
            {
                result = ScriptDatum.Null;
                return;
            }

            var iterator = ResolveIterator(args[0]);
            if (iterator == null)
            {
                result = ScriptDatum.Null;
                return;
            }
            var thisArg = args.Length > 2 ? ScriptDatum.ToObject(in args[2]) : ScriptObject.Null;
            thisArg ??= ScriptObject.Null;
            var executeOptions = context.ExecuteOptions ?? ExecuteOptions.Default;
            var array = new ScriptArray();
            var index = 0;

            while (iterator.HasValue())
            {
                var current = iterator.Value();
                array.PushDatum(current);
                iterator.Next();
                index++;
            }
            result = ScriptDatum.FromArray(array);
        }

        internal static void OF(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var array = new ScriptArray(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                array.Set(i, args[i]);
            }
            result = ScriptDatum.FromArray(array);
        }


        internal static void IS_ARRAY(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromBoolean(args.Length > 0 && args[0].Kind == ValueKind.Array);
        }





        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var capacity = 0;
            if (args.Length == 1)
            {
                var datum = args[0];
                if (datum.Kind == ValueKind.Number)
                {
                    capacity = (int)datum.Number;
                }
            }
            result = ScriptDatum.FromArray(new ScriptArray(capacity));
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

            if (ScriptDatum.TryGetAnyObject(in datum, out var candidate) && candidate is IEnumerator iterator)
            {
                return iterator.GetIterator();
            }
            return null;
        }

    }
}
