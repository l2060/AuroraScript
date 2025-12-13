using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Base
{

    public sealed partial class NumberValue : ScriptValue
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Boolean IsTrue()
        {
            return _value != 0;
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
    }
}