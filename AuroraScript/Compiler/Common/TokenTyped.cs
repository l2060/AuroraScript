namespace AuroraScript.Common
{
    internal enum TokenTyped
    {
        Unknown,
        String,
        StringTemplate,

        Regex,

        StringBlock,

        Number,

        Identifier,

        Punctuator,

        Comment,

        NewLine,

        WhiteSpace
    }
}