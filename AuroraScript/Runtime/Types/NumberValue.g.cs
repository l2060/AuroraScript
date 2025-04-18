using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AuroraScript.Runtime.Base
{
    public partial class NumberValue
    {

        private new readonly static ScriptObject Prototype;


        static NumberValue()
        {
            Prototype = new ScriptObject();
            Prototype.Define("constructor", new NumberConstructor(), readable: true, writeable: false);
            Prototype.Define("toString", new ClrFunction(TOSTRING), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }




        public new static ScriptObject TOSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var thisNumber = thisObject as NumberValue;
            if (args.Length == 1 && args[0] is NumberValue num)
            {
                if (num.Int32Value == 16)
                {
                    var strValue = thisNumber.Int32Value.ToString("X");
                    return StringValue.Of(strValue);
                }
                throw new Exception("未实现的");
            }
            return StringValue.Of(thisNumber._value.ToString());
        }


    }
}
