using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    public abstract class ScriptValue : ScriptObject
    {
        protected ScriptValue()
        {
            IsFrozen = true;
        }



        public override void SetPropertyValue(string key, ScriptObject value)
        {

        }

        public override void Define(string key, ScriptObject value, bool readable = true, bool writeable = true)
        {

        }





    }
}
