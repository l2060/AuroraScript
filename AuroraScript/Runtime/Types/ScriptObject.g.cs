using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        internal static readonly ScriptObject Null = NullValue.Instance;

        internal static void LENGTH(ScriptObject thisObject, ref ScriptDatum result)
        {
            if (thisObject is ScriptObject obj)
            {
                result = ScriptDatum.FromNumber(obj._properties.Count);
            }
            else
            {
                result = ScriptDatum.FromNumber(0);
            }
        }

        internal static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
        {
            result = ScriptDatum.FromString(thisObject.ToString());
        }


        internal static void STRICT_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
        {
            if (args.TryGet(-10, out var var1) && args.TryGet(1, out var var2))
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
                        result = ScriptDatum.FromBoolean(var1.TryGetDate(out var date1) && var2.TryGetDate(out var date2) && date1.DateTime.Equals(date2.DateTime));
                        return;
                    default:
                        result = ScriptDatum.FromBoolean(var1.TryGetAnyObject(out var obj1) && var2.TryGetAnyObject(out var obj2) && ReferenceEquals(obj1, obj2));
                        return;
                }
            }
        }


        internal static void VALUE_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
        {
            // TODO 待实现
            result = ScriptDatum.FromBoolean(true);
        }


        internal static void DEEP_EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
        {
            // TODO 待实现
            result = ScriptDatum.FromBoolean(true);
        }

        internal static void ASSIGN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
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

    }
}
