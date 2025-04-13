using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        protected readonly static ScriptObject Prototype;

        static ScriptObject()
        {
            Prototype = new ScriptObject();
            Prototype.Define("toString", new ClrFunction(TOSTRING), readable: true, writeable: false);
            Prototype.Define("constructor", new ClrFunction(CONSTRUCTOR), readable: true, writeable: false);
            Prototype.Define("length", new ClrGetter(LENGTH), readable: true, writeable: false);
            Prototype.IsFrozen = true;
        }


        public static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptObject;
            return new NumberValue(strValue._properties.Count);
        }


        public static ScriptObject TOSTRING(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            return new StringValue(thisObject.ToString());
        }


        public static ScriptObject CONSTRUCTOR(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            var array = new ScriptObject();
            return array;
        }
    }
}
