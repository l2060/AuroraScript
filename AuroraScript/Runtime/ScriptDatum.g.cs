using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using AuroraScript.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace AuroraScript.Runtime
{
    public partial struct ScriptDatum
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetAnyObject(in ScriptDatum d, out ScriptObject value)
        {
            value = d.Object;
            return value != null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetObject(in ScriptDatum d, out ScriptObject value)
        {
            if (d.Kind == ValueKind.Object)
            {
                value = d.Object;
                return true;
            }
            value = null;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetArray(in ScriptDatum d, out ScriptArray value)
        {
            if (d.Kind == ValueKind.Array)
            {
                value = (ScriptArray)d.Object;
                return true;
            }
            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetRegex(in ScriptDatum d, out ScriptRegex value)
        {
            if (d.Kind == ValueKind.Regex)
            {
                value = (ScriptRegex)d.Object;
                return true;
            }
            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetClrType(in ScriptDatum d, out ClrType value)
        {
            if (d.Kind == ValueKind.ClrType)
            {
                value = (ClrType)d.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetClrBonding(in ScriptDatum d, out BondingFunction value)
        {
            if (d.Kind == ValueKind.ClrBonding)
            {
                value = (BondingFunction)d.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetClrFunction(in ScriptDatum d, out ClrMethodBinding value)
        {
            if (d.Kind == ValueKind.ClrFunction)
            {
                value = (ClrMethodBinding)d.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Boolean TryGetClrInvokable(in ScriptDatum d, out IClrInvokable value)
        {
            var k = d.Kind;
            if (k == ValueKind.ClrFunction || k == ValueKind.ClrType)
            {
                value = (IClrInvokable)d.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetDate(in ScriptDatum d, out ScriptDate value)
        {
            if (d.Kind == ValueKind.Date)
            {
                value = (ScriptDate)d.Object;
                return true;
            }
            value = null;
            return false;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean TryGetFunction(in ScriptDatum d, out ClosureFunction value)
        {
            if (d.Kind == ValueKind.Function)
            {
                value = (ClosureFunction)d.Object;
                return true;
            }
            value = null;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToNumber(in ScriptDatum d, out double value)
        {
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
                        out value
                    );
            }
            value = default;
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryToInteger(in ScriptDatum d, out long value)
        {
            switch (d.Kind)
            {
                case ValueKind.Number:
                    value = (long)d.Number;
                    return true;

                case ValueKind.Boolean:
                    value = d.Boolean ? 1 : 0;
                    return true;

                case ValueKind.String:
                    return long.TryParse(
                        d.String.Value,
                        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture,
                        out value
                    );
            }
            value = default;
            return false;
        }



        public static ScriptDatum Clone(in ScriptDatum d, bool deepth = false)
        {
            switch (d.Kind)
            {
                case ValueKind.Null:
                case ValueKind.Number:
                case ValueKind.Boolean:
                case ValueKind.String:
                    return d;
                default:
                    return RuntimeHelper.Clone(d, deepth);
            }
        }
    }
}
