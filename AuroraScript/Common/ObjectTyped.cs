using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Common
{
    public class ObjectTyped
    {
        internal ObjectTyped(Token identifier)
        {
            Identifier = identifier;    
        }



        /// <summary>
        /// function name
        /// </summary>
        public Token Identifier { get; set; }

    }
}
