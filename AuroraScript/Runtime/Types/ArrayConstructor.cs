using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using System;

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

            var hasMap = args.Length > 1 && IsCallable(args[1]);
            var mapDatum = hasMap ? args[1] : default;
            var thisArg = args.Length > 2 ? args[2].ToObject() : ScriptObject.Null;
            thisArg ??= ScriptObject.Null;

            var executeOptions = context?.ExecuteOptions ?? ExecuteOptions.Default;
            var result = new ScriptArray();
            var index = 0;

            while (iterator.HasValue())
            {
                var current = iterator.Value() ?? ScriptObject.Null;
                if (hasMap)
                {
                    current = ApplyMapFunction(context, mapDatum, thisArg, current, index, result, executeOptions);
                }
                result.Push(current);
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
                array.SetDatum(i, args[i]);
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
            var candidate = datum.ToObject();
            if (candidate is IEnumerator iterator)
            {
                return iterator.GetIterator();
            }
            return null;
        }

        private static Boolean IsCallable(ScriptDatum datum)
        {
            if (datum.Kind == ValueKind.Function && datum.Object is ClosureFunction)
            {
                return true;
            }

            if (datum.Kind == ValueKind.ClrBonding && datum.Object is Callable)
            {
                return true;
            }

            if ((datum.Kind == ValueKind.ClrFunction || datum.Kind == ValueKind.ClrType) && datum.Object is IClrInvokable)
            {
                return true;
            }

            return false;
        }

        private static ScriptObject ApplyMapFunction(
            ExecuteContext context,
            ScriptDatum mapDatum,
            ScriptObject thisArg,
            ScriptObject value,
            Int32 index,
            ScriptArray target,
            ExecuteOptions executeOptions)
        {
            value ??= ScriptObject.Null;
            thisArg ??= ScriptObject.Null;
            target ??= new ScriptArray();

            if (mapDatum.Kind == ValueKind.Function && mapDatum.Object is ClosureFunction closure)
            {
                return InvokeClosureMap(closure, executeOptions, value, index, target, thisArg);
            }

            var callArgs = new[]
            {
                ScriptDatum.FromObject(value),
                ScriptDatum.FromNumber(index),
                ScriptDatum.FromObject(target),
                ScriptDatum.FromObject(thisArg)
            };

            if (mapDatum.Kind == ValueKind.ClrBonding && mapDatum.Object is Callable callable)
            {
                return callable.Invoke(context, thisArg, callArgs) ?? ScriptObject.Null;
            }

            if ((mapDatum.Kind == ValueKind.ClrFunction || mapDatum.Kind == ValueKind.ClrType) && mapDatum.Object is IClrInvokable invokable)
            {
                var resultDatum = invokable.Invoke(context, thisArg, callArgs);
                return resultDatum.ToObject();
            }

            return value;
        }

        private static ScriptObject InvokeClosureMap(
            ClosureFunction callback,
            ExecuteOptions executeOptions,
            ScriptObject value,
            Int32 index,
            ScriptArray target,
            ScriptObject thisArg)
        {
            executeOptions ??= ExecuteOptions.Default;
            var args = new ScriptObject[]
            {
                value ?? ScriptObject.Null,
                NumberValue.Of(index),
                target ?? ScriptObject.Null,
                thisArg ?? ScriptObject.Null
            };

            var execContext = callback.Invoke(executeOptions, args);
            try
            {
                execContext.Done();
                if (execContext.Status == ExecuteStatus.Error)
                {
                    throw execContext.Error ?? new AuroraRuntimeException("Array.from callback execution failed.");
                }

                return execContext.Result ?? ScriptObject.Null;
            }
            finally
            {
                execContext.Dispose();
            }
        }
    }
}
