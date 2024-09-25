using AuroraScript.Compiler;

namespace AuroraScript.Tokens
{
    public enum ValueType
    {
        String,
        Number,
        Boolean,
        Null,
    }

    /// <summary>
    /// value string boolean number null
    /// </summary>
    public class ValueToken : Token
    {
        internal ValueToken()
        {
        }

        public ValueType Type { get; protected set; }

        public virtual string ToValue()
        {
            return Value;
        }
    }
}