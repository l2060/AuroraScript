using AuroraScript.Runtime.Base;
using System;


namespace AuroraScript.Runtime
{
    public class ScriptDomain
    {

        public readonly ScriptGlobal Global;

        public readonly AuroraEngine Engine;

        private readonly RuntimeVM _virtualMachine;

        internal ScriptDomain(AuroraEngine engine, RuntimeVM vm, ScriptGlobal domainGlobal)
        {
            Global = domainGlobal;
            Engine = engine;
            _virtualMachine = vm;
        }


        public ExecuteContext Execute(String moduleName, String methodName, ExecuteOptions options, params ScriptObject[] arguments)
        {
            ExecuteContext exeContext = new ExecuteContext(Global, _virtualMachine);
            var module = Global.GetPropertyValue("@" + moduleName);
            if (module == null)
            {
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new Exception("not found module " + moduleName));
                return exeContext;
            }
            var method = module.GetPropertyValue(methodName);
            if (method == null)
            {
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new Exception("not found method " + methodName));
                return exeContext;
            }
            if (method is not ClosureFunction closure)
            {
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new Exception(methodName + " is not method"));
                return exeContext;
            }
            exeContext._callStack.Push(new CallFrame(closure.Environment, Global, closure.Module, closure.EntryPointer, arguments));
            _virtualMachine.Execute(exeContext);
            return exeContext;
        }


        public ExecuteContext Execute(String moduleName, String methodName, params ScriptObject[] arguments)
        {
            return Execute(moduleName, methodName, ExecuteOptions.Default, arguments);
        }


        public ExecuteContext Execute(ClosureFunction closure, ExecuteOptions options, params ScriptObject[] arguments)
        {
            ExecuteContext exeContext = new ExecuteContext(Global, _virtualMachine);
            exeContext._callStack.Push(new CallFrame(closure.Environment, Global, closure.Module, closure.EntryPointer, arguments));
            _virtualMachine.Execute(exeContext);
            return exeContext;
        }


        public ExecuteContext Execute(ClosureFunction closure, params ScriptObject[] arguments)
        {
            return Execute(closure, ExecuteOptions.Default, arguments);
        }




        public ScriptObject GetGlobal(string name)
        {
            return Global.GetPropertyValue(name);
        }


        public void SetGlobal(string name, ScriptObject value)
        {
            Global.SetPropertyValue(name, value);
        }




    }
}
