using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        internal static readonly ScriptObject Null = NullValue.Instance;

        internal static void LENGTH(ScriptObject thisObject, ref ScriptDatum result)
        {
            if (thisObject is ScriptObject obj && obj._properties != null)
            {
                result = ScriptDatum.FromNumber(obj._properties.Count);
            }
            else
            {
                result = ScriptDatum.FromNumber(0);
            }
        }

        internal static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            result = ScriptDatum.FromString(thisObject.ToString());
        }






    }
}
