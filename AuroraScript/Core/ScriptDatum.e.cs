using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Core
{
    public partial struct ScriptDatum
    {





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromNull()
        {
            return new ScriptDatum { Kind = ValueKind.Null };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromBoolean(bool value)
        {
            return new ScriptDatum { Kind = ValueKind.Boolean, Boolean = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromNumber(double value)
        {
            return new ScriptDatum { Kind = ValueKind.Number, Number = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromString(StringValue value)
        {
            return new ScriptDatum { Kind = ValueKind.String, String = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromString(String value)
        {
            return new ScriptDatum { Kind = ValueKind.String, String = StringValue.Of(value) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromArray(ScriptArray value)
        {
            return new ScriptDatum { Kind = ValueKind.Array, Object = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromDate(ScriptDate date)
        {
            return new ScriptDatum { Kind = ValueKind.Date, Object = date };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromDate(DateTime date)
        {
            return new ScriptDatum { Kind = ValueKind.Date, Object = new ScriptDate(date) };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromDate(DateTimeOffset date)
        {
            return new ScriptDatum { Kind = ValueKind.Date, Object = new ScriptDate(date) };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromRegex(ScriptRegex value)
        {
            return new ScriptDatum { Kind = ValueKind.Regex, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromFunction(ClosureFunction value)
        {
            return new ScriptDatum { Kind = ValueKind.Function, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromClrFunction(ClrMethodBinding value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrFunction, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromClrType(ClrType value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrType, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromBonding(BondingFunction value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrBonding, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromBonding(ClrDatumDelegate value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrBonding, Object = new BondingFunction(value) };
        }




    }
}
