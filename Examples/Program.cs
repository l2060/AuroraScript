using AuroraScript;
using AuroraScript.Exceptions;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Program
{


    





    public static async Task Main()
    {

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });

        await engine.BuildAsync("./unit.as");

        var g = engine.NewEnvironment();
        g.Define("PI", new NumberValue(Math.PI), readable: true, writeable: false, enumerable: false);

        engine.Global.Define("PI", g.GetPropertyValue("PI"));
        engine.Global.SetPropertyValue("PI", g.GetPropertyValue("PI"));
        var pi = engine.Global.GetPropertyValue("PI");

        var domain = engine.CreateDomain(g);
        try
        {
            // for in iterator test
            domain.Execute("UNIT_LIB", "testIterator").Done();


            // clouse test
            var clouse = domain.Execute("UNIT_LIB", "testClouse").Done();
            var clouseResult = domain.Execute(clouse.Result as ClosureFunction);
            Console.WriteLine($"testClouse result:{clouseResult.Result}");
            clouseResult = domain.Execute(clouse.Result as ClosureFunction);
            Console.WriteLine($"testClouse result:{clouseResult.Result}");


            var testInterruption = domain.Execute("UNIT_LIB", "testInterruption");

            if (testInterruption.Status  == ExecuteStatus.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(testInterruption.Error.ToString());
                Console.ResetColor();

                testInterruption.Continue().Done(AbnormalStrategy.Continue);
            }

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

            var forCount = 10000000;
            var testFor = domain.Execute("UNIT_LIB", "forTest", new NumberValue(forCount)).Done();

            Console.WriteLine($"for:{forCount} Use {testFor.UsedTime}ms");

            var timerResult = domain.Execute("TIMER_LIB", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);
            timerResult.Done();

            if (timerResult.Status == ExecuteStatus.Complete)
            {
                domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
                domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
            }
        }
        catch (AuroraRuntimeException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }

        Console.ReadKey();
    }

}