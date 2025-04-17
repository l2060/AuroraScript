using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue
    {


        private new readonly static ScriptObject Prototype;


        static StringValue()
        {
            Prototype = new ScriptObject();

            Prototype.Define("constructor", new ClrFunction(CONSTRUCTOR), readable: true, writeable: false);
            Prototype.Define("length", new ClrGetter(LENGTH), readable: true, writeable: false);
            Prototype.Define("contains", new ClrFunction(CONTANINS), readable: true, writeable: false);
            Prototype.Define("indexOf", new ClrFunction(INDEXOF), readable: true, writeable: false);
            Prototype.Define("lastIndexOf", new ClrFunction(LASTINDEXOF), readable: true, writeable: false);
            Prototype.Define("startsWith", new ClrFunction(STARTSWITH), readable: true, writeable: false);
            Prototype.Define("endsWith", new ClrFunction(ENDSWITH), readable: true, writeable: false);
            Prototype.Define("substring", new ClrFunction(SUBSTRING), readable: true, writeable: false);
            Prototype.Define("split", new ClrFunction(SPLIT), readable: true, writeable: false);
            Prototype.Define("match", new ClrFunction(MATCH), readable: true, writeable: false);
            Prototype.Define("replace", new ClrFunction(REPLACE), readable: true, writeable: false);
            Prototype.Define("padLeft", new ClrFunction(PADLEFT), readable: true, writeable: false);
            Prototype.Define("padRight", new ClrFunction(PADRIGHT), readable: true, writeable: false);
            Prototype.Define("trim", new ClrFunction(TRIM), readable: true, writeable: false);
            Prototype.Define("trimLeft", new ClrFunction(TRIMLEFT), readable: true, writeable: false);
            Prototype.Define("trimRight", new ClrFunction(TRIMRIGHT), readable: true, writeable: false);
            Prototype.Define("slice", new ClrFunction(SLICE), readable: true, writeable: false);
            Prototype.Define("toString", new ClrFunction(TOSTRING), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }

        public new static ScriptObject TOSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            return thisObject;
        }

        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as StringValue;
            return NumberValue.Of(strValue.Value.Length);
        }


        public static ScriptObject CONTANINS(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.Contains(str.Value));
        }

        public static ScriptObject INDEXOF(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return NumberValue.Of(strValue.Value.IndexOf(str.Value));
        }

        public static ScriptObject LASTINDEXOF(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return NumberValue.Of(strValue.Value.LastIndexOf(str.Value));
        }




        public static ScriptObject STARTSWITH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.StartsWith(str.Value));
        }


        public static ScriptObject ENDSWITH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.EndsWith(str.Value));
        }


        public static ScriptObject SUBSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return StringValue.Of(strValue.Value.Substring(1, 1));
        }

        public static ScriptObject SPLIT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public static ScriptObject MATCH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public new static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var array = new StringValue("");
            return array;
        }

        public static ScriptObject REPLACE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject PADLEFT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject PADRIGHT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is StringValue str)
            {
                return StringValue.Of(str.Value.Trim());
            }
            return null;
        }
        public static ScriptObject TRIM(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is StringValue str)
            {
                return StringValue.Of(str.Value.Trim());
            }
            return null;
        }
        public static ScriptObject TRIMLEFT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject TRIMRIGHT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject SLICE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }

    }
}
