using System;
using System.Runtime.CompilerServices;


namespace AuroraScript.Runtime
{
    internal class CallArguments
    {
        internal ScriptDatum[] _items = new ScriptDatum[16];
        private int viewSize;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref ScriptDatum GetRef(int index)
        {
            return ref _items[index];
        }

        public void Copy(ScriptDatum[] datums)
        {
            viewSize = datums.Length;
            EnsureCapacity(viewSize);
            for (int i = 0; i < viewSize; i++)
            {
                _items[i] = datums[i];
            }
        }



        public Span<ScriptDatum> ViewSpan(Int32 length)
        {
            viewSize = length;
            EnsureCapacity(length);
            return _items.AsSpan(0, length);
        }

        public Span<ScriptDatum> ViewSpan()
        {
            return _items.AsSpan(0, viewSize);
        }

        private void EnsureCapacity(Int32 min)
        {
            var _count = _items.Length;
            if (_count >= min) return;
            var newCapacity = _count == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min) newCapacity = min;
            var newArray = new ScriptDatum[newCapacity];
            if (_count > 0)
            {
                Array.Copy(_items, newArray, _count);
            }
            _items = newArray;
        }


        public Int32 Length => viewSize;

        public void Clean()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i] = default;
            }
        }

    }
}
