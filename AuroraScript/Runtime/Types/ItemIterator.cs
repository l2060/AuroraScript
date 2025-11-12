using AuroraScript.Runtime.Base;
using AuroraScript.Core;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Types
{
    internal class ItemIterator : ScriptObject
    {
        private enum IteratorKind
        {
            DatumArray,
            ScriptArray,
            ObjectList,
            String
        }

        private readonly IteratorKind _kind;
        private readonly ScriptDatum[] _datumItems;
        private readonly ScriptArray _array;
        private readonly IList<ScriptObject> _objectItems;
        private readonly string _stringValue;
        private readonly Int32 _length;
        private Int32 _index;

        public ItemIterator(ScriptArray array)
        {
            _kind = IteratorKind.ScriptArray;
            _array = array;
            _length = array?.Length ?? 0;
            _index = 0;
        }

        public ItemIterator(ScriptDatum[] items)
        {
            _kind = IteratorKind.DatumArray;
            _datumItems = items ?? Array.Empty<ScriptDatum>();
            _length = _datumItems.Length;
            _index = 0;
        }

        private ItemIterator(IList<ScriptObject> objects)
        {
            _kind = IteratorKind.ObjectList;
            _objectItems = objects ?? Array.Empty<ScriptObject>();
            _length = _objectItems.Count;
            _index = 0;
        }

        private ItemIterator(string value)
        {
            _kind = IteratorKind.String;
            _stringValue = value ?? string.Empty;
            _length = _stringValue.Length;
            _index = 0;
        }

        public static ItemIterator FromObjects(IList<ScriptObject> objects)
        {
            return new ItemIterator(objects ?? Array.Empty<ScriptObject>());
        }

        public static ItemIterator FromObjects(ScriptObject[] objects)
        {
            return new ItemIterator(objects ?? Array.Empty<ScriptObject>());
        }

        public static ItemIterator FromString(string value)
        {
            return new ItemIterator(value);
        }

        public ScriptObject Value()
        {
            return _kind switch
            {
                IteratorKind.ScriptArray => _array?.GetDatum(_index).ToObject(),
                IteratorKind.ObjectList => _objectItems[_index],
                IteratorKind.String => StringValue.FromChar(_stringValue[_index]),
                _ => _datumItems[_index].ToObject(),
            };
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
