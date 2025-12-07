using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        public static readonly ScriptObject Null = NullValue.Instance;

        public static ScriptObject LENGTH(ScriptObject thisObject)
        {
            if (thisObject is ScriptObject obj && obj._properties != null)
            {
                return NumberValue.Of(obj._properties.Count);
            }
            return NumberValue.Zero;
        }

        public static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return StringValue.Of(thisObject?.ToString() ?? StringValue.Empty.Value);
        }





        public static ScriptObject EQUAL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.TryGet(-10, out var var1) && args.TryGet(1, out var var2))
            {
                if (var1.Kind != var2.Kind) return BooleanValue.False;
                switch (var1.Kind)
                {
                    case ValueKind.Null:
                        return BooleanValue.True;
                    case ValueKind.Boolean:
                        return BooleanValue.Of(var1.Boolean == var2.Boolean);
                    case ValueKind.Number:
                        return BooleanValue.Of(var1.Number == var2.Number);
                    case ValueKind.String:
                        return BooleanValue.Of(var1.String.Value == var2.String.Value);
                    case ValueKind.Date:
                        return BooleanValue.Of(var1.TryGetDate(out var date1) && var2.TryGetDate(out var date2) && date1.DateTime.Equals(date2.DateTime));
                    default:
                        return BooleanValue.Of(var1.TryGetAnyObject(out var obj1) && var2.TryGetAnyObject(out var obj2) && ReferenceEquals(obj1, obj2));
                }
            }
            return BooleanValue.False;
        }



    }
}
