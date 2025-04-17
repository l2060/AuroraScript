using System;

namespace AuroraScript.Compiler.Exceptions
{
    public class CompilerException : Exception
    {
        public string FileName { get; private set; }

        internal CompilerException(string fileName, string message) : base(message)
        {
            this.FileName = fileName;
        }
    }
}