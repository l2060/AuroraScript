using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class NullValue : ScriptObject
    {
        private static StringValue valueString = new StringValue("null");

        public readonly static NullValue Instance = new NullValue();

        private NullValue()
        {
            _prototype = Prototypes.NullValuePrototype;
            Frozen();
        }


        public new static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            return valueString;
        }

        public override ScriptObject GetPropertyValue(string key)
        {
            throw new AuroraVMException(string.Format("Cannot read properties of undefined (reading '{0}')", key));
        }

        public override void SetPropertyValue(string key, ScriptObject value)
        {
            throw new AuroraVMException(string.Format("Cannot read properties of undefined (reading '{0}')", key));
        }


        public override void Define(string key, ScriptObject value, bool readable = true, bool writeable = true, bool enumerable = true)
        {
            throw new AuroraVMException(string.Format("Cannot read properties of undefined (reading '{0}')", key));
        }

        public override string ToString()
        {
            return "NULL_VALUE";
        }

        public override string ToDisplayString()
        {
            return "null";
        }

        public override bool IsTrue()
        {
            return false;
        }

    }

}
