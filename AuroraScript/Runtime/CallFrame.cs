using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// </summary>
    public class CallFrame : IDisposable
    {
        private const Int32 DefaultLocalCapacity = 64;

        public readonly Int32 EntryPointer;
        public Int32 LastInstructionPointer;
        public Int32 Pointer;

        private ScriptDatum[] _locals;
        public readonly ScriptDatum[] Arguments;
        public readonly ScriptModule Module;
        public readonly ScriptGlobal Global;
        public readonly CallFrame Environment;

        public CallFrame(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, params ScriptObject[] arguments)
            : this(environment, global, thisModule, entryPointer, ConvertArguments(arguments))
        {
        }

        internal CallFrame(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, ScriptDatum[] argumentDatums)
        {
            Global = global;
            Environment = environment;
            EntryPointer = Pointer = entryPointer;
            Module = thisModule;
            Arguments = argumentDatums ?? Array.Empty<ScriptDatum>();
            var requiredCapacity = Math.Max(DefaultLocalCapacity, argumentDatums?.Length ?? 0);
            var initialCapacity = CalculateInitialCapacity(requiredCapacity);
            _locals = ArrayPool<ScriptDatum>.Shared.Rent(initialCapacity);
            Array.Clear(_locals, 0, _locals.Length);
        }

        private static ScriptDatum[] ConvertArguments(ScriptObject[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return Array.Empty<ScriptDatum>();
            }
            var result = new ScriptDatum[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = ScriptDatum.FromObject(arguments[i]);
            }
            return result;
        }

        public Boolean TryGetArgument(Int32 index, out ScriptObject arg)
        {
            arg = ScriptObject.Null;
            if (index >= Arguments.Length) return false;
            arg = Arguments[index].ToObject();
            return true;
        }

        public ScriptObject GetArgument(Int32 index)
        {
            if (index >= Arguments.Length) return ScriptObject.Null;
            return Arguments[index].ToObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetArgumentDatum(Int32 index, out ScriptDatum datum)
        {
            if (index >= Arguments.Length)
            {
                datum = ScriptDatum.FromNull();
                return false;
            }
            datum = Arguments[index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum GetArgumentDatum(Int32 index)
        {
            if (index >= Arguments.Length) return ScriptDatum.FromNull();
            return Arguments[index];
        }

        public ScriptDatum[] Locals => _locals ?? Array.Empty<ScriptDatum>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum GetLocalDatum(Int32 index)
        {
            if (_locals == null || index >= _locals.Length || index < 0)
            {
                return ScriptDatum.FromNull();
            }
            return _locals[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLocalDatum(Int32 index, ScriptDatum datum)
        {
            if (index < 0)
            {
                return;
            }
            EnsureLocalCapacity(index + 1);
            _locals[index] = datum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureLocalStorage(Int32 minLength)
        {
            if (minLength <= 0)
            {
                return;
            }
            EnsureLocalCapacity(minLength);
        }

        public void Dispose()
        {
            if (_locals != null)
            {
                ArrayPool<ScriptDatum>.Shared.Return(_locals, clearArray: true);
                _locals = null;
            }
        }

        private static Int32 CalculateInitialCapacity(Int32 required)
        {
            var capacity = DefaultLocalCapacity;
            while (capacity < required)
            {
                capacity <<= 1;
            }
            return capacity;
        }

        private void EnsureLocalCapacity(Int32 minLength)
        {
            if (_locals != null && _locals.Length >= minLength)
            {
                return;
            }

            var current = _locals ?? Array.Empty<ScriptDatum>();
            var newCapacity = current.Length == 0 ? DefaultLocalCapacity : current.Length;
            while (newCapacity < minLength)
            {
                newCapacity <<= 1;
            }

            var newBuffer = ArrayPool<ScriptDatum>.Shared.Rent(newCapacity);
            if (current.Length > 0)
            {
                Array.Copy(current, newBuffer, current.Length);
            }
            Array.Clear(newBuffer, current.Length, newCapacity - current.Length);

            if (_locals != null)
            {
                ArrayPool<ScriptDatum>.Shared.Return(_locals, clearArray: true);
            }
            _locals = newBuffer;
        }
    }
}

