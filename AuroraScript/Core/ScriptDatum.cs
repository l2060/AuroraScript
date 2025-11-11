using AuroraScript.Runtime.Base;
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
                default:
                    return false;
            }
        }
    }
}

