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

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });

        await engine.BuildAsync("./unit.as");

        var g = engine.NewEnvironment();

        var domain = engine.CreateDomain(g);


        // for in iterator test
        domain.Execute("UNIT_LIB", "testIterator").Done();


        // clouse test
        var clouse = domain.Execute("UNIT_LIB", "testClouse").Done();
        var clouseResult = domain.Execute(clouse.Result as ClosureFunction);
        Console.WriteLine($"testClouse result:{clouseResult.Result}");
        clouseResult = domain.Execute(clouse.Result as ClosureFunction);
        Console.WriteLine($"testClouse result:{clouseResult.Result}");


        // clr function test
        var testClrFunc = domain.Execute("UNIT_LIB", "testClrFunc").Done();


        // script function test
        var testMD5 = domain.Execute("UNIT_LIB", "testMD5").Done();
        var result = domain.Execute("UNIT_LIB", "test").Done();



        if (result.Status == ExecuteStatus.Complete)
        {
            domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
        }

        var md5 = domain.Execute("MD5_LIB", "MD5", new StringValue("12345")).Done();

        var forCount = 100;
        var testFor = domain.Execute("UNIT_LIB", "forTest", new NumberValue(forCount)).Done();

        Console.WriteLine($"for:{forCount} Use {testFor.UsedTime}ms");

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