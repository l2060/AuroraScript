using AuroraScript;
using AuroraScript.Core;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


public class Program
{



    public static async Task Main()
    {

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./temp/" });

        await engine.BuildAsync("./unit.as");

        var g = engine.NewEnvironment();

        var domain = engine.CreateDomain(g);

        var result = domain.Execute("UNIT_LIB", "test").Done();



        var a = domain.Execute(result.Result as ClosureFunction);

        a = domain.Execute(result.Result as ClosureFunction);











        if (result.Status == ExecuteStatus.Complete)
        {
            domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
        }




        //for (int i = 0; i < 10000; i++)
        //{
        var md5 = domain.Execute("MD5_LIB", "MD5", new StringValue("12345")).Done();
        //}





        domain.Execute("UNIT_LIB", "forTest").Done();

        var timerResult = domain.Execute("TIMER_LIB", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);
        timerResult.Done();

        if (timerResult.Status == ExecuteStatus.Complete)
        {
            domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
            domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
        }



        Console.ReadKey();
    }

}