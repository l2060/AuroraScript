using AuroraScript.Runtime.Base;


namespace AuroraScript.Runtime
{
    public class ScriptDomain
    {
        /// <summary>
        /// Doamin Global
        /// </summary>
        public readonly ScriptGlobal Global;

        public readonly AuroraEngine Engine;

        private readonly RuntimeVM virtualMachine;

        internal ScriptDomain(AuroraEngine engine, RuntimeVM vm, ScriptGlobal domainGlobal)
        {
            Global = domainGlobal;
            Engine = engine;
            virtualMachine = vm;
        }


        public ScriptObject Execute(String moduleName, String methodName, params ScriptObject[] args)
        {
            var module = Global.GetPropertyValue("@" + moduleName);
            if (module == null) throw new Exception("not found module " + moduleName);
            var method = module.GetPropertyValue(methodName);
            if (method == null) throw new Exception("not found method " + methodName);
            if (method is ClosureFunction closure)
            {
                return virtualMachine.Execute(closure, args);
            }
            throw new Exception(methodName + " is not method");
        }


        public ScriptObject Execute(ClosureFunction closure, params ScriptObject[] args)
        {
            return virtualMachine.Execute(closure, args);
        }




    }
}
