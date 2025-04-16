using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    internal class CapturedVariablee : ScriptObject
    {
        private readonly CallFrame Frame;
        private readonly Int32 VarIndex;

        public CapturedVariablee(CallFrame frame, Int32 varIndex)
        {
            Frame = frame;
            VarIndex = varIndex;
        }

        public ScriptObject Read() => Frame.Locals[VarIndex];

        public void Write(ScriptObject value) => Frame.Locals[VarIndex] = value;

        public override string ToString()
        {
            return "CapturedVariablee";
        }

    }
}
