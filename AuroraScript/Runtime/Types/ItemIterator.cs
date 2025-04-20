using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Types
{
    internal class ItemIterator : ScriptObject
    {
        private IReadOnlyList<ScriptObject> _items;
        private int _index = 0;


        public ItemIterator(IReadOnlyList<ScriptObject> items)
        {
            _items = items ?? [];
        }

        public ScriptObject Value()
        {
            return _items[_index];
        }

        public Boolean HasValue()
        {
            return _index < _items.Count;
        }

        public void Next()
        {
            _index++;
        }




    }
}
