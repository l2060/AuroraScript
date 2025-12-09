using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 执行上下文类，用于管理脚本执行过程中的状态和环境
    /// </summary>
    public sealed class ExecuteContext : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        internal readonly ScriptDatumStack _operandStack;
        internal readonly Stack<CallFrame> _callStack;
        private RuntimeVM _virtualMachine;

        private ExecuteStatus _status = ExecuteStatus.Idle;
        private ScriptObject _result = ScriptObject.Null;
        private AuroraRuntimeException _error;
        private ScriptObject _userState;

        /// <summary>
        /// 当前执行位置模块
        /// </summary>
        public ScriptModule CurrentModule
        {
            get
            {
                return _callStack.Count > 0 ? _callStack.Peek().Module : null;
            }
        }
        public ScriptDomain Domain { get; private set; }
        public ExecuteOptions ExecuteOptions { get; private set; } = ExecuteOptions.Default;

        private long _accumulatedTicks;
        private long _startTicks;

        private Boolean _pooled;
        private Boolean _released;

        internal ExecuteContext()
        {
            _operandStack = new ScriptDatumStack();
            _callStack = new();
            _stopwatch = new Stopwatch();
            _released = true;
        }



        internal void Lease(ScriptDomain domain, RuntimeVM virtualMachine, ExecuteOptions executeOptions)
        {
            _pooled = true;
            _released = false;
            GC.ReRegisterForFinalize(this);
            InitializeCore(domain, virtualMachine, executeOptions);
        }

        private void InitializeCore(ScriptDomain domain, RuntimeVM virtualMachine, ExecuteOptions executeOptions)
        {
            Domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _virtualMachine = virtualMachine ?? throw new ArgumentNullException(nameof(virtualMachine));
            ExecuteOptions = executeOptions ?? ExecuteOptions.Default;
            _userState = ExecuteOptions.UserState;
            _status = ExecuteStatus.Idle;
            _result = ScriptObject.Null;
            _error = null;
            while (_callStack.Count > 0)
            {
                CallFramePool.Return(_callStack.Pop());
            }
            _operandStack.Clear();
            _accumulatedTicks = 0;
            _startTicks = 0;
            _stopwatch.Restart();
        }

        /// <summary>
        /// 在CLR中继续中断脚本的执行。
        /// </summary>
        /// <returns></returns>
        public ExecuteContext Continue()
        {
            if (_status == ExecuteStatus.Interrupted || _status == ExecuteStatus.Error)
            {
                if (_status == ExecuteStatus.Error)
                {
                    var currentCallStack = _callStack.Peek();
                    currentCallStack.Pointer = currentCallStack.LastInstructionPointer;
                    var patchVM = _virtualMachine.PatchVM();
                    patchVM.Patch(this);
                }
                _virtualMachine.Execute(this);
            }
            return this;
        }

        [DebuggerHidden]
        public ExecuteContext Done(AbnormalStrategy strategy = AbnormalStrategy.Interruption)
        {
            if (_status == ExecuteStatus.Running)
            {
                throw new AuroraException("Current context is running and cannot be repeated before it is completed");
            }
            if (_status == ExecuteStatus.Complete) return this;

            while (_status != ExecuteStatus.Complete)
            {
                if (_status == ExecuteStatus.Error && strategy == AbnormalStrategy.Interruption)
                {
                    break;
                }
                Continue();
            }
            return this;
        }


        /// <summary>
        /// 中断脚本执行
        /// </summary>
        public void Interrupt()
        {
            SetStatus(ExecuteStatus.Interrupted, ScriptObject.Null, null);
        }


        internal void SetStatus(ExecuteStatus status, ScriptObject result, Exception exception)
        {
            _result = result;
            _status = status;
            if (status == ExecuteStatus.Running)
            {
                _startTicks = _stopwatch.ElapsedTicks;
            }
            else
            {
                var tick = _stopwatch.ElapsedTicks - _startTicks;
                if (tick > 0)
                {
                    _accumulatedTicks += tick;
                }
            }

            if (exception != null)
            {
                var stackTrace = CaptureStackTrace();
                _error = new AuroraRuntimeException(exception, stackTrace);
            }
        }

        private String CaptureStackTrace()
        {
            var current = _callStack.Peek();
            StringBuilder sb = new StringBuilder();

            var moduleSymbols = _virtualMachine.ResolveModule(current.LastInstructionPointer);
            String funcName = "";
            var symbol = moduleSymbols.Resolve(current.LastInstructionPointer);
            var funcSymbol = symbol.ResolveParent<FunctionSymbol>();
            if (funcSymbol != null)
            {
                funcName = funcSymbol.Name;
            }
            sb.AppendLine($" at {moduleSymbols.FilePath} {funcName}() line:{symbol.LineNumber} [{current.LastInstructionPointer}]");
            Boolean isFirst = true;
            foreach (var frame in _callStack)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }
                var pointer = frame.Pointer - 2;
                moduleSymbols = _virtualMachine.ResolveModule(pointer);
                if (moduleSymbols != null)
                {
                    symbol = moduleSymbols.Resolve(pointer);
                    funcSymbol = symbol.ResolveParent<FunctionSymbol>();
                    sb.AppendLine($" at {moduleSymbols.FilePath} {funcSymbol.Name}() line:{symbol.LineNumber} [{pointer}]");
                }
            }
            var nativeCallPointer = _callStack.Last().EntryPointer;
            moduleSymbols = _virtualMachine.ResolveModule(nativeCallPointer);
            if (moduleSymbols != null)
            {
                symbol = moduleSymbols.Resolve(nativeCallPointer);
                funcSymbol = symbol.ResolveParent<FunctionSymbol>();

                if (funcSymbol != null)
                {
                    sb.Append($" at {moduleSymbols.FilePath} {funcSymbol.Name}() line:{funcSymbol.LineNumber} [{nativeCallPointer}]");
                }
                else
                {
                    sb.Append($" at clr");
                }



            }
            return sb.ToString();
        }

        public double UsedTime
        {
            get
            {
                var totalTicks = _accumulatedTicks;
                if (_status == ExecuteStatus.Running)
                {
                    totalTicks += _stopwatch.ElapsedTicks - _startTicks;
                }
                if (totalTicks <= 0)
                {
                    return 0d;
                }
                return totalTicks * 1000.0 / Stopwatch.Frequency;
            }
        }

        public ExecuteStatus Status => _status;
        public ScriptObject Result => _result;
        public AuroraRuntimeException Error => _error;

        public ScriptObject UserState => _userState;

        public void Reset()
        {
            while (_callStack.Count > 0)
            {
                CallFramePool.Return(_callStack.Pop());
            }
            _operandStack.Clear();
            _status = ExecuteStatus.Idle;
            _result = ScriptObject.Null;
            _error = null;
            _accumulatedTicks = 0;
            _startTicks = 0;
        }

        internal void ResetForPool()
        {
            Reset();
            _virtualMachine = null;
            Domain = null;
            ExecuteOptions = ExecuteOptions.Default;
            _stopwatch.Reset();
            _pooled = true;
            _released = true;
        }

        public void Dispose()
        {
            if (_pooled)
            {
                if (!_released)
                {
                    _released = true;
                    ExecuteContextPool.Return(this);
                }
                GC.SuppressFinalize(this);
            }
            else
            {
                Reset();
            }
        }

        ~ExecuteContext()
        {
            if (_pooled && !_released)
            {
                _released = true;
                ExecuteContextPool.Return(this);
            }
        }
    }
}

