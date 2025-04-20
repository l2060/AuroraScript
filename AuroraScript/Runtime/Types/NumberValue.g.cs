using AuroraScript.Exceptions;
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
        public static readonly NumberValue Negative1 = new NumberValue(-1);
        public static readonly NumberValue NaN = new NumberValue(Double.NaN);
        public static readonly NumberValue Zero = new NumberValue(0);
        public static readonly NumberValue Num1 = new NumberValue(1);
        public static readonly NumberValue Num2 = new NumberValue(2);
        public static readonly NumberValue Num3 = new NumberValue(3);
        public static readonly NumberValue Num4 = new NumberValue(4);
        public static readonly NumberValue Num5 = new NumberValue(5);
        public static readonly NumberValue Num6 = new NumberValue(6);
        public static readonly NumberValue Num7 = new NumberValue(7);
        public static readonly NumberValue Num8 = new NumberValue(8);
        public static readonly NumberValue Num9 = new NumberValue(9);


        public new static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            var thisNumber = thisObject as NumberValue;
            if (args.Length == 1 && args[0] is NumberValue num)
            {
                if (num.Int32Value == 16)
                {
                    var strValue = thisNumber.Int32Value.ToString("X");
                    return StringValue.Of(strValue);
                }
                throw new AuroraVMException("未实现的");
            }
            return StringValue.Of(thisNumber._value.ToString());
        }


    }
}
