using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptArray
    {
        private new readonly static ScriptObject Prototype;


        static ScriptArray()
        {
            Prototype = new ScriptObject();
            Prototype.Define("length", new ClrGetter(LENGTH), readable: true, writeable: false);
            Prototype.Define("push", new ClrFunction(PUSH), readable: true, writeable: false);
            Prototype.Define("pop", new ClrFunction(POP), readable: true, writeable: false);
            Prototype.Define("constructor", new ClrFunction(CONSTRUCTOR), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }

        public static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptArray;
            return new NumberValue(strValue._items.Count);
        }

        public static ScriptObject PUSH(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                foreach (var item in args)
                {
                    array.Push(item);
                }
            }
            return null;
        }


        public static ScriptObject POP(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if(thisObject  is ScriptArray array)
            {
                return array.Pop();
            }
            return null;
        }


        public new static ScriptObject CONSTRUCTOR(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            var array = new ScriptArray();
            return array;
        }



    }
}
