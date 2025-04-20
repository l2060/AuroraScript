using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        public static readonly ScriptObject Null = NullValue.Instance;
        public static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptObject;
            return new NumberValue(strValue._properties.Count);
        }


        public static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            return new StringValue(thisObject.ToString());
        }


    }
}
