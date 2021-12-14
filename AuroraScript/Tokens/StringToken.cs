using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Tokens
{
    internal class StringToken: ValueToken
    {
        public StringToken()
        {
            this.Type = ValueType.String;
        }
    }
}
