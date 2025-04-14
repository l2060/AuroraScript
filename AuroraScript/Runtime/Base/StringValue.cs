using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue : ScriptValue
    {
        private readonly String _value;

        public StringValue(String str)
        {
            _value = str;
            IsFrozen = true;
            _prototype = StringValue.Prototype;
        }


        public String Value => _value;




        public NumberValue Length
        {
            get
            {
                return new NumberValue(_value.Length);
            }
        }


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


        public static StringValue Of(String value)
        {
            return new StringValue(value);
        }



        public static StringValue operator +(ScriptObject a, StringValue b)
        {
            return new StringValue(a.ToString() + b._value);
        }



        public static StringValue operator +(StringValue a, ScriptObject b)
        {
            return new StringValue(a._value + b.ToString());
        }

        public static StringValue operator +(StringValue a, StringValue b)
        {
            return new StringValue(a._value + b._value);
        }



    }
}
