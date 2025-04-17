using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    public abstract class Callable : ScriptObject
    {
        public abstract BoundFunction Bind(ScriptObject target);
        public abstract ScriptObject Invoke(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args);

    }
}
