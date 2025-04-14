using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AuroraScript.Runtime.Base
{

    public partial class NumberValue : ScriptValue
    {
        private readonly Double _value;

        public NumberValue(Double str = 0)
        {
            _value = str;
            IsFrozen = true;
            _prototype = NumberValue.Prototype;
        }


        public override string ToString()
        {
            return _value.ToString();
        }

        public override string ToDisplayString()
        {
            return _value.ToString();
        }


        public static NumberValue Of(Double value)
        {
            return new NumberValue(value);
        }






        public static NumberValue operator +(NumberValue a, NumberValue b)
        {
            return new NumberValue(a._value + b._value);
        }


        public static NumberValue operator -(NumberValue a, NumberValue b)
        {
            return new NumberValue(a._value - b._value);
        }

        public static NumberValue operator *(NumberValue a, NumberValue b)
        {
            return new NumberValue(a._value * b._value);
        }

        public static NumberValue operator /(NumberValue a, NumberValue b)
        {
            return new NumberValue(a._value / b._value);
        }

        public static NumberValue operator %(NumberValue a, NumberValue b)
        {
            return new NumberValue(a._value % b._value);
        }
        public static NumberValue operator &(NumberValue a, NumberValue b)
        {
            return new NumberValue((Int64)a._value & (Int64)b._value);
        }

        public static NumberValue operator <<(NumberValue a, NumberValue b)
        {
            return new NumberValue((Int64)a._value << (Byte)b._value);
        }

        public static NumberValue operator >>(NumberValue a, NumberValue b)
        {
            return new NumberValue((Int64)a._value >> (Byte)b._value);
        }

        public static NumberValue operator ~(NumberValue a)
        {
            return new NumberValue(~(Int64)a._value);
        }
        


        public static NumberValue operator |(NumberValue a, NumberValue b)
        {
            return new NumberValue((Int64)a._value | (Int64)b._value);
        }

        public static NumberValue operator ^(NumberValue a, NumberValue b)
        {
            return new NumberValue((Int64)a._value ^ (Int64)b._value);
        }





    }
}