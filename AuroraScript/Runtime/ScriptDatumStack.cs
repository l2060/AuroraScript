using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    internal sealed class ScriptDatumStack
    {
        private ScriptDatum[] _buffer;
        private int _size;

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
            var size = _size;
            if (size == buffer.Length)
            {
                Array.Resize(ref _buffer, buffer.Length * 2);
                buffer = _buffer;
            }
            buffer[size] = datum;
            _size = size + 1;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushRef( ref ScriptDatum datum)
        {
            var buffer = _buffer;
            var size = _size;
            if (size == buffer.Length)
            {
                Array.Resize(ref _buffer, buffer.Length * 2);
                buffer = _buffer;
            }
            buffer[size] = datum;
            _size = size + 1;
        }


        public void PushObject(ScriptObject value)
        {
            PushDatum(ScriptDatum.FromObject(value));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReplaceTop(ScriptDatum value)
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            _buffer[_size - 1] = value;
        }

        public void ReplaceTop(ScriptObject value)
        {
            ReplaceTop(ScriptDatum.FromObject(value));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum PopDatum()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            var buffer = _buffer;
            var index = _size - 1;
            var value = buffer[index];
            buffer[index] = default;
            _size = index;
            return value;
        }

        public ScriptObject PopObject()
        {
            return PopDatum().ToObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum PeekDatum()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            return _buffer[_size - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ScriptDatum PeekRef(int offset = 0)
        {
            var index = _size - 1 - offset;
            if (index < 0)
            {
                throw new InvalidOperationException("Stack does not have enough elements.");
            }
            return ref _buffer[index];
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
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            var index = --_size;
            _buffer[index] = default;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _size = 0;
        }
    }
}

