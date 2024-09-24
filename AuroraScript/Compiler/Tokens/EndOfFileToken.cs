using AuroraScript.Common;
using AuroraScript.Compiler;

namespace AuroraScript.Tokens
{
    /// <summary>
    /// end of file
    /// </summary>
    public class EndOfFileToken : Token
    {
        internal EndOfFileToken()
        {
            this.Symbol = Symbols.KW_EOF;
        }
    }
}
