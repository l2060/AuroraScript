
using AuroraScript.Runtime.Base;
using System;
using System.Diagnostics;

namespace AuroraScript.Runtime
{
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Name = "{Key,nq}", Type = "{DebuggerDisplayType,nq}")]

    internal sealed class ObjectProperty
    {
        public Boolean Readable;
        public Boolean Writable;
        public ScriptObject Value;
        public String Key;

        public Boolean Enumerable;


        public ObjectProperty(String key, bool writeable, bool readable, bool enumerable)
        {
            Key = key;
            Enumerable = readable;
            Readable = readable;
            Writable = writeable;
        }

        public ObjectProperty(String key, ScriptObject value, bool writeable, bool readable, bool enumerable)
        {
            Key = key;
            Value = value;
            Enumerable = readable;
            Readable = readable;
            Writable = writeable;
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplayValue
        {
            get
            {
                return Value.ToString();
            }
        }

        public ObjectProperty Clone()
        {
            return new ObjectProperty(Key, Value, Writable, Readable, Enumerable);
        }


    }
}
