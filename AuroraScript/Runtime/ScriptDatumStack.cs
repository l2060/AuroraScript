using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Util;
using System;
using System.Runtime.CompilerServices;

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
            if (_size == 0) ThrowHelper.ThrowEmptyStack();
            int idx = --_size;
            ref var elem = ref _buffer[idx];
            var value = elem;
            elem = default;
            return value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopToRef(ref ScriptDatum datum)
        {
            if (_size == 0) ThrowHelper.ThrowEmptyStack();
            int idx = --_size;
            ref var src = ref _buffer[idx];
            datum = src;
            src = default;
        }





        public ScriptObject PopObject()
        {
            return PopDatum().ToObject();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum PeekDatum()
        {
            if (_size == 0) ThrowHelper.ThrowEmptyStack();
            return _buffer[_size - 1];
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ScriptDatum PeekRef(int offset = 0)
        {
            var index = _size - 1 - offset;
            return ref _buffer[index];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool PopTopDatumIsTrue()
        {
            var index = _size-- - 1;
            return _buffer[index].IsTrue();
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Duplicate()
        {
            PushDatum(PeekDatum());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap()
        {
            if (_size < 2)
            {
                throw new InvalidOperationException("Stack has fewer than two elements.");
            }
            var buffer = _buffer;
            var topIndex = _size - 1;
            var belowIndex = topIndex - 1;
            ref var top = ref buffer[topIndex];
            var temp = top;
            top = buffer[belowIndex];
            buffer[belowIndex] = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopDiscard()
        {
            if (_size == 0) ThrowHelper.ThrowEmptyStack();
            var index = --_size;
            _buffer[index] = default;
        }


        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _size = 0;
        }
    }
}

