// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Core;

//rsTest.Run();




var engine = new ScriptEngine();

engine.build("./scripts/unit.as");

Console.WriteLine("=====================================================================================");
Console.ReadKey();