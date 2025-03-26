using AuroraScript.Compiler;
using System;

namespace AuroraScript.Tokens
{
    public class NumberToken : ValueToken
    {
        internal NumberToken(String value)
        {
            this.Type = ValueType.Number;

            if (value.StartsWith("0x"))
            {
                this.NumberValue = Convert.ToInt64(value, 16);
            }
            else
            {
                this.NumberValue = Double.Parse(value.Replace("_", "")); ;
            }
        }

        public Double NumberValue { get; private set; }
    }

}