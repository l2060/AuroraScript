
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
{
    internal class ObjectProperty
    {
        public Boolean Readable;
        public Boolean Writeeable;

        public ScriptObject Value;


        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }

    }
}
