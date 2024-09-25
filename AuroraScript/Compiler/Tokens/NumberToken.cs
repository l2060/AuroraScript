namespace AuroraScript.Tokens
{
    public class NumberToken : ValueToken
    {
        internal NumberToken()
        {
            this.Type = ValueType.Number;
        }
    }
}