using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 执行上下文类，用于管理脚本执行过程中的状态和环境
    /// 包含操作数栈、调用栈、全局对象和执行状态等信息
    /// </summary>
    public class ExecuteContext
    {
        private Stopwatch _stopwatch = Stopwatch.StartNew();


        /// <summary>
        /// 操作数栈，用于存储执行过程中的临时值
        /// </summary>
        internal readonly Stack<ScriptObject> _operandStack;

        /// <summary>
        /// 调用栈，用于管理函数调用
        /// </summary>
        internal readonly Stack<CallFrame> _callStack;

        /// <summary>
        /// 虚拟机实例，用于执行字节码
        /// </summary>
        private readonly RuntimeVM _virtualMachine;

        /// <summary>
        /// 当前执行状态，初始为空闲状态
        /// </summary>
        private ExecuteStatus _status = ExecuteStatus.Idle;

        /// <summary>
        /// 执行结果，存储脚本执行完成后的返回值
        /// </summary>
        private ScriptObject _result;

        /// <summary>
        /// 执行错误，存储脚本执行过程中发生的异常
        /// </summary>
        private Exception _error;

        /// <summary>
        /// 全局对象，存储脚本的全局变量和函数
        /// </summary>
        public readonly ScriptGlobal Global;

        /// <summary>
        /// 执行选项，如最大调用栈深度等
        /// </summary>
        public readonly ExecuteOptions ExecuteOptions;


        private Int64 _accumulatedTick = 0;
        private Int64 _startTick;




        /// <summary>
        /// 创建新的执行上下文
        /// </summary>
        /// <param name="global">全局对象，用于存储全局变量和函数</param>
        /// <param name="virtualMachine">虚拟机实例，用于执行字节码</param>
        /// <param name="executeOptions">执行选项，如最大调用栈深度等</param>
        internal ExecuteContext(ScriptGlobal global, RuntimeVM virtualMachine, ExecuteOptions executeOptions)
        {
            _accumulatedTick = 0;
            // 执行选项
            ExecuteOptions = executeOptions;
            // 初始化虚拟机引用
            _virtualMachine = virtualMachine;
            // 初始化操作数栈，用于存储执行过程中的临时值
            _operandStack = new Stack<ScriptObject>();
            // 初始化调用栈，用于管理函数调用
            _callStack = new Stack<CallFrame>();
            // 设置全局对象
            Global = global;
        }


        /// <summary>
        /// 创建新的执行上下文
        /// </summary>
        /// <param name="global">全局对象，用于存储全局变量和函数</param>
        /// <param name="virtualMachine">虚拟机实例，用于执行字节码</param>
        internal ExecuteContext(ScriptGlobal global, RuntimeVM virtualMachine) : this(global, virtualMachine, ExecuteOptions.Default)
        {
        }


        /// <summary>
        /// 继续执行当前中断或出错的脚本
        /// </summary>
        /// <returns>当前执行上下文，支持链式调用</returns>
        public ExecuteContext Continue()
        {
            // 如果当前状态是中断或错误，则继续执行
            if (_status == ExecuteStatus.Interrupted || _status == ExecuteStatus.Error)
            {
                // 调用虚拟机继续执行
                _virtualMachine.Execute(this);
            }
            return this;
        }


        /// <summary>
        /// 执行到完成状态，直到脚本全部执行完毕或出现错误
        /// </summary>
        /// <returns>当前执行上下文，支持链式调用</returns>
        public ExecuteContext Done(AbnormalStrategy strategy = AbnormalStrategy.Interruption)
        {
            if (_status == ExecuteStatus.Running) throw new RuntimeException("Current context is running and cannot be repeated before it is completed");
            if (_status == ExecuteStatus.Complete) return this;
            // 循环继续执行，直到状态变为完成
            while (_status != ExecuteStatus.Complete)
            {
                if (_status == ExecuteStatus.Error && strategy == AbnormalStrategy.Interruption) break;
                // 调用Continue方法继续执行
                Continue();
            }
            return this;
        }



        /// <summary>
        /// 设置执行状态、结果和异常信息
        /// </summary>
        /// <param name="status">新的执行状态</param>
        /// <param name="result">执行结果对象</param>
        /// <param name="exception">执行过程中的异常</param>
        internal void SetStatus(ExecuteStatus status, ScriptObject result, Exception exception)
        {
            // 设置执行结果
            _result = result;
            // 设置异常信息
            _error = exception;
            // 设置执行状态
            _status = status;
            if (status == ExecuteStatus.Running)
            {
                // 开始时间戳
                _startTick = _stopwatch.ElapsedMilliseconds;
            }
            else
            {
                var tick = _stopwatch.ElapsedMilliseconds - _startTick;
                _accumulatedTick += tick;
            }
        }

        /// <summary>
        /// 获取当前调用用时 (Milliseconds)
        /// </summary>
        public Int64 UsedTime
        {
            get
            {
                var total = _accumulatedTick;
                if (_status == ExecuteStatus.Running)
                {
                    total += _stopwatch.ElapsedMilliseconds - _startTick;
                }
                return total;
            }
        }

        /// <summary>
        /// 执行状态
        /// </summary>
        public ExecuteStatus Status => _status;

        /// <summary>
        /// 获取执行返回结果
        /// </summary>
        public ScriptObject Result => _result;

        /// <summary>
        /// 获取执行代码过程产生的异常
        /// </summary>
        public Exception Error => _error;


        /// <summary>
        /// 重置执行上下文，清空操作数栈和调用栈
        /// </summary>
        public void Reset()
        {
            // 清空操作数栈
            _operandStack.Clear();
            // 清空调用栈
            _callStack.Clear();
        }
    }
}
