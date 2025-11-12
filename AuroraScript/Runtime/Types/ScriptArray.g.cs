using AuroraScript.Core;
using AuroraScript.Exceptions;
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
        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as ScriptArray;
            return NumberValue.Of(strValue.Length);
        }

        public static ScriptObject PUSH(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is ScriptArray array && args != null)
            {
                foreach (var datum in args)
                {
                    array.PushDatum(datum);
                }
            }
            return ScriptObject.Null;
        }


        public static ScriptObject POP(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if(thisObject is ScriptArray array)
            {
                return array.PopDatum().ToObject();
            }
            throw new AuroraVMException("array is empty!");
        }


        public static ScriptObject SLICE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not ScriptArray strValue)
            {
                return thisObject;
            }

            if (args == null || args.Length == 0)
            {
                return thisObject;
            }

            var start = 0;
            var arg0 = args[0];
            if (arg0.Kind == ValueKind.Number)
            {
                start = (Int32)arg0.Number;
            }
            else
            {
                var startObj = arg0.ToObject();
                if (startObj is NumberValue posNum)
                {
                    start = posNum.Int32Value;
                }
            }

            if (args.Length > 1)
            {
                var arg1 = args[1];
                if (arg1.Kind == ValueKind.Number)
                {
                    var end = (Int32)arg1.Number;
                    return strValue.Slice(start, end);
                }

                var endObj = arg1.ToObject();
                if (endObj is NumberValue lenNum)
                {
                    return strValue.Slice(start, lenNum.Int32Value);
                }
            }

            return strValue.Slice(start);
        }


    }
}
