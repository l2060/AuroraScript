namespace AuroraScript.Tokens
{
    public class StringToken : ValueToken
    {
        public Boolean Blocked;
        internal StringToken(Boolean blocked)
        {
            this.Blocked = blocked;
            this.Type = ValueType.String;
        }

        public override string ToValue()
        {

            if (this.Blocked)
            {
                var lines = this.Value.Split(Environment.NewLine).Select(e => "|> " + e);
                return Environment.NewLine + string.Join(Environment.NewLine, lines) + Environment.NewLine;
            }
            return $"'{this.Value.Replace("\r", "\\r").Replace("\n", "\\n")}'";
        }
    }
}