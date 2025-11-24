using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System.Runtime.CompilerServices;

namespace AuroraScript.Core
{
    public struct ScriptDatum
    {
        public ValueKind Kind;
        public double Number;
        public bool Boolean;
        public StringValue String;
        public ScriptObject Object;

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
        public static ScriptDatum FromArray(ScriptArray value)
        {
            return new ScriptDatum { Kind = ValueKind.Array, Object = value };
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromClosure(ClosureFunction value)
        {
            return new ScriptDatum { Kind = ValueKind.Function, Object = value };
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromClrMethodBinding(ClrMethodBinding value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrFunction, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromClrType(ClrTypeObject value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrType, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromBonding(BondingFunction value)
        {
            return new ScriptDatum { Kind = ValueKind.ClrBonding, Object = value };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptDatum FromObject(ScriptObject value)
        {
            if (value == null || value == ScriptObject.Null)
            {
                return FromNull();
            }
            if (value is NumberValue numberValue)
            {
                return FromNumber(numberValue.DoubleValue);
            }
            if (value is BooleanValue booleanValue)
            {
                return FromBoolean(booleanValue.Value);
            }
            if (value is StringValue stringValue)
            {
                return FromString(stringValue);
            }
            if (value is ScriptArray scriptArray)
            {
                return FromArray(scriptArray);
            }
            if (value is ClosureFunction closureFunction)
            {
                return FromClosure(closureFunction);
            }
            if (value is ClrMethodBinding clrMethodBinding)
            {
                return FromClrMethodBinding(clrMethodBinding);
            }
            if (value is ClrTypeObject clrTypeObject)
            {
                return FromClrType(clrTypeObject);
            }
            if (value is BondingFunction bonding)
            {
                return FromBonding(bonding);
            }
            return new ScriptDatum { Kind = ValueKind.Object, Object = value };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptObject ToObject()
        {
            switch (Kind)
            {
                case ValueKind.Null:
                    return ScriptObject.Null;
                case ValueKind.Boolean:
                    return BooleanValue.Of(Boolean);
                case ValueKind.Number:
                    return NumberValue.Of(Number);
                case ValueKind.String:
                    return String;
                case ValueKind.Object:
                    return Object ?? ScriptObject.Null;
                case ValueKind.Array:
                    return Object ?? ScriptObject.Null;
                case ValueKind.ClrType:
                    return Object ?? ScriptObject.Null;
                case ValueKind.Function:
                    return Object ?? ScriptObject.Null;
                case ValueKind.ClrFunction:
                    return Object ?? ScriptObject.Null;
                case ValueKind.ClrBonding:
                    return Object ?? ScriptObject.Null;

                default:
                    return ScriptObject.Null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTrue()
        {
            switch (Kind)
            {
                case ValueKind.Null:
                    return false;
                case ValueKind.Boolean:
                    return Boolean;
                case ValueKind.Number:
                    return Number != 0 && !double.IsNaN(Number);
                case ValueKind.String:
                    return !string.IsNullOrEmpty(String?.Value);
                case ValueKind.Object:
                    return Object?.IsTrue() ?? false;
                case ValueKind.Array:
                    return Object?.IsTrue() ?? false;
                default:
                    return false;
            }
        }

        public ScriptDatum TypeOf()
        {
            switch (Kind)
            {
                case ValueKind.Null:
                    return Datums.Null;
                case ValueKind.Boolean:
                    return Datums.Boolean;
                case ValueKind.Number:
                    return Datums.Number;
                case ValueKind.String:
                    return Datums.String;
                case ValueKind.Object:
                    return Datums.Object;
                case ValueKind.Array:
                    return Datums.Array;
                case ValueKind.Function:
                    return Datums.Function;
                case ValueKind.ClrType:
                    return Datums.ClrType;
                case ValueKind.ClrFunction:
                    return Datums.ClrFunction;
                case ValueKind.ClrBonding:
                    return Datums.ClrBonding;
                default:
                    return Datums.Object;
            }
        }


        public override string ToString()
        {
            switch (Kind)
            {
                case ValueKind.Null:
                    return "null";
                case ValueKind.Boolean:
                    return Boolean.ToString();
                case ValueKind.Number:
                    return Number.ToString();
                case ValueKind.String:
                    return String.Value;
                case ValueKind.Object:
                    return Object?.ToString();
                case ValueKind.Array:
                    return Object?.ToString();
                case ValueKind.Function:
                    return Object?.ToString();
                case ValueKind.ClrType:
                    return Object?.ToString();
                case ValueKind.ClrFunction:
                    return Object?.ToString();
                case ValueKind.ClrBonding:
                    return Object?.ToString();

                default:
                    return "";
            }
        }



    }
}

