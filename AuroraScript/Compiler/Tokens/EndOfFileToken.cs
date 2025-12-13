using AuroraScript.Compiler;

namespace AuroraScript.Tokens
{
    /// <summary>
    /// end of file
    /// </summary>
    internal class EndOfFileToken : Token
    {
        internal EndOfFileToken()
        {
            this.Symbol = Symbols.KW_EOF;
        }
    }
}