using AuroraScript.Runtime.Types;
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

        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptArray;
            return new NumberValue(strValue._items.Count);
        }

        public static ScriptObject PUSH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is ScriptArray array)
            {
                foreach (var item in args)
                {
                    array.Push(item);
                }
            }
            return ScriptObject.Null;
        }


        public static ScriptObject POP(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if(thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            return ScriptObject.Null;
        }


        public new static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            int capacity = 0;
            if (args.Length == 1 && args[0] is NumberValue numberValue)
            {
                capacity = numberValue.Int32Value;
            }
            var array = new ScriptArray(capacity);
            return array;
        }



    }
}
