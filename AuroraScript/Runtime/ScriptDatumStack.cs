using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

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

        public void PushDatum(ScriptDatum datum)
        {
            if (_size == _buffer.Length)
            {
                Array.Resize(ref _buffer, _buffer.Length * 2);
            }
            _buffer[_size++] = datum;
        }

        public void Push(ScriptObject value)
        {
            PushDatum(ScriptDatum.FromObject(value));
        }



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



        public ScriptDatum PopDatum()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            var index = --_size;
            var value = _buffer[index];
            _buffer[index] = default;
            return value;
        }

        public ScriptObject Pop()
        {
            return PopDatum().ToObject();
        }

        public ScriptDatum PeekDatum()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            return _buffer[_size - 1];
        }




        public void Duplicate()
        {
            PushDatum(PeekDatum());
        }

        public void Swap()
        {
            var value1 = _buffer[_size];
            var value2 = _buffer[_size - 1];
            _buffer[_size - 1] = value2;
            _buffer[_size] = value1;
            //PushDatum(PeekDatum());
        }


        public void Clear()
        {
            Array.Clear(_buffer, 0, _size);
            _size = 0;
        }
    }
}

