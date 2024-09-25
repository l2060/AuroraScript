using RoslynScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptRuner
{
    internal class rsTest
    {


        public static void Run()
        {
            ScriptManager scriptManager = new ScriptManager();


            scriptManager.LoadScriptsFromDirectory("sharps");

            scriptManager.RunScriptMethod("sharps/ScriptA.cs", "ScriptA", "PrintMessage", "Hello from script!");

        }




    }
}
