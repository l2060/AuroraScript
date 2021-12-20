

namespace AuroraScript.Tokens
{
    internal enum ValueType
    {
        String,
        Number,
        Boolean,
        Null,
    }

    /// <summary>
    /// value string boolean number null
    /// </summary>
    internal class ValueToken : Token
    {
        public ValueToken()
        {
        }

        public ValueType Type { get; protected set; }

    }
}
