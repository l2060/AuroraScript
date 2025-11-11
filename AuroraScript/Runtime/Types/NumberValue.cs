using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{

    public partial class NumberValue : ScriptValue
    {
        private readonly Double _value;



        public NumberValue(Double dValue = 0) : base(Prototypes.NumberValuePrototype)
        {
            _value = dValue;
        }


        public Double DoubleValue => _value;

        public Int32 Int32Value => (Int32)_value;

        public Int64 Int64Value => (Int64)_value;

        public override string ToString()
        {
            return _value.ToString();
        }

        public override string ToDisplayString()
        {
            return _value.ToString();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Boolean IsTrue()
        {
            return _value != 0;
        }

        public static NumberValue operator -(NumberValue a)
        {
            return NumberValue.Of(-a._value);
        }


        public static BooleanValue operator <(NumberValue a, NumberValue b)
        {
            return BooleanValue.Of(a._value < b._value);
        }
        public static BooleanValue operator <=(NumberValue a, NumberValue b)
        {
            return BooleanValue.Of(a._value <= b._value);
        }

        public static BooleanValue operator >(NumberValue a, NumberValue b)
        {
            return BooleanValue.Of(a._value > b._value);
        }
        public static BooleanValue operator >=(NumberValue a, NumberValue b)
        {
            return BooleanValue.Of(a._value >= b._value);
        }

        public static NumberValue operator +(NumberValue a, Int32 b)
        {
            return NumberValue.Of(a._value + b);
        }

        public static NumberValue operator -(NumberValue a, Int32 b)
        {
            return NumberValue.Of(a._value - b);
        }

        public static NumberValue operator %(NumberValue a, NumberValue b)
        {
            return NumberValue.Of(a._value % b._value);
        }


        public static NumberValue operator +(NumberValue a, NumberValue b)
        {
            return NumberValue.Of(a._value + b._value);
        }


        public static NumberValue operator -(NumberValue a, NumberValue b)
        {
            return NumberValue.Of(a._value - b._value);
        }

        public static NumberValue operator *(NumberValue a, NumberValue b)
        {
            return NumberValue.Of(a._value * b._value);
        }

        public static NumberValue operator /(NumberValue a, NumberValue b)
        {
            return NumberValue.Of(a._value / b._value);
        }



        public static NumberValue operator <<(NumberValue a, NumberValue b)
        {
            return NumberValue.Of((Int32)a.Int32Value << (Int32)b.Int32Value);
        }

        public static NumberValue operator >>(NumberValue a, NumberValue b)
        {
            return NumberValue.Of((Int32)a.Int32Value >> (Int32)b._value);
        }


        public static NumberValue operator >>>(NumberValue a, NumberValue b)
        {
            return NumberValue.Of((Int32)a.Int32Value >>> (Int32)b._value);
        }

        public static NumberValue operator ~(NumberValue a)
        {
            return NumberValue.Of(~(Int64)a._value);
        }


        public static NumberValue operator &(NumberValue a, NumberValue b)
        {
            var c = unchecked((Int32)a.Int64Value);
            var d = unchecked((Int32)b.Int64Value);
            return NumberValue.Of(c & d);
        }

        public static NumberValue operator |(NumberValue a, NumberValue b)
        {
            var c = unchecked((Int32)a.Int64Value);
            var d = unchecked((Int32)b.Int64Value);
            return NumberValue.Of(c | d);
        }

        public static NumberValue operator ^(NumberValue a, NumberValue b)
        {
            var c = unchecked((Int32)a.Int64Value);
            var d = unchecked((Int32)b.Int64Value);
            return NumberValue.Of(c ^ d);
        }


        public override int GetHashCode()
        {
            return (Int32)_value;
        }


        public override bool Equals(object obj)
        {
            if (obj is NumberValue num)
            {
                return num._value == _value;
            }
            else if (obj is BooleanValue bol)
            {
                return _value == bol.IntValue;
            }
            else if (obj is StringValue str && Double.TryParse(str.Value, out var dVal))
            {
                return _value == dVal;
            }
            return false;
        }

        /// <summary>
        /// TODO 数值计算脚本性能较差
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NumberValue Of(Double value)
        {
            if (Double.IsNaN(value))
            {
                return NaN;
            }
            if (value == 0d)
            {
                return Zero;
            }
            return new NumberValue(value);
        }


    }
}