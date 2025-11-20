using System;


namespace AuroraScript.Exceptions
{
    /// <summary>
    /// AuroraScript虚拟机异常类,表示脚本在VM执行过程中发生的错误
    /// </summary>
    public class AuroraVMException : Exception
    {
        public AuroraVMException(String message) : base(message)
        {

        }
    }
}
