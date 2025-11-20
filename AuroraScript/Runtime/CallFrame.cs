using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    public sealed class CallFrame : IDisposable
    {
        private const Int32 DefaultLocalCapacity = 8;

        public Int32 EntryPointer { get; private set; }
        public Int32 LastInstructionPointer;
        public Int32 Pointer;

        private ScriptDatum[] _locals;
        private Int32 _localsUsed;
        private Dictionary<Int32, Upvalue> _openUpvalues;
        private ClosureUpvalue[] _capturedUpvalues = Array.Empty<ClosureUpvalue>();

        public ScriptDatum[] Arguments { get; private set; } = Array.Empty<ScriptDatum>();
        public ScriptModule Module { get; private set; }
        public ScriptDomain Domain { get; private set; }

        internal CallFrame()
        {
        }

        internal void Initialize(ScriptDomain domain, ScriptModule module, Int32 entryPointer, ScriptDatum[] argumentDatums, ClosureUpvalue[] captured)
        {
            Domain = domain;
            Module = module;
            EntryPointer = entryPointer;
            Pointer = entryPointer;
            LastInstructionPointer = entryPointer;
            Arguments = argumentDatums ?? Array.Empty<ScriptDatum>();
            _capturedUpvalues = captured ?? Array.Empty<ClosureUpvalue>();

            var requiredLocals = Math.Max(DefaultLocalCapacity, Arguments.Length);
            var previouslyUsed = _localsUsed;
            EnsureLocalCapacity(requiredLocals);
            if (_locals != null)
            {
                var clearLength = Math.Min(_locals.Length, Math.Max(previouslyUsed, requiredLocals));
                if (clearLength > 0)
                {
                    Array.Clear(_locals, 0, clearLength);
                }
            }
            _localsUsed = 0;
            _openUpvalues?.Clear();
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

        internal Upvalue GetCapturedUpvalue(Int32 slot)
        {
            if (_capturedUpvalues == null || _capturedUpvalues.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < _capturedUpvalues.Length; i++)
            {
                if (_capturedUpvalues[i].Slot == slot)
                {
                    return _capturedUpvalues[i].Upvalue;
                }
            }
            return null;
        }

        internal Upvalue GetOrCreateUpvalue(Int32 slot)
        {
            _openUpvalues ??= new Dictionary<Int32, Upvalue>();
            if (_openUpvalues.TryGetValue(slot, out var existing))
            {
                return existing;
            }

            var datum = GetLocalDatum(slot);
            if (datum.Kind == ValueKind.Object && datum.Object is Upvalue inherited)
            {
                return inherited;
            }

            var upvalue = new Upvalue(this, slot);
            _openUpvalues[slot] = upvalue;
            return upvalue;
        }

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
            if (index + 1 > _localsUsed)
            {
                _localsUsed = index + 1;
            }
        }

        public void Dispose()
        {
            ResetFull();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureLocalStorage(Int32 length)
        {
            if (length <= 0)
            {
                return;
            }
            EnsureLocalCapacity(length);
            if (length > _localsUsed)
            {
                _localsUsed = length;
            }
        }

        internal void ResetForPool()
        {
            ResetCore(releaseLocals: false);
        }

        internal void ResetFull()
        {
            ResetCore(releaseLocals: true);
        }

        private void EnsureLocalCapacity(Int32 length)
        {
            if (_locals != null && _locals.Length >= length)
            {
                return;
            }

            var current = _locals ?? Array.Empty<ScriptDatum>();
            var newCapacity = current.Length == 0 ? DefaultLocalCapacity : current.Length;
            while (newCapacity < length)
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

        private void CloseOpenUpvalues()
        {
            if (_openUpvalues == null || _openUpvalues.Count == 0)
            {
                return;
            }
            foreach (var upvalue in _openUpvalues.Values)
            {
                upvalue.Close();
            }
            _openUpvalues.Clear();
        }

        private void ResetCore(Boolean releaseLocals)
        {
            CloseOpenUpvalues();
            if (_locals != null)
            {
                if (releaseLocals)
                {
                    ArrayPool<ScriptDatum>.Shared.Return(_locals, clearArray: true);
                    _locals = null;
                }
                else if (_localsUsed > 0)
                {
                    Array.Clear(_locals, 0, Math.Min(_localsUsed, _locals.Length));
                }
            }
            _localsUsed = 0;
            _openUpvalues?.Clear();
            Arguments = Array.Empty<ScriptDatum>();
            _capturedUpvalues = Array.Empty<ClosureUpvalue>();
            Module = null;
            Domain = null;
            EntryPointer = 0;
            Pointer = 0;
            LastInstructionPointer = 0;
        }
    }
}

