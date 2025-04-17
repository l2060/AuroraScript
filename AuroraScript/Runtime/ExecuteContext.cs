using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime
{
    public class ExecuteContext
    {
        /// <summary>
        /// 操作数栈，用于存储执行过程中的临时值
        /// </summary>
        internal readonly Stack<ScriptObject> _operandStack;

        /// <summary>
        /// 调用栈，用于管理函数调用
        /// </summary>
        internal readonly Stack<CallFrame> _callStack;

        private ExecuteStatus _status = ExecuteStatus.Idle;
        private ScriptObject _result;
        private Exception _error;

        public readonly ScriptGlobal Global;

        private readonly RuntimeVM _virtualMachine;

        internal ExecuteContext(ScriptGlobal global, RuntimeVM virtualMachine)
        {
            _virtualMachine = virtualMachine;
            _operandStack = new Stack<ScriptObject>();
            _callStack = new Stack<CallFrame>();
            Global = global;
        }



        public ExecuteContext Continue()
        {
            if (_status == ExecuteStatus.Interrupted || _status == ExecuteStatus.Error)
            {
                _virtualMachine.Execute(this);
            }
            return this;
        }


        public ExecuteContext Done()
        {
            while (_status != ExecuteStatus.Complete)
            {
                Continue();
            }
            return this;
        }



        internal void SetStatus(ExecuteStatus status, ScriptObject result, Exception exception)
        {
            _result = result;
            _error = exception;
            _status = status;
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


        public void Reset()
        {
            _operandStack.Clear();
            _callStack.Clear();
        }
    }
}
