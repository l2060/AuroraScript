// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Core;

//rsTest.Run();




var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./scripts/" });
try
{
    engine.BuildAsync("./unit.as").Wait();
}
catch (Exception ex)
{
    Console.WriteLine($"[Error]: {ex.Message}");
}


Console.WriteLine("=====================================================================================");
Console.ReadKey();