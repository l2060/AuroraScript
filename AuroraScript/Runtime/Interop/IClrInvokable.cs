using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Interop
{
    internal interface IClrInvokable
    {
        ScriptDatum Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args);
    }
}

