using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptArray : ScriptObject, IEnumerator
    {
        private readonly List<ScriptObject> _items;

        public ScriptArray(Int32 capacity)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            this._items = new List<ScriptObject>(new ScriptObject[capacity]);
            for (int i = 0; i < capacity; i++)
            {
                this._items[i] = ScriptObject.Null;
            }
        }
        public ScriptArray(List<ScriptObject> list)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            this._items = new List<ScriptObject>(list);
        }

        public ScriptArray(ScriptObject[] array)
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            this._items = new List<ScriptObject>(array);
        }

        public ScriptArray()
        {
            this._prototype = Prototypes.ScriptArrayPrototype;
            this._items = new List<ScriptObject>();
        }


        public ScriptObject GetElement(NumberValue index)
        {
            return this._items[index.Int32Value];
        }

        public void SetElement(NumberValue index, ScriptObject value)
        {
            while (this._items.Count <= index.Int32Value)
            {
                this._items.Add(ScriptObject.Null);
            }
            this._items[index.Int32Value] = value;
        }

        public void Push(ScriptObject item)
        {
            _items.Add(item);
        }

        public ScriptArray Slice(Int32 start, Int32 end)
        {
            return new ScriptArray(_items.Slice(start, end - start));
        }

        public ScriptArray Slice(Int32 start)
        {
            return new ScriptArray(_items.Slice(start, _items.Count - start));
        }

        public ScriptObject Pop()
        {
            if (_items.Count > 0)
            {
                var item = _items[_items.Count - 1];
                _items.RemoveAt(_items.Count - 1);
                return item;
            }
            return null;
        }

        public override void SetPropertyValue(String key, ScriptObject value)
        {

        }

        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true, bool enumerable = true)
        {

        }

        public override string ToString()
        {
            return "[" + String.Join(", ", _items) + "]";
        }

        public override string ToDisplayString()
        {
            return "[" + String.Join(", ", _items.Select(e => e.ToDisplayString())) + "]";
        }

        ItemIterator IEnumerator.GetIterator()
        {
            return new ItemIterator(_items);
        }

        public Int32 Length
        {
            get
            {
                return _items.Count;
            }
        }

    }
}
