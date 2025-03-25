using AuroraScript.Compiler;
using System;

namespace AuroraScript.Tokens
{
    public class NumberToken : ValueToken
    {
        internal NumberToken(String value)
        {
            if (value.StartsWith("0x"))
            {
                var int64 = Convert.ToInt64(value, 16);
                if (int64 <= Int32.MaxValue || int64 >= Int32.MinValue)
                {
                    this.Type = ValueType.IntegerNumber;
                    this.IntegerValue = (int)int64;
                }
                else
                {
                    this.Type = ValueType.DoubleNumber;
                    this.DoubleValue = int64;
                }
            }
            else
            {
                var _value = Double.Parse(value.Replace("_", "")); ;
                if (_value % 1 == 0 && _value <= Int32.MaxValue || _value >= Int32.MinValue)
                {
                    this.Type = ValueType.IntegerNumber;
                    this.IntegerValue = (int)_value;
                }
                else
                {
                    this.Type = ValueType.DoubleNumber;
                    this.DoubleValue = _value;
                }
            }

        }


        public int IntegerValue { get; private set; }
        public Double DoubleValue { get; private set; }








    }

}