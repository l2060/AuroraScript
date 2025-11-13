using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptArray
    {
        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptArray;
            return NumberValue.Of(strValue.Length);
        }

        public static ScriptObject PUSH(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is ScriptArray array && args != null)
            {
                foreach (var datum in args)
                {
                    array.PushDatum(datum);
                }
            }
            return ScriptObject.Null;
        }


        public static ScriptObject POP(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if(thisObject is ScriptArray array)
            {
                return array.PopDatum().ToObject();
            }
            throw new AuroraVMException("array is empty!");
        }


        public static ScriptObject SORT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not ScriptArray array)
            {
                return thisObject;
            }

            var count = array._count;
            if (count <= 1)
            {
                return thisObject;
            }

            var buffer = new ScriptDatum[count];
            Array.Copy(array._items, buffer, count);

            Array.Sort(buffer, CompareDatumForSort);

            Array.Copy(buffer, 0, array._items, 0, count);
            return thisObject;
        }



        



        public static ScriptObject JOIN(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is ScriptArray array)
            {
                var separator = ",";
                if (args != null && args.Length > 0)
                {
                    separator = CoerceDatumToString(args[0], ",");
                }

                if (array.Length == 0)
                {
                    return StringValue.Empty;
                }

                var builder = new StringBuilder();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(separator);
                    }
                    var element = array.GetElement(i);
                    builder.Append(CoerceScriptValueToString(element));
                }
                return StringValue.Of(builder.ToString());
            }
            return thisObject;
        }

        private static string CoerceDatumToString(ScriptDatum datum, string fallback)
        {
            if (datum.Kind == ValueKind.String && datum.String != null)
            {
                return datum.String.Value;
            }

            var value = datum.ToObject();
            if (value is StringValue str)
            {
                return str.Value;
            }
            if (value is NumberValue number)
            {
                return number.DoubleValue.ToString(CultureInfo.InvariantCulture);
            }
            if (value is BooleanValue bol)
            {
                return bol.Value ? "true" : "false";
            }
            if (value == null || ReferenceEquals(value, ScriptObject.Null))
            {
                return string.Empty;
            }
            return value.ToString() ?? fallback;
        }

        private static string CoerceScriptValueToString(ScriptObject value)
        {
            if (value == null || ReferenceEquals(value, ScriptObject.Null))
            {
                return string.Empty;
            }
            if (value is StringValue str)
            {
                return str.Value;
            }
            if (value is NumberValue number)
            {
                return number.DoubleValue.ToString(CultureInfo.InvariantCulture);
            }
            if (value is BooleanValue bol)
            {
                return bol.Value ? "true" : "false";
            }
            return value.ToString() ?? string.Empty;
        }

        private static int CompareDatumForSort(ScriptDatum left, ScriptDatum right)
        {
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                return left.Number.CompareTo(right.Number);
            }

            var leftString = CoerceScriptValueToString(left.ToObject());
            var rightString = CoerceScriptValueToString(right.ToObject());
            return string.CompareOrdinal(leftString, rightString);
        }
        public static ScriptObject SLICE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not ScriptArray strValue)
            {
                return thisObject;
            }

            if (args == null || args.Length == 0)
            {
                return thisObject;
            }

            var start = 0;
            var arg0 = args[0];
            if (arg0.Kind == ValueKind.Number)
            {
                start = (Int32)arg0.Number;
            }
            else
            {
                var startObj = arg0.ToObject();
                if (startObj is NumberValue posNum)
                {
                    start = posNum.Int32Value;
                }
            }

            if (args.Length > 1)
            {
                var arg1 = args[1];
                if (arg1.Kind == ValueKind.Number)
                {
                    var end = (Int32)arg1.Number;
                    return strValue.Slice(start, end);
                }

                var endObj = arg1.ToObject();
                if (endObj is NumberValue lenNum)
                {
                    return strValue.Slice(start, lenNum.Int32Value);
                }
            }

            return strValue.Slice(start);
        }


    }
}
