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
            return new NumberValue(strValue._items.Count);
        }

        public static ScriptObject PUSH(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
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


        public static ScriptObject POP(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            if(thisObject is ScriptArray array)
            {
                return array.Pop();
            }
            throw new AuroraVMException("array is empty!");
        }


        public static ScriptObject SLICE(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as ScriptArray;
            var start = 0;
            var end = 0;
            if (args.Length > 0 && args[0] is NumberValue posNum)
            {
                start = posNum.Int32Value;
                if (args.Length > 1 && args[1] is NumberValue lenNum)
                {
                    end = lenNum.Int32Value;
                    return strValue.Slice(start,end);
                }
                return strValue.Slice(start);
            }
            return thisObject;
        }


    }
}
