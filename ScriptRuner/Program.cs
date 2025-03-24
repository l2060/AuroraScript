// See https://aka.ms/new-console-template for more information
using AuroraScript;
using ScriptRuner;

//rsTest.Run();


var engine = new ScriptEngine();

engine.build("./scripts/main.ts");

Console.WriteLine("=====================================================================================");
Console.ReadKey();