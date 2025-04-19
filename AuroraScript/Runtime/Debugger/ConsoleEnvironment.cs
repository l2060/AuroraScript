using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Debugger
{
    internal class ConsoleEnvironment : ScriptObject
    {
        /// <summary>
        /// 用于计时功能的秒表对象
        /// </summary>
        private static Stopwatch _stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// 存储计时标记的字典，键为计时标记名称，值为开始时间
        /// </summary>
        private static Dictionary<String, Int64> _times = new();

        public ConsoleEnvironment()
        {
            // 在控制台对象中注册日志、计时和计时结束函数
            Define("log", new ClrFunction(LOG), false);
            Define("time", new ClrFunction(TIME), false);
            Define("timeEnd", new ClrFunction(TIMEEND), false);
        }



        /// <summary>
        /// 日志输出函数，在脚本中通过console.log或debug调用
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组</param>
        /// <returns>空对象</returns>
        public static ScriptObject LOG(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            Console.WriteLine(String.Join(", ", args));
            return ScriptObject.Null;
        }



        /// <summary>
        /// 开始计时函数，在脚本中通过console.time调用
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组，第一个参数为计时标记名称</param>
        /// <returns>空对象</returns>
        public static ScriptObject TIME(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1)
            {
                // 记录当前时间作为计时开始点
                _times[args[0].ToString()] = _stopwatch.ElapsedMilliseconds;
            }
            return ScriptObject.Null;
        }

        /// <summary>
        /// 结束计时函数，在脚本中通过console.timeEnd调用
        /// 计算并输出从开始计时到结束计时的时间间隔
        /// </summary>
        /// <param name="domain">脚本域</param>
        /// <param name="thisObject">调用对象（this）</param>
        /// <param name="args">参数数组，第一个参数为计时标记名称</param>
        /// <returns>空对象</returns>
        public static ScriptObject TIMEEND(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length == 1 && _times.TryGetValue(args[0].ToString(), out var value))
            {
                // 计算时间间隔
                var time = _stopwatch.ElapsedMilliseconds - value;
                // 移除计时标记
                _times.Remove(args[0].ToString());
                // 输出计时结果

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{args[0]} Used {time}ms");
                Console.ResetColor();

            }
            return ScriptObject.Null;
        }


    }
}
