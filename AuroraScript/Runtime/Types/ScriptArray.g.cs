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

        public static ScriptObject PUSH(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
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


        public static ScriptObject POP(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.PopDatum().ToObject();
            }
            throw new AuroraVMException("array is empty!");
        }



        public static ScriptObject REVERSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                var count = array._count;
                var items = array._items;
                if (items != null && count > 1)
                {
                    for (int left = 0, right = count - 1; left < right; left++, right--)
                    {
                        (items[left], items[right]) = (items[right], items[left]);
                    }
                }
                return array;
            }
            return thisObject;
        }

        public static ScriptObject UNSHIFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                if (args == null || args.Length == 0)
                {
                    return NumberValue.Of(array._count);
                }

                var insertCount = args.Length;
                array.EnsureCapacity(array._count + insertCount);
                for (int i = array._count - 1; i >= 0; i--)
                {
                    array._items[i + insertCount] = array._items[i];
                }
                for (int i = 0; i < insertCount; i++)
                {
                    array._items[i] = args[i];
                }
                array._count += insertCount;
                return NumberValue.Of(array._count);
            }
            return NumberValue.Zero;
        }


        public static ScriptObject SHIFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                if (array._count == 0)
                {
                    return ScriptObject.Null;
                }

                var first = array._items[0];
                for (int i = 1; i < array._count; i++)
                {
                    array._items[i - 1] = array._items[i];
                }
                array._count--;
                array._items[array._count] = ScriptDatum.FromNull();
                return first.ToObject();
            }
            return ScriptObject.Null;
        }
        public static ScriptObject CONCAT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                var result = new ScriptArray(array._count);
                AppendArrayContents(result, array);

                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        if (arg.TryGetArray(out var scriptArray))
                        {
                            AppendArrayContents(result, scriptArray);
                        }
                        else
                        {
                            result.PushDatum(arg);
                        }
                    }
                }

                return result;
            }
            return ScriptObject.Null;
        }





        public static ScriptObject SORT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
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







        public static ScriptObject JOIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (thisObject is ScriptArray array)
            {
                var separator = ",";
                if (args != null && args.Length > 0)
                {
                    separator = args[0].ToString();
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
                    var element = array.Get(i);
                    builder.Append(CoerceScriptValueToString(element));
                }
                return StringValue.Of(builder.ToString());
            }
            return thisObject;
        }



        private static string CoerceScriptValueToString(ScriptDatum value)
        {
            if (value.Kind == ValueKind.Null)
            {
                return string.Empty;
            }
            return value.ToString();
        }

        private static int CompareDatumForSort(ScriptDatum left, ScriptDatum right)
        {
            if (left.Kind == ValueKind.Number && right.Kind == ValueKind.Number)
            {
                return left.Number.CompareTo(right.Number);
            }
            var leftString = CoerceScriptValueToString(left);
            var rightString = CoerceScriptValueToString(right);
            return string.CompareOrdinal(leftString, rightString);
        }
        public static ScriptObject SLICE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
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
            if (args.Length > 1)
            {
                var arg1 = args[1];
                if (arg1.Kind == ValueKind.Number)
                {
                    var end = (Int32)arg1.Number;
                    return strValue.Slice(start, end);
                }
            }

            return strValue.Slice(start);
        }

        private static void AppendArrayContents(ScriptArray target, ScriptArray source)
        {
            if (target == null || source == null || source._count == 0)
            {
                return;
            }

            target.EnsureCapacity(target._count + source._count);
            Array.Copy(source._items, 0, target._items, target._count, source._count);
            target._count += source._count;
        }
    }
}
