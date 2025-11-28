using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

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


        private ItemIterator(string value)
        {
            _kind = IteratorKind.String;
            _stringValue = value ?? string.Empty;
            _length = _stringValue.Length;
            _index = 0;
        }



        public static ItemIterator FromString(string value)
        {
            return new ItemIterator(value);
        }

        public ScriptDatum Value()
        {
            return _kind switch
            {
                IteratorKind.ScriptArray => _array.Get(_index),
                IteratorKind.String => ScriptDatum.FromString(StringValue.FromChar(_stringValue[_index])),
                IteratorKind.DatumArray => _datumItems[_index],
                _ => _datumItems[_index],
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
