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
        public new static void LENGTH(ScriptObject thisObject, ref ScriptDatum result)
        {
            var strValue = thisObject as ScriptArray;
            result = ScriptDatum.FromNumber(strValue.Length);
        }

        public static void PUSH(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array && args != null)
            {
                foreach (var datum in args)
                {
                    array.PushDatum(datum);
                }
            }
        }


        public static void POP(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array)
            {
                result = array.PopDatum();
                return;
            }
            throw new AuroraVMException("array is empty!");
        }



        public static void REVERSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
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
                result = ScriptDatum.FromArray(array);
            }
        }

        public static void UNSHIFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array)
            {
                if (args == null || args.Length == 0)
                {
                    result = ScriptDatum.FromNumber(array._count);
                    return;
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
                result = ScriptDatum.FromNumber(array._count);
                return;
            }
            result = ScriptDatum.FromNumber(0);
        }


        public static void SHIFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array)
            {
                if (array._count == 0) return;
                var first = array._items[0];
                for (int i = 1; i < array._count; i++)
                {
                    array._items[i - 1] = array._items[i];
                }
                array._count--;
                array._items[array._count] = ScriptDatum.Null;
                result = first;
            }
        }
        public static void CONCAT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array)
            {
                var newArray = new ScriptArray(array._count);
                AppendArrayContents(newArray, array);
                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        if (arg.TryGetArray(out var scriptArray))
                        {
                            AppendArrayContents(newArray, scriptArray);
                        }
                        else
                        {
                            newArray.PushDatum(arg);
                        }
                    }
                }
                result = ScriptDatum.FromArray(newArray);
            }
        }





        public static void SORT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not ScriptArray array)
            {
                result = ScriptDatum.FromObject(thisObject);
                return;
            }

            var count = array._count;
            if (count <= 1)
            {
                result = ScriptDatum.FromObject(thisObject);
                return;
            }
            var buffer = new ScriptDatum[count];
            Array.Copy(array._items, buffer, count);
            Array.Sort(buffer, CompareDatumForSort);
            Array.Copy(buffer, 0, array._items, 0, count);
            result = ScriptDatum.FromArray(array);
        }







        public static void JOIN(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is ScriptArray array)
            {
                args.TryGetString(0, out var separator);
                if (array.Length == 0)
                {
                    result = ScriptDatum.FromString(string.Empty);
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
                result = ScriptDatum.FromString(builder.ToString());
                return;
            }
            result = ScriptDatum.FromString(string.Empty);
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
        public static void SLICE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not ScriptArray array)
            {
                result = ScriptDatum.Null;
                return;
            }

            if (args == null || args.Length == 0)
            {
                result = ScriptDatum.FromArray(array);
                return;
            }
            args.TryGetInteger(0, out var start);
            if (args.TryGetInteger(1, out var end))
            {
                result = ScriptDatum.FromArray(array.Slice((int)start, (int)end));
            }
            else
            {
                result = ScriptDatum.FromArray(array.Slice((int)start));
            }

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
