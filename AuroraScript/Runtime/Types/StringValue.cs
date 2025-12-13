using AuroraScript.Runtime.Types;
using AuroraScript.Runtime.Types.Internal;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{
    public sealed partial class StringValue : ScriptValue, IEnumerator
    {
        public readonly String Value;

        private static readonly StringValue[] _charCache = new StringValue[256];

        public StringValue(String str) : base(Prototypes.StringValuePrototype)
        {
            Value = str;
        }


        public override void SetPropertyValue(String key, ScriptObject value)
        {
            // Ignore
        }


        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }


        public override string ToString()
        {
            return Value;
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
            return !String.IsNullOrEmpty(Value);
        }


        public override bool Equals(object obj)
        {
            if (obj is StringValue str)
            {
                return str.Value == Value;
            }
            else if (obj is NumberValue num && Double.TryParse(Value, out var dVal))
            {
                return num.DoubleValue == dVal;
            }
            return false;
        }

        ItemIterator IEnumerator.GetIterator()
        {
            return ItemIterator.FromString(Value);
        }


    }
}
