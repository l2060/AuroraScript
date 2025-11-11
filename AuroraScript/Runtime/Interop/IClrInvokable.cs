using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;

namespace AuroraScript.Runtime.Interop
{
    internal interface IClrInvokable
    {
        ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args);
    }
}

