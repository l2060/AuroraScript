using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Exceptions
{
    public class CompilerException : Exception
    {
        public string fileName { get; private set; }


        internal CompilerException(string fileName, string message) : base(message)
        {
            this.fileName = fileName;
        }

    }
}
