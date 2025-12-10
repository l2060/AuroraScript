using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AuroraScript.Runtime.Interop
{
    internal interface IClrInvokable
    {
        void Invoke([NotNull] ExecuteContext context, ScriptObject thisObject, [NotNull] Span<ScriptDatum> args, ref ScriptDatum result);
    }
}

