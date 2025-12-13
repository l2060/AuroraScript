using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    public partial struct ScriptDatum
    {
        public static readonly ScriptDatum Null = ScriptDatum.FromNull();
        public static readonly ScriptDatum NaN = ScriptDatum.FromNumber(Double.NaN);
        public static readonly ScriptDatum True = ScriptDatum.FromBoolean(true);
        public static readonly ScriptDatum False = ScriptDatum.FromBoolean(false);




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
        public static void BooleanOf(bool value, out ScriptDatum dst)
        {
            dst = new ScriptDatum { Kind = ValueKind.Boolean, Boolean = value };
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NumberOf(double value, out ScriptDatum dst)
        {
            dst = new ScriptDatum { Kind = ValueKind.Number, Number = value };
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






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromObject(ScriptObject value)
        {
            switch (value)
            {
                case null:
                case NullValue:
                    return Null;

                case NumberValue numberValue:
                    return FromNumber(numberValue.DoubleValue);

                case BooleanValue booleanValue:
                    return FromBoolean(booleanValue.Value);

                case StringValue stringValue:
                    return FromString(stringValue);

                case ScriptArray scriptArray:
                    return FromArray(scriptArray);

                case ScriptDate scriptDate:
                    return FromDate(scriptDate);

                case ScriptRegex scriptRegex:
                    return FromRegex(scriptRegex);

                case ClosureFunction closureFunction:
                    return FromFunction(closureFunction);

                case ClrMethodBinding clrMethodBinding:
                    return FromClrFunction(clrMethodBinding);

                case ClrType clrTypeObject:
                    return FromClrType(clrTypeObject);

                case BondingFunction bonding:
                    return FromBonding(bonding);

                default:
                    return new ScriptDatum { Kind = ValueKind.Object, Object = value };
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptObject ToObject(in ScriptDatum d)
        {
            switch (d.Kind)
            {
                case ValueKind.Null:
                    return ScriptObject.Null;
                case ValueKind.Boolean:
                    return BooleanValue.Of(d.Boolean);
                case ValueKind.Number:
                    return NumberValue.Of(d.Number);
                case ValueKind.String:
                    return d.String;
                default:
                    return d.Object;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTrue(in ScriptDatum d)
        {
            switch (d.Kind)
            {
                case ValueKind.Null:
                    return false;
                case ValueKind.Boolean:
                    return d.Boolean;
                case ValueKind.Number:
                    var num = d.Number;
                    return num != 0 && !double.IsNaN(num);
                case ValueKind.String:
                    return !string.IsNullOrEmpty(d.String.Value);
                default:
                    return d.Object != ScriptObject.Null;
            }
        }

        public static void TypeOf(in ScriptDatum d, out ScriptDatum result)
        {
            switch (d.Kind)
            {
                case ValueKind.Null:
                    result = TypeNames.Null;
                    break;
                case ValueKind.Boolean:
                    result = TypeNames.Boolean;
                    break;
                case ValueKind.Number:
                    result = TypeNames.Number;
                    break;
                case ValueKind.String:
                    result = TypeNames.String;
                    break;
                case ValueKind.Object:
                    result = TypeNames.Object;
                    break;
                case ValueKind.Date:
                    result = TypeNames.Date;
                    break;
                case ValueKind.Array:
                    result = TypeNames.Array;
                    break;
                case ValueKind.Regex:
                    result = TypeNames.Regex;
                    break;
                case ValueKind.Function:
                    result = TypeNames.Function;
                    break;
                case ValueKind.ClrType:
                    result = TypeNames.ClrType;
                    break;
                case ValueKind.ClrFunction:
                    result = TypeNames.ClrFunction;
                    break;
                case ValueKind.ClrBonding:
                    result = TypeNames.ClrBonding;
                    break;
                default:
                    result = TypeNames.Object;
                    break;
            }
        }


        public static string ToString(in ScriptDatum d)
        {
            switch (d.Kind)
            {
                case ValueKind.Null:
                    return "null";
                case ValueKind.Boolean:
                    return d.Boolean.ToString();
                case ValueKind.Number:
                    return d.Number.ToString();
                case ValueKind.String:
                    return d.String.Value;
                default:
                    return d.Object.ToString();
            }
        }




    }
}
