using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime
{
    public abstract class ScriptValue : ScriptObject
    {

        internal ScriptValue(ScriptObject prototype) : base(prototype, false)
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

        public override void Define(String key, ScriptObject value, bool writeable = true, bool readable = true, bool enumerable = true)
        {

        }




    }
}
