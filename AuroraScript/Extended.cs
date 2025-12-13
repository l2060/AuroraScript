using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;



namespace AuroraScript
{
    public static class Extended
    {

        /// <summary>
        /// 检测 ValueKind中是否包含另外一个flag
        /// </summary>
        /// <param name="valueKind"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean Include(this ValueKind valueKind, ValueKind flag)
        {
            return (valueKind & flag) != 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetNumber(this Span<ScriptDatum> source, int index, out double value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                switch (d.Kind)
                {
                    case ValueKind.Number:
                        value = d.Number;
                        return true;

                    case ValueKind.Boolean:
                        value = d.Boolean ? 1.0 : 0.0;
                        return true;

                    case ValueKind.String:
                        return double.TryParse(
                            d.String.Value,
                            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture,
                            out value);
                }
            }
            value = double.NaN;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInteger(this Span<ScriptDatum> source, Int32 index, out Int64 value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                switch (d.Kind)
                {
                    case ValueKind.Number:
                        value = (Int64)d.Number;
                        return true;

                    case ValueKind.Boolean:
                        value = d.Boolean ? 1 : 0;
                        return true;

                    case ValueKind.String:
                        return Int64.TryParse(
                            d.String.Value,
                            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture,
                            out value);
                }
            }
            value = 0;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetStrictNumber(this Span<ScriptDatum> source, int index, out double value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                if (d.Kind == ValueKind.Number)
                {
                    value = d.Number;
                    return true;
                }
            }
            value = 0;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetString(this Span<ScriptDatum> source, int index, out string value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                switch (d.Kind)
                {
                    case ValueKind.String:
                        value = d.String.Value;
                        return true;

                    case ValueKind.Null:
                        value = "null";
                        return true;

                    case ValueKind.Number:
                        value = d.Number.ToString(CultureInfo.InvariantCulture);
                        return true;

                    case ValueKind.Boolean:
                        value = d.Boolean ? "true" : "false";
                        return true;
                }
            }
            value = string.Empty;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetObject(this Span<ScriptDatum> source, int index, out ScriptObject value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                if (d.Kind >= ValueKind.Object)
                {
                    value = d.Object;
                    return true;
                }
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly ScriptDatum GetRefUnchecked(this Span<ScriptDatum> source, int index)
        {
            return ref source[index];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetRef(this Span<ScriptDatum> source, int index, out ScriptDatum value)
        {
            if ((uint)index < (uint)source.Length)
            {
                value = source[index];
                return true;
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFunction(this Span<ScriptDatum> source, int index, out ClosureFunction value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                if (d.Kind == ValueKind.Function)
                {
                    value = d.Object as ClosureFunction;
                    return value != null;
                }
            }
            value = null;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetBoolean(this Span<ScriptDatum> source, int index, out bool value)
        {
            if ((uint)index < (uint)source.Length)
            {
                ref readonly var d = ref source[index];
                switch (d.Kind)
                {
                    case ValueKind.Boolean:
                        value = d.Boolean;
                        return true;

                    case ValueKind.Number:
                        value = d.Number != 0;
                        return true;

                    case ValueKind.String:
                        value = d.String.Value.Length != 0;
                        return true;

                    default:
                        if (d.Object != null)
                        {
                            value = d.Object != ScriptObject.Null;
                            return true;
                        }
                        break;
                }
            }
            value = false;
            return false;
        }

    }
}
