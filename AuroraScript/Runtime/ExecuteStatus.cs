namespace AuroraScript.Runtime
{
    public enum ExecuteStatus
    {
        /// <summary>
        /// 空闲的，未开始的
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 完成的，结果返回
        /// </summary>
        Complete = 1,

        /// <summary>
        /// 正在运行的
        /// </summary>
        Running = 2,

        /// <summary>
        /// 错误的，可继续？
        /// </summary>
        Error = 3,

        /// <summary>
        /// 中断的，可通过Continue()方法继续执行
        /// </summary>
        Interrupted = 4
    }
}
