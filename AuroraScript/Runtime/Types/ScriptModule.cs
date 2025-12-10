using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    public sealed class ScriptModule : ScriptObject
    {
        public string Name;

        public ScriptModule(string moduleName)
        {
            Name = moduleName;
        }


        public override string ToString()
        {
            return $"module: {Name}";
        }

    }
}
