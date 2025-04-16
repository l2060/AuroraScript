
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    public class ObjectProperty
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
