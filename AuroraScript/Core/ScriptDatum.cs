using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Core
{

    public partial struct ScriptDatum
    {
        public ValueKind Kind;
        private UnionNumber _numeric;
        private ScriptObject _object;

        public double Number
        {
            readonly get => _numeric.DoubleValue;
            set => _numeric.DoubleValue = value;
        }

        public bool Boolean
        {
            readonly get => _numeric.Int64Value != 0;
            set => _numeric.Int64Value = value ? 1L : 0L;
        }

        public ScriptObject Object
        {
            readonly get => _object;
            set => _object = value;
        }

        public StringValue String
        {
            readonly get => _object as StringValue;
            set => _object = value;
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
            if (value is ScriptDate scriptDate)
            {
                return FromDate(scriptDate);
            }
            if (value is ScriptRegex scriptRegex)
            {
                return FromRegex(scriptRegex);
            }
            if (value is ClosureFunction closureFunction)
            {
                return FromFunction(closureFunction);
            }
            if (value is ClrMethodBinding clrMethodBinding)
            {
                return FromClrFunction(clrMethodBinding);
            }
            if (value is ClrType clrTypeObject)
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
                    return !string.IsNullOrEmpty(String?.Value);
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

