using System;


namespace AuroraScript.Exceptions
{
    /// <summary>
    /// AuroraScript异常类,表示脚本引擎操作过程中发生的错误
    /// </summary>
    public class AuroraException : Exception
    {
        public AuroraException(String message) : base(message)
        {

        }
    }
}
