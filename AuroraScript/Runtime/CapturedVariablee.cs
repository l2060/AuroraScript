using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    internal class CapturedVariablee : ScriptObject
    {
        private readonly CallFrame Frame;
        private readonly Int32 VarIndex;

        public CapturedVariablee(CallFrame frame, Int32 varIndex) : base(null, false)
        {
            Frame = frame;
            VarIndex = varIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptObject Read() => Frame.GetLocalDatum(VarIndex).ToObject();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ScriptObject value) => Frame.SetLocalDatum(VarIndex, ScriptDatum.FromObject(value));

        public override string ToString()
        {
            return "CapturedVariablee";
        }

    }
}
