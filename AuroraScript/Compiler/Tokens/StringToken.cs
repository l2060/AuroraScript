namespace AuroraScript.Tokens
{
    public class StringToken : ValueToken
    {
        internal StringToken()
        {
            this.Type = ValueType.String;
        }

        public override string ToValue()
        {
            return $"'{this.Value.Replace("\r", "\\r").Replace("\n", "\\n")}'";
        }
    }
}