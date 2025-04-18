using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue : ScriptValue
    {
        public readonly static StringValue Empty = new StringValue("");


        private readonly String _value;

        public StringValue(String str) : base()
        {
            _value = str;
            _prototype = StringValue.Prototype;
        }


        public String Value => _value;



        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToDisplayString()
        {
            return $"\"{_value}\"";
        }


        public override string ToString()
        {
            return _value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringValue Of(String value)
        {
            return new StringValue(value);
        }

        public static StringValue operator +(ScriptObject a, StringValue b)
        {
            return Of(a.ToString() + b._value);
        }

        public static StringValue operator +(StringValue a, ScriptObject b)
        {
            return Of(a._value + b.ToString());
        }

        public static StringValue operator +(StringValue a, StringValue b)
        {
            return Of(a._value + b._value);
        }


        public override Boolean IsTrue()
        {
            return _value != null && _value.Length > 0;
        }
    }
}
