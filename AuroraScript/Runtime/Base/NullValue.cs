using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public class NullValue : ScriptObject
    {
        private static StringValue valueString = new StringValue("null");

        public readonly static NullValue Instance = new NullValue();

        private NullValue()
        {
            this.IsFrozen = true;
            this._prototype = new ScriptObject();
            this._prototype.Define("toString", new ClrFunction(TOSTRING), true, false);
            this._prototype.IsFrozen = true;
        }


        public new static ScriptObject TOSTRING(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {           
            return valueString;
        }

        public override ScriptObject GetPropertyValue(String key, ScriptObject own = null)
        {
            throw new Exception(String.Format("Cannot read properties of undefined (reading '{0}')", key));
        }

        public override void SetPropertyValue(String key, ScriptObject value)
        {
            throw new Exception(String.Format("Cannot read properties of undefined (reading '{0}')", key));
        }


        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true)
        {
            throw new Exception(String.Format("Cannot read properties of undefined (reading '{0}')", key));
        }

        public override string ToString()
        {
            return "NULL_VALUE";
        }

        public override string ToDisplayString()
        {
            return "null";
        }

    }

}
