using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{

    public partial class NumberValue : ScriptValue
    {
        public static readonly NumberValue Negative1 = new NumberValue(-1);
        public static readonly NumberValue NaN = new NumberValue(Double.NaN);
        public static readonly NumberValue Zero = new NumberValue(0);
        public static readonly NumberValue Num1 = new NumberValue(1);
        public static readonly NumberValue Num2 = new NumberValue(2);
        public static readonly NumberValue Num3 = new NumberValue(3);
        public static readonly NumberValue Num4 = new NumberValue(4);
        public static readonly NumberValue Num5 = new NumberValue(5);
        public static readonly NumberValue Num6 = new NumberValue(6);
        public static readonly NumberValue Num7 = new NumberValue(7);
        public static readonly NumberValue Num8 = new NumberValue(8);
        public static readonly NumberValue Num9 = new NumberValue(9);





        private readonly Double _value;

        public NumberValue(Double str = 0) : base()
        {
            _value = str;
            _prototype = NumberValue.Prototype;
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
        public static NumberValue Of(Double value)
        {
            return new NumberValue(value);
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
            return NumberValue.Of((Int32)a.Int32Value << (Byte)b.Int32Value);
        }

        public static NumberValue operator >>(NumberValue a, NumberValue b)
        {
            return NumberValue.Of((Int32)a.Int32Value >> (Byte)b._value);
        }


        public static NumberValue operator >>>(NumberValue a, NumberValue b)
        {
            return NumberValue.Of((Int32)a.Int32Value >>> (Byte)b._value);
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





    }
}