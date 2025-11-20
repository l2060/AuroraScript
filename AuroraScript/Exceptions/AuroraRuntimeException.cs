using System;

namespace AuroraScript.Exceptions
{
    /// <summary>
    /// AuroraScript运行时异常类,表示脚本在VM执行过程中发生的错误
    /// </summary>
    public class AuroraRuntimeException : Exception
    {
        public readonly String ModuleName;

        public readonly Int32 LineNumber;
        public new String StackTrace { get; set; }

        public AuroraRuntimeException(Exception ex, String stackTrace) : base(ex.Message, ex)
        {
            StackTrace = stackTrace;
        }

        public AuroraRuntimeException(String message) : base(message)
        {

        }


        public override string ToString()
        {
            return Message + Environment.NewLine + StackTrace;
        }
    }
}
