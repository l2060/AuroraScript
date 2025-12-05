using AuroraScript.Core;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime.Base
{
    public partial class ScriptObject
    {
        public static readonly ScriptObject Null = NullValue.Instance;

        public static ScriptObject LENGTH(ScriptObject thisObject)
        {
            if (thisObject is ScriptObject obj && obj._properties != null)
            {
                return NumberValue.Of(obj._properties.Count);
            }
            return NumberValue.Zero;
        }

        public static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return StringValue.Of(thisObject?.ToString() ?? StringValue.Empty.Value);
        }
    }
}
