using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Tokens
{
    internal class NullToken : ValueToken
    {
        public NullToken()
        {
            this.Type = ValueType.Null;
        }
    }
}
