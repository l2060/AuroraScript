using System;
using System.Collections.Generic;
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

        public new static ScriptObject TOSTRING(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            return thisObject;
        }

        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as StringValue;
            return strValue.Length;
        }


        public static ScriptObject INDEXOF(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }

        public static ScriptObject LASTINDEXOF(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        

        public static ScriptObject STARTSWITH(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public static ScriptObject ENDSWITH(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public static ScriptObject SUBSTRING(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }

        public static ScriptObject SPLIT(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public static ScriptObject MATCH(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public new static ScriptObject CONSTRUCTOR(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            var array = new StringValue("");
            return array;
        }

        public static ScriptObject REPLACE(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject PADLEFT(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject PADRIGHT(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject TRIM(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject TRIMLEFT(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject TRIMRIGHT(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }
        public static ScriptObject SLICE(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }

    }
}
