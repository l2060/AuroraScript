// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;


public class Program
{

    public static async Task Main()
    {

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });
        //try
        //{
        await engine.BuildAsync("./unit.as");

        var domain = engine.CreateDomain();

        domain.Execute("UNIT", "forTest");


        var var1 = domain.Execute("UNIT", "test");
        domain.Execute(var1.GetPropertyValue("start") as ClosureFunction);


        var timer = domain.Execute("TIMER", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);


        domain.Execute(timer.GetPropertyValue("reset") as ClosureFunction);

        domain.Execute(timer.GetPropertyValue("cancel") as ClosureFunction);


        Console.WriteLine("=====================================================================================");
        Console.ReadKey();
    }

}