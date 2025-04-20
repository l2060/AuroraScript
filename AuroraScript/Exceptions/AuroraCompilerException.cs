using System;

namespace AuroraScript.Compiler.Exceptions
{
    public class AuroraCompilerException : Exception
    {
        public string FileName { get; private set; }

        internal AuroraCompilerException(string fileName, string message) : base(message)
        {
            this.FileName = fileName;
        }
    }
}