using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
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
        public static bool TryGetNumber(this Span<ScriptDatum> source, Int32 index, out double value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];
                if (datum.Kind == ValueKind.Number)
                {
                    value = datum.Number;
                    return true;
                }
                if (datum.Kind == ValueKind.Boolean)
                {
                    value = datum.Boolean ? 1 : 0;
                    return true;
                }
                if (datum.Kind == ValueKind.String)
                {
                    return Double.TryParse(datum.String.Value, out value);
                }
            }
            value = Double.NaN;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetInteger(this Span<ScriptDatum> source, Int32 index, out Int64 value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];
                if (datum.Kind == ValueKind.Number)
                {
                    value = (Int64)datum.Number;
                    return true;
                }
                if (datum.Kind == ValueKind.Boolean)
                {
                    value = datum.Boolean ? 1 : 0;
                    return true;
                }
                if (datum.Kind == ValueKind.String)
                {
                    return Int64.TryParse(datum.String.Value, out value);
                }
            }
            value = 0;
            return false;
        }




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetString(this Span<ScriptDatum> source, Int32 index, out String value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];

                if (datum.Kind == ValueKind.Number)
                {
                    value = datum.Number.ToString();
                    return true;
                }
                if (datum.Kind == ValueKind.Boolean)
                {
                    value = datum.Boolean.ToString();
                    return true;
                }
                if (datum.Kind == ValueKind.String)
                {
                    value = datum.String.Value;
                    return true;
                }
            }
            value = String.Empty;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetObject(this Span<ScriptDatum> source, Int32 index, out ScriptObject value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];
                if (datum.Object != null)
                {
                    value = datum.Object;
                    return true;
                }
            }
            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet(this Span<ScriptDatum> source, Int32 index, out ScriptDatum value)
        {
            if (index >= 0 && index < source.Length)
            {
                value = source[index];
                return true;
            }
            value = default;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFunction(this Span<ScriptDatum> source, Int32 index, out ClosureFunction value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];
                if (datum.Kind == ValueKind.Number)
                {
                    value = datum.Object as ClosureFunction;
                    return true;
                }
            }
            value = default;
            return false;
        }








        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetBoolean(this Span<ScriptDatum> source, Int32 index, out Boolean value)
        {
            if (index >= 0 && index < source.Length)
            {
                var datum = source[index];

                if (datum.Kind == ValueKind.Number)
                {
                    value = datum.Number != 0;
                    return true;
                }
                if (datum.Kind == ValueKind.Boolean)
                {
                    value = datum.Boolean;
                    return true;
                }
                if (datum.Kind == ValueKind.String)
                {
                    value = !String.IsNullOrWhiteSpace(datum.String.Value);
                    return true;
                }
                if (datum.Object != null)
                {
                    value = datum.Object.IsTrue();
                    return true;
                }
            }
            value = false;
            return false;
        }

    }
}
