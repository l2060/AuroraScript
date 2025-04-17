using System;

namespace AuroraScript.Runtime.Base
{
    public abstract class ScriptValue : ScriptObject
    {
        protected ScriptValue()
        {
            IsFrozen = true;
        }



        public override void SetPropertyValue(String key, ScriptObject value)
        {

        }

        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true)
        {

        }





    }
}
