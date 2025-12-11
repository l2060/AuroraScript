using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraScript.Core
{



    [StructLayout(LayoutKind.Explicit)]
    public partial struct ScriptDatum
    {
        [FieldOffset(0)]
        public ValueKind Kind;

        [FieldOffset(8)]
        public Double Number;

        [FieldOffset(8)]
        public Boolean Boolean;

        [FieldOffset(16)]
        public ScriptObject Object;




        public StringValue String
        {
            readonly get => Object as StringValue;
            set => Object = value;
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
                default:
                    return Object ?? ScriptObject.Null;
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
                    return !string.IsNullOrEmpty(String.Value);
                default:
                    return Object != null && Object.IsTrue();
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
                case ValueKind.Date:
                    return Datums.Date;
                case ValueKind.Array:
                    return Datums.Array;
                case ValueKind.Regex:
                    return Datums.Regex;
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
                default:
                    return Object.ToString();
            }
        }



    }
}

