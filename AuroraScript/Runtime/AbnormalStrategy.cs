using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// Done异常策略
    /// </summary>
    public enum AbnormalStrategy
    {
        /// <summary>
        /// 出现异常时继续运行
        /// </summary>
        Continue = 0,


        /// <summary>
        /// 出现异常时中断
        /// </summary>
        Interruption = 1,


    }
}
