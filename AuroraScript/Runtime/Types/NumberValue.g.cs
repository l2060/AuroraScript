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


        internal static readonly NumberValue Negative1 = new NumberValue(-1);
        internal static readonly NumberValue NaN = new NumberValue(Double.NaN);
        internal static readonly NumberValue Zero = new NumberValue(0);
        internal static readonly NumberValue Num1 = new NumberValue(1);
        internal static readonly NumberValue Num2 = new NumberValue(2);
        internal static readonly NumberValue Num3 = new NumberValue(3);
        internal static readonly NumberValue Num4 = new NumberValue(4);
        internal static readonly NumberValue Num5 = new NumberValue(5);
        internal static readonly NumberValue Num6 = new NumberValue(6);
        internal static readonly NumberValue Num7 = new NumberValue(7);
        internal static readonly NumberValue Num8 = new NumberValue(8);
        internal static readonly NumberValue Num9 = new NumberValue(9);


        internal new static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args , ref ScriptDatum result)
        {
            if (thisObject is not NumberValue thisNumber)
            {
                result = ScriptDatum.Null;
                return;
            }

            if (args != null && args.Length == 1)
            {
                var arg = args[0];
                if (arg.Kind == ValueKind.Number)
                {
                    if ((Int32)arg.Number == 16)
                    {
                        result = ScriptDatum.FromString(thisNumber.Int32Value.ToString("X"));
                        return ;
                    }
                    throw new AuroraVMException("未实现的");
                }
            }
            result = ScriptDatum.FromString(thisNumber._value.ToString());
        }
    }
}
