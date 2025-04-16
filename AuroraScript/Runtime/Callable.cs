using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime
{
    internal abstract class Callable : ScriptObject
    {
        public abstract ScriptObject Invoke(AuroraEngine engine, ScriptObject module, ScriptObject[] args);

    }
}
