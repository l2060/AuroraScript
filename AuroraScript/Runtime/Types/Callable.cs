using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Buffers;

namespace AuroraScript.Runtime.Types
{
    public abstract partial class Callable : ScriptObject
    {
        protected Callable(ClrMethodDelegate method)
        {
            Method = method;
            _prototype = Prototypes.CallablePrototype;
        }

        public String Name => Method.Method.DeclaringType.Name + "." + Method.Method.Name;

        public readonly ClrMethodDelegate Method;
        public abstract BoundFunction Bind(ScriptObject target);
        public abstract ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args);

        protected static ScriptObject[] ConvertArgs(ScriptDatum[] args, out ScriptObject[] rented)
        {
            rented = null;
            //if (args == null || args.Length == 0)
            //{
            //    return Array.Empty<ScriptObject>();
            //}
            //var array = ArrayPool<ScriptObject>.Shared.Rent(args.Length);
            //for (int i = 0; i < args.Length; i++)
            //{
            //    array[i] = args[i].ToObject();
            //}
            //rented = array;
            //return array;


            if (args == null || args.Length == 0)
            {
                return Array.Empty<ScriptObject>();
            }
            var converted = new ScriptObject[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                converted[i] = args[i].ToObject();
            }
            return converted;




        }

        protected static void ReturnArgs(ScriptObject[] rented)
        {
            if (rented != null)
            {
                ArrayPool<ScriptObject>.Shared.Return(rented, clearArray: true);
            }
        }

        public static ScriptObject BIND(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            var callable = thisObject as Callable;
            var target = (args != null && args.Length > 0) ? args[0] : ScriptObject.Null;
            return new BoundFunction(callable, target);
        }
    }
}
