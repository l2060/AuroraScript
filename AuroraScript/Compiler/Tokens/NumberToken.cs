namespace AuroraScript.Tokens
{
    public class NumberToken : ValueToken
    {
        public readonly static NumberToken Zero = new NumberToken("0");
        public readonly static NumberToken One = new NumberToken("1");



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

        public override string ToString()
        {
            return NumberValue.ToString();
        }
        public Double NumberValue { get; private set; }
    }

}