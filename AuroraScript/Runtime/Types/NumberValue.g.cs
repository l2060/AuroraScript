using AuroraScript.Core;
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


        public new static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not NumberValue thisNumber)
            {
                return ScriptObject.Null;
            }

            if (args != null && args.Length == 1)
            {
                var arg = args[0];
                if (arg.Kind == ValueKind.Number)
                {
                    if ((Int32)arg.Number == 16)
                    {
                        return StringValue.Of(thisNumber.Int32Value.ToString("X"));
                    }
                    throw new AuroraVMException("未实现的");
                }

                var obj = arg.ToObject();
                if (obj is NumberValue num && num.Int32Value == 16)
                {
                    return StringValue.Of(thisNumber.Int32Value.ToString("X"));
                }
                if (obj is NumberValue)
                {
                    throw new AuroraVMException("未实现的");
                }
            }

            return StringValue.Of(thisNumber._value.ToString());
        }
    }
}
