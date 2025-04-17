// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using System;
using System.Threading.Tasks;


public class Program
{

    public static async Task Main()
    {

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });
        //try
        //{
        await engine.BuildAsync("./unit.as");

        var domain = engine.CreateDomain();



        var result = domain.Execute("UNIT", "test");
        if (result.Status == ExecuteStatus.Complete)
        {
            domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
        }

        domain.Execute("UNIT", "forTest").Done();

        var timerResult = domain.Execute("TIMER", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);
        if (timerResult.Status == ExecuteStatus.Complete)
        {
            domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
            domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
        }




        Console.WriteLine("=====================================================================================");
        Console.ReadKey();
    }

}