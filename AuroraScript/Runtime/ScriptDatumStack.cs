using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Util;
using System;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuroraScript.Runtime
{
    internal sealed class ScriptDatumStack
    {
        internal ScriptDatum[] _buffer;
        internal int _size;

        public ScriptDatumStack(int capacity = 128)
        {
            _buffer = new ScriptDatum[Math.Max(4, capacity)];
            _size = 0;
        }

        public int Count => _size;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushDatum(ScriptDatum datum)
        {
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            buffer[i] = datum;
            _size = i + 1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushRef(ref ScriptDatum datum)
        {
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            buffer[i] = datum;
            _size = i + 1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushNumber(Double value)
        {
            ScriptDatum.NumberOf(value, out var result);
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            buffer[i] = result;
            _size = i + 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushNull()
        {
            var result = ScriptDatum.Null;
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            buffer[i] = result;
            _size = i + 1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushTrue()
        {
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            ScriptDatum.BooleanOf(true, out buffer[i]);
            _size = i + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushFalse()
        {
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            ScriptDatum.BooleanOf(false, out buffer[i]);
            _size = i + 1;
        }



        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void Grow()
        {
            Array.Resize(ref _buffer, _buffer.Length * 2);
        }


        public void PushObject(ScriptObject value)
        {
            PushDatum(ScriptDatum.FromObject(value));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum PopDatum()
        {
            int idx = _size - 1;
            if ((uint)idx >= (uint)_buffer.Length) ThrowHelper.ThrowEmptyStack();
            _size = idx;
            return _buffer[idx];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly ScriptDatum PopRef()
        {
            int idx = _size - 1;
            if ((uint)idx >= (uint)_buffer.Length) ThrowHelper.ThrowEmptyStack();
            _size = idx;
            return ref _buffer[idx];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopToRef(ref ScriptDatum datum)
        {
            int idx = _size - 1;
            if ((uint)idx >= (uint)_buffer.Length) ThrowHelper.ThrowEmptyStack();
            _size = idx;
            datum = _buffer[idx];
        }


        public ScriptObject PopObject()
        {
            ref readonly var datum = ref PopRef();
            return ScriptDatum.ToObject(in datum);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ScriptDatum PeekRef(int offset = 0)
        {
            var index = _size - 1 - offset;
            return ref _buffer[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PeekIsTrue(int offset = 0)
        {
            var index = _size - 1 - offset;
            return ScriptDatum.IsTrue(in _buffer[index]);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PopTopDatumIsTrue()
        {
            var index = _size-- - 1;
            return ScriptDatum.IsTrue(in _buffer[index]);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Duplicate()
        {
            var buffer = _buffer;
            int i = _size;
            if ((uint)i >= (uint)buffer.Length) Grow();
            buffer[i] = buffer[i - 1];
            _size = i + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap()
        {
            if (_size < 2) ThrowHelper.ThrowSwapUnderflow();
            var buffer = _buffer;
            int topIndex = _size - 1;
            var temp = buffer[topIndex];
            buffer[topIndex] = buffer[topIndex - 1];
            buffer[topIndex - 1] = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopDiscard()
        {
            if (_size == 0) ThrowHelper.ThrowEmptyStack();
            var index = --_size;
        }


        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _size = 0;
        }
    }
}

