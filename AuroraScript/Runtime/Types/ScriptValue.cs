using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime
{
    public abstract class ScriptValue : ScriptObject
    {

        internal ScriptValue(ScriptObject prototype) : base(prototype, true)
        {
            Frozen();
        }


        public override void SetPropertyValue(String key, ScriptObject value)
        {

        }

        public override Boolean DeletePropertyValue(String key)
        {
            return false;
        }

        public override void Define(string key, ScriptObject value, bool writeable = true, bool enumerable = true)
        {
        }

    }
}
