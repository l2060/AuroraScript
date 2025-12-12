
using AuroraScript.Runtime.Base;
using System;
using System.Diagnostics;

namespace AuroraScript.Runtime
{
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Name = "{Key,nq}", Type = "{DebuggerDisplayType,nq}")]

    internal sealed class ObjectProperty
    {
        internal ScriptObject Value;
        internal Boolean Enumerable;
        internal Boolean Writable;
        internal String Key;

        internal ObjectProperty(String key, bool writeable, bool enumerable)
        {
            Key = key;
            Enumerable = enumerable;
            Writable = writeable;
        }

        internal ObjectProperty(String key, ScriptObject value, bool writeable, bool enumerable)
        {
            Key = key;
            Value = value;
            Writable = writeable;
            Enumerable = enumerable;
        }


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplayValue
        {
            get
            {
                return Value.ToString();
            }
        }

        internal ObjectProperty Clone()
        {
            return new ObjectProperty(Key, Value, Writable, Enumerable);
        }


    }
}
