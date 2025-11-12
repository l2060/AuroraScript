using AuroraScript.Runtime.Types;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue : ScriptValue, IEnumerator
    {

        private readonly String _value;

        private static readonly StringValue[] _charCache = new StringValue[256];

        public StringValue(String str) : base(Prototypes.StringValuePrototype)
        {
            _value = str;
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

        internal static StringValue FromChar(Char ch)
        {
            if (ch < _charCache.Length)
            {
                var cached = _charCache[ch];
                if (cached == null)
                {
                    cached = new StringValue(ch.ToString());
                    _charCache[ch] = cached;
                }
                return cached;
            }
            return new StringValue(ch.ToString());
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


        public override bool Equals(object obj)
        {
            if (obj is StringValue str)
            {
                return str._value == _value;
            }
            else if (obj is NumberValue num && Double.TryParse(_value, out var dVal))
            {
                return num.DoubleValue == dVal;
            }
            return false;
        }

        ItemIterator IEnumerator.GetIterator()
        {
            return ItemIterator.FromString(_value);
        }
    }
}
