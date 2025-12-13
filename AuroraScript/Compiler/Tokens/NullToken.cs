namespace AuroraScript.Tokens
{
    /// <summary>
    /// null value token
    /// </summary>
    internal class NullToken : ValueToken
    {
        internal NullToken()
        {
            this.Type = ValueType.Null;
        }
        public override string ToString()
        {
            return "null";
        }
    }
}