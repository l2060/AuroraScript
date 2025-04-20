
using AuroraScript.Runtime.Base;
using System;
using System.Diagnostics;

namespace AuroraScript.Runtime
{
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Name = "{Key,nq}", Type = "{DebuggerDisplayType,nq}")]

    internal class ObjectProperty
    {
        public StringValue Key;

        public Boolean Readable;
        public Boolean Writable;
        public Boolean Enumerable;
        public ScriptObject Value;



        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                return Value.ToString();
            }
        }
    }
}
