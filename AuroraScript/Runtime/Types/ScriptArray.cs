using AuroraScript.Runtime.Types;
using AuroraScript.Runtime.Types.Internal;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Base
{
    public sealed partial class ScriptArray : ScriptObject, IEnumerator
    {
        private ScriptDatum[] _items;
        private Int32 _count;

        public ScriptArray(ScriptArray array)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            var capacity = array._count;
            _items = new ScriptDatum[Math.Max(4, capacity)];
            if (capacity > 0)
            {
                Array.Copy(array._items, _items, capacity);
            }
        }


        public ScriptArray(Int32 capacity)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            if (capacity <= 0)
            {
                _items = Array.Empty<ScriptDatum>();
                _count = 0;
            }
            else
            {
                _items = new ScriptDatum[Math.Max(4, capacity)];
                _count = capacity;
                for (int i = 0; i < _count; i++)
                {
                    _items[i] = ScriptDatum.Null;
                }
            }
        }
        public ScriptArray(List<ScriptObject> list)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            if (list == null || list.Count == 0)
            {
                _items = Array.Empty<ScriptDatum>();
                _count = 0;
            }
            else
            {
                _items = new ScriptDatum[Math.Max(4, list.Count)];
                _count = list.Count;
                for (int i = 0; i < _count; i++)
                {
                    _items[i] = ScriptDatum.FromObject(list[i]);
                }
            }
        }

        public ScriptArray(Span<ScriptDatum> array)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            if (array.Length == 0)
            {
                _items = Array.Empty<ScriptDatum>();
                _count = 0;
            }
            else
            {
                _items = new ScriptDatum[Math.Max(4, array.Length)];
                _count = array.Length;
                for (int i = 0; i < _count; i++)
                {
                    _items[i] = array[i];
                }
            }
        }



        public ScriptArray(ScriptObject[] array)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            if (array == null || array.Length == 0)
            {
                _items = Array.Empty<ScriptDatum>();
                _count = 0;
            }
            else
            {
                _items = new ScriptDatum[Math.Max(4, array.Length)];
                _count = array.Length;
                for (int i = 0; i < _count; i++)
                {
                    _items[i] = ScriptDatum.FromObject(array[i]);
                }
            }
        }

        public ScriptArray()
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            this._items = Array.Empty<ScriptDatum>();
            this._count = 0;
        }


        public ScriptDatum Get(Int32 index)
        {
            if (index < 0 || index >= _count) return ScriptDatum.Null;
            return _items[index];
        }

        public void Get(Int32 index, out ScriptDatum scriptDatum)
        {
            if (index < 0 || index >= _count)
            {
                scriptDatum = ScriptDatum.Null;
            }
            scriptDatum = _items[index];
        }


        public void Set(Int32 index, ScriptDatum datum)
        {
            if (index < 0) return;
            if (index >= _items.Length) EnsureCapacity(index + 1);
            if (index >= _count)
            {
                _count = index + 1;
            }
            _items[index] = datum;
        }
        public void SetRef(Int32 index, ref ScriptDatum datum)
        {
            if (index < 0) return;
            if (index >= _items.Length) EnsureCapacity(index + 1);
            if (index >= _count)
            {
                _count = index + 1;
            }
            _items[index] = datum;
        }

        public void PushDatum(ScriptDatum datum)
        {
            Set(_count, datum);
        }

        public ScriptArray Slice(Int32 start, Int32 end)
        {
            if (start < 0) start = 0;
            if (end > _count) end = _count;
            if (end < start) end = start;
            var length = end - start;
            var slice = new ScriptArray();
            if (length <= 0)
            {
                slice.EnsureCapacity(0);
                slice._count = 0;
                return slice;
            }
            slice.EnsureCapacity(length);
            slice._count = length;
            Array.Copy(_items, start, slice._items, 0, length);
            return slice;
        }

        public ScriptArray Slice(Int32 start)
        {
            return Slice(start, _count);
        }

        public Span<ScriptDatum> Values()
        {
            return _items.AsSpan(0, _count);
        }


        public override string ToString()
        {
            if (_count == 0) return "[]";
            var parts = new string[_count];
            for (int i = 0; i < _count; i++)
            {
                parts[i] = _items[i].ToString();
            }
            return "[" + String.Join(", ", parts) + "]";
        }


        ItemIterator IEnumerator.GetIterator()
        {
            return new ItemIterator(this);
        }

        public Int32 Length
        {
            get
            {
                return _count;
            }
        }



        internal ScriptDatum PopDatum()
        {
            if (_count > 0)
            {
                var datum = _items[--_count];
                _items[_count] = ScriptDatum.Null;
                return datum;
            }
            return ScriptDatum.Null;
        }

        private void EnsureCapacity(Int32 min)
        {
            if (_items.Length >= min) return;
            var newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;
            if (newCapacity < min) newCapacity = min;
            var newArray = new ScriptDatum[newCapacity];
            if (_count > 0)
            {
                Array.Copy(_items, newArray, _count);
            }
            _items = newArray;
        }
    }
}
