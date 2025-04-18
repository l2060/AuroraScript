using System;

namespace AuroraScript.Exceptions
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Exception ex, String message) : base(message, ex)
        {

        }

        public RuntimeException(String message) : base(message)
        {

        }

    }
}
