using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Exceptions
{
    public class CompilerException : Exception
    {
        public String fileName { get; private set; }


        internal CompilerException(String fileName, String message) : base(message)
        {
            this.fileName = fileName;
        }

    }
}
