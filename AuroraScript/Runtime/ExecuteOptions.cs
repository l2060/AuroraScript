using System;

namespace AuroraScript.Runtime
{
    public class ExecuteOptions
    {
        /// <summary>
        /// 默认执行选项， 不会自动中断
        /// </summary>
        public static ExecuteOptions Default = new ExecuteOptions();

        /// <summary>
        /// 脚本内部最大调用栈深度
        /// </summary>
        public readonly Int32 MaxCallStackDepth = 1024;

        /// <summary>
        /// 自动中断指令数量 0表示不自动中断
        /// </summary>
        public readonly Int32 AutoInterruption = 0;

        /// <summary>
        /// 启用中断功能
        /// 为false时遇到yield指令不处理
        /// </summary>
        public readonly Boolean EnabledYield = true;

        /// <summary>
        /// 
        /// </summary>
        public readonly Object UserState = null;


        public ExecuteOptions(Int32 maxCallStackDepth = 1024, Object userState = null, Int32 autoInterruption = 0, bool yieldEnabled = false)
        {
            UserState = userState;
            MaxCallStackDepth = maxCallStackDepth;
            AutoInterruption = autoInterruption;
            EnabledYield = yieldEnabled;
        }



        public ExecuteOptions WithMaxCallStackDepth(Int32 value)
        {
            if (value == MaxCallStackDepth) return this;
            return new ExecuteOptions(value, UserState, AutoInterruption, EnabledYield);
        }

        public ExecuteOptions WithUserState(Object value)
        {
            if (ReferenceEquals(value, UserState)) return this;
            return new ExecuteOptions(MaxCallStackDepth, value, AutoInterruption, EnabledYield);
        }

        public ExecuteOptions WithAutoInterruption(Int32 value)
        {
            if (value == AutoInterruption) return this;
            return new ExecuteOptions(MaxCallStackDepth, UserState, value, EnabledYield);
        }

        public ExecuteOptions WithEnabledYield(Boolean value)
        {
            if (value == EnabledYield) return this;
            return new ExecuteOptions(MaxCallStackDepth, UserState, AutoInterruption, value);
        }

    }
}
