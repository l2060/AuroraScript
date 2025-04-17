using System;
using System.Collections.Generic;
using System.Linq;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptArray : ScriptObject
    {
        private readonly List<ScriptObject> _items;

        public ScriptArray(ScriptObject[] array)
        {
            this._prototype = ScriptArray.Prototype;
            this._items = new List<ScriptObject>(array);
        }

        public ScriptArray()
        {
            this._prototype = ScriptArray.Prototype;
            this._items = new List<ScriptObject>();
        }


        public ScriptObject GetElement(NumberValue index)
        {
            return this._items[index.Int32Value];
        }

        public void SetElement(NumberValue index, ScriptObject value)
        {
            this._items[index.Int32Value] = value;
        }

        public void Push(ScriptObject item)
        {
            _items.Add(item);
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

        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true)
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

        public Int32 Length
        {
            get
            {
                return _items.Count;
            }
        }

    }
}
