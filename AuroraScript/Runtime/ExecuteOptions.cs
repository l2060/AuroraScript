using System;

namespace AuroraScript.Runtime
{
    public class ExecuteOptions
    {
        /// <summary>
        /// 默认执行选项， 不会自动中断
        /// </summary>
        public static ExecuteOptions Default = new ExecuteOptions(256, 0, true);

        /// <summary>
        /// 脚本内部最大调用栈深度
        /// </summary>
        public readonly Int32 MaxCallStackDepth = 1000;

        /// <summary>
        /// 自动中断指令数量 0表示不自动中断
        /// </summary>
        public readonly Int32 AutoInterruption = 0;

        /// <summary>
        /// 自动中断指令数量 0表示不自动中断
        /// </summary>
        public readonly Boolean YieldEnabled = true;




        public ExecuteOptions(Int32 maxCallStackDepth, Int32 autoInterruption, bool yieldEnabled)
        {
            MaxCallStackDepth = maxCallStackDepth;
            AutoInterruption = autoInterruption;
            YieldEnabled = yieldEnabled;
        }





    }
}
