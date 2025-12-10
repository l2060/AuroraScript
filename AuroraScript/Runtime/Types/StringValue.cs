using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{
    public sealed partial class StringValue : ScriptValue, IEnumerator
    {

        private readonly String _value;

        private static readonly StringValue[] _charCache = new StringValue[256];

        public StringValue(String str) : base(Prototypes.StringValuePrototype)
        {
            _value = str;
        }


        public String Value => _value;



        public override void SetPropertyValue(StringValue key, ScriptObject value)
        {
            // Ignore
        }

        public override void SetPropertyValue(String key, ScriptObject value)
        {
            // Ignore
        }


        public override int GetHashCode()
        {
            return _value.GetHashCode();
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


        public override Boolean IsTrue()
        {
            return !String.IsNullOrEmpty(_value);
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
