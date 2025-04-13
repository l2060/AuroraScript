using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Compiler.Emits
{
    public class StringList
    {
        private readonly List<string> _stringTable = new List<string>();


        public Int32 GetSlot(String str)
        {
            var index = _stringTable.IndexOf(str);
            if (index > -1) return index;
            _stringTable.Add(str);
            return _stringTable.Count - 1;
        }



        public ImmutableArray<String> List => _stringTable.ToImmutableArray();



    }
}
