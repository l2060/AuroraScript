using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Debugger;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 执行上下文类，用于管理脚本执行过程中的状态和环境
    /// </summary>
    public class ExecuteContext
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        internal readonly ScriptDatumStack _operandStack;
        internal readonly Stack<CallFrame> _callStack;
        private readonly RuntimeVM _virtualMachine;

        private ExecuteStatus _status = ExecuteStatus.Idle;
        private ScriptObject _result;
        private AuroraRuntimeException _error;

        public readonly ScriptGlobal Global;
        public readonly ExecuteOptions ExecuteOptions;

        private Int64 _accumulatedTick = 0;
        private Int64 _startTick;

        internal ExecuteContext(ScriptGlobal global, RuntimeVM virtualMachine, ExecuteOptions executeOptions)
        {
            ExecuteOptions = executeOptions;
            _virtualMachine = virtualMachine;
            Global = global;
            _operandStack = new ScriptDatumStack();
            _callStack = new Stack<CallFrame>();
        }

        internal ExecuteContext(ScriptGlobal global, RuntimeVM virtualMachine)
            : this(global, virtualMachine, ExecuteOptions.Default)
        {
        }

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

        internal void SetStatus(ExecuteStatus status, ScriptObject result, Exception exception)
        {
            _result = result;
            _status = status;
            if (status == ExecuteStatus.Running)
            {
                _startTick = _stopwatch.ElapsedMilliseconds;
            }
            else
            {
                var tick = _stopwatch.ElapsedMilliseconds - _startTick;
                _accumulatedTick += tick;
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
                symbol = moduleSymbols.Resolve(pointer);
                funcSymbol = symbol.ResolveParent<FunctionSymbol>();
                sb.AppendLine($" at {moduleSymbols.FilePath} {funcSymbol.Name}() line:{symbol.LineNumber} [{pointer}]");
            }

            var nativeCallPointer = _callStack.Last().EntryPointer;
            moduleSymbols = _virtualMachine.ResolveModule(nativeCallPointer);
            symbol = moduleSymbols.Resolve(nativeCallPointer);
            funcSymbol = symbol.ResolveParent<FunctionSymbol>();
            sb.Append($" at {moduleSymbols.FilePath} {funcSymbol.Name}() line:{funcSymbol.LineNumber} [{nativeCallPointer}]");
            return sb.ToString();
        }

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

        public ExecuteStatus Status => _status;
        public ScriptObject Result => _result;
        public AuroraRuntimeException Error => _error;

        public void Reset()
        {
            _operandStack.Clear();
            _callStack.Clear();
        }
    }
}

