using AuroraScript.Runtime.Base;
using AuroraScript.Core;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Types
{
    internal class ItemIterator : ScriptObject
    {
        private readonly ScriptDatum[] _items;
        private Int32 _index;
        private readonly Int32 _length;

        public ItemIterator(ScriptArray array)
        {
            _length = array.Length;
            _items = new ScriptDatum[_length];
            for (int i = 0; i < _length; i++)
            {
                _items[i] = array.GetDatum(i);
            }
            _index = 0;
        }

        public ItemIterator(ScriptDatum[] items)
        {
            _items = items ?? Array.Empty<ScriptDatum>();
            _length = _items.Length;
            _index = 0;
        }

        public static ItemIterator FromObjects(IList<ScriptObject> objects)
        {
            if (objects == null || objects.Count == 0)
            {
                return new ItemIterator(Array.Empty<ScriptDatum>());
            }

            var buffer = new ScriptDatum[objects.Count];
            for (int i = 0; i < objects.Count; i++)
            {
                buffer[i] = ScriptDatum.FromObject(objects[i]);
            }
            return new ItemIterator(buffer);
        }

        public static ItemIterator FromObjects(ScriptObject[] objects)
        {
            if (objects == null || objects.Length == 0)
            {
                return new ItemIterator(Array.Empty<ScriptDatum>());
            }

            var buffer = new ScriptDatum[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                buffer[i] = ScriptDatum.FromObject(objects[i]);
            }
            return new ItemIterator(buffer);
        }

        public ScriptObject Value()
        {
            return _items[_index].ToObject();
        }

        public Boolean HasValue()
        {
            return _index < _length;
        }

        public void Next()
        {
            _index++;
        }
    }
}
