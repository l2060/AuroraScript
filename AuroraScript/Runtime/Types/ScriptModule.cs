using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{
    public class ScriptModule : ScriptObject
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
