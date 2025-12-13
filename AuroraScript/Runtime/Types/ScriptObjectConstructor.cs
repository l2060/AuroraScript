using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptObjectConstructor : BondingFunction
    {
        internal static ScriptObjectConstructor INSTANCE = new ScriptObjectConstructor();

        internal ScriptObjectConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetObject(0, out var scriptObject))
            {
                result = ScriptDatum.FromObject(new ScriptObject(scriptObject));
            }
            else
            {
                result = ScriptDatum.FromObject(new ScriptObject());
            }
        }


        internal static void KEYS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetObject(0, out var scriptObject))
            {
                result = ScriptDatum.FromArray(scriptObject.GetKeys());
            }
            else
            {
                result = ScriptDatum.FromArray(new ScriptArray());
            }
        }


        internal static void STRICT_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetRef(-10, out var var1) && args.TryGetRef(1, out var var2))
            {
                if (var1.Kind != var2.Kind)
                {
                    result = ScriptDatum.FromBoolean(false);
                    return;
                }
                switch (var1.Kind)
                {
                    case ValueKind.Null:
                        result = ScriptDatum.FromBoolean(true);
                        return;
                    case ValueKind.Boolean:
                        result = ScriptDatum.FromBoolean(var1.Boolean == var2.Boolean);
                        return;
                    case ValueKind.Number:
                        result = ScriptDatum.FromBoolean(var1.Number == var2.Number);
                        return;
                    case ValueKind.String:
                        result = ScriptDatum.FromBoolean(var1.String.Value == var2.String.Value);
                        return;
                    case ValueKind.Date:
                        result = ScriptDatum.FromBoolean(ScriptDatum.TryGetDate(in var1, out var date1) && ScriptDatum.TryGetDate(in var2, out var date2) && date1.DateTime.Equals(date2.DateTime));
                        return;
                    default:
                        result = ScriptDatum.FromBoolean(ScriptDatum.TryGetAnyObject(in var1, out var obj1) && ScriptDatum.TryGetAnyObject(in var2, out var obj2) && ReferenceEquals(obj1, obj2));
                        return;
                }
            }
        }


        internal static void VALUE_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            // TODO 待实现
            result = ScriptDatum.FromBoolean(true);
        }


        internal static void DEEP_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            // TODO 待实现
            result = ScriptDatum.FromBoolean(true);
        }

        internal static void ASSIGN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetObject(0, out var source))
            {
                var index = 1;
                while (args.TryGetObject(index, out var obj))
                {
                    source.CopyPropertysFrom(obj, true);
                    index++;
                }
                result = ScriptDatum.FromObject(source);
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal static void CLONE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetRef(0, out var datum))
            {
                result = ScriptDatum.Clone(in datum, false);
            }
        }

        internal static void DEEP_CLONE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetRef(0, out var datum))
            {
                result = ScriptDatum.Clone(in datum, true);
            }
        }


    }
}
