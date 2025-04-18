using AuroraScript;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Threading.Tasks;


public class Program
{



    public static async Task Main()
    {
        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });

        await engine.BuildAsync("./unit.as");

        var g = engine.NewEnvironment();

        var domain = engine.CreateDomain(g);

        var result = domain.Execute("UNIT", "test").Done();

        if (result.Status == ExecuteStatus.Complete)
        {
            domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
        }

        domain.Execute("UNIT", "forTest").Done();

        var timerResult = domain.Execute("TIMER", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);
        timerResult.Done();

        if (timerResult.Status == ExecuteStatus.Complete)
        {
            domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
            domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
        }



        Console.ReadKey();
    }

}