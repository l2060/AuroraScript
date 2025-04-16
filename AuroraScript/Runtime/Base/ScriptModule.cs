namespace AuroraScript.Runtime.Base
{
    internal class ScriptModule : ScriptObject
    {
        public String Name;

        public ScriptModule(String moduleName)
        {
            Name = moduleName;
        }


        public override string ToString()
        {
            return $"module: {Name}";
        }

    }
}
