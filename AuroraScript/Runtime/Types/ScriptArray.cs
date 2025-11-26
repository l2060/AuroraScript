using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptArray : ScriptObject, IEnumerator
    {
        private ScriptDatum[] _items;
        private Int32 _count;

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
                    _items[i] = ScriptDatum.FromNull();
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


        public ScriptObject GetElement(Int32 index)
        {
            if (index < 0 || index >= _count)
            {
                return ScriptObject.Null;
            }
            return _items[index].ToObject();
        }

        public void SetElement(NumberValue index, ScriptObject value)
        {
            SetDatum(index.Int32Value, ScriptDatum.FromObject(value));
        }

        internal void SetDatum(Int32 index, ScriptDatum datum)
        {
            if (index < 0) return;
            EnsureCapacity(index + 1);

            if (index >= _count)
            {
                for (int i = _count; i < index; i++)
                {
                    _items[i] = ScriptDatum.FromNull();
                }
                _count = index + 1;
            }

            _items[index] = datum;
        }

        public void Push(ScriptObject item)
        {
            PushDatum(ScriptDatum.FromObject(item));
        }

        public void PushDatum(ScriptDatum datum)
        {
            SetDatum(_count, datum);
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

        public ScriptObject Pop()
        {
            return PopDatum().ToObject();
        }

        public override void SetPropertyValue(String key, ScriptObject value)
        {
            base.SetPropertyValue(key, value);
        }

        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true, bool enumerable = true)
        {
            base.Define(key, value, readable, writeable, enumerable);
        }

        public override string ToString()
        {
            if (_count == 0) return "[]";
            var parts = new string[_count];
            for (int i = 0; i < _count; i++)
            {
                parts[i] = _items[i].ToObject()?.ToString();
            }
            return "[" + String.Join(", ", parts) + "]";
        }

        public override string ToDisplayString()
        {
            if (_count == 0) return "[]";
            var parts = new string[_count];
            for (int i = 0; i < _count; i++)
            {
                parts[i] = _items[i].ToObject()?.ToDisplayString();
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

        internal ScriptDatum GetDatum(Int32 index)
        {
            if (index < 0 || index >= _count) return ScriptDatum.FromNull();
            return _items[index];
        }

        internal ScriptDatum PopDatum()
        {
            if (_count > 0)
            {
                var datum = _items[--_count];
                _items[_count] = ScriptDatum.FromNull();
                return datum;
            }
            return ScriptDatum.FromNull();
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
