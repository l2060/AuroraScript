using System;

namespace AuroraScript.Tokens
{
    public class BooleanToken : ValueToken
    {
        internal BooleanToken(String value)
        {
            this.Type = ValueType.Boolean;
            this.BoolValue = Boolean.Parse(value);
        }



        public Boolean BoolValue { get; private set; }

        public override string ToString()
        {
            return BoolValue.ToString();
        }


    }
}