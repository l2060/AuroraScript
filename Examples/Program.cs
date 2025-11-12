using AuroraScript;
using AuroraScript.Exceptions;
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
            var closure = domain.Execute("UNIT_LIB", "testClosure").Done();
            Console.WriteLine(closure);

            // for in iterator test

            // clouse test
            var clouse = domain.Execute("UNIT_LIB", "testClouse").Done();
            var clouseResult = domain.Execute(clouse.Result as ClosureFunction);
            Console.WriteLine($"testClouse result:{clouseResult.Result}");
            clouseResult = domain.Execute(clouse.Result as ClosureFunction);
            Console.WriteLine($"testClouse result:{clouseResult.Result}");
            //var testInterruption = domain.Execute("UNIT_LIB", "testInterruption");
            //if (testInterruption.Status  == ExecuteStatus.Error)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(testInterruption.Error.ToString());
            //    Console.ResetColor();
            //    testInterruption.Continue().Done(AbnormalStrategy.Continue);
            //}
            // script function test
            var result = domain.Execute("UNIT_LIB", "test").Done();
            var callmethod = domain.CreateDelegateFromMethod("UNIT_LIB", "testMD5");
            callmethod();
            if (result.Status == ExecuteStatus.Complete)
            {
                domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
            }

            var timerResult = domain.Execute("TIMER_LIB", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);
            timerResult.Done();
            if (timerResult.Status == ExecuteStatus.Complete)
            {
                domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
                domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
            }
            BenchmarkScript(domain, "MD5_LIB", "MD5", new StringValue("12345"));
            BenchmarkScript(domain, "MD5_LIB", "MD5", new StringValue("12345"));
            BenchmarkScript(domain, "UNIT_LIB", "testMD5");
            BenchmarkScript(domain, "UNIT_LIB", "testIterator");
            BenchmarkScript(domain, "UNIT_LIB", "testClosure");
            BenchmarkScript(domain, "UNIT_LIB", "test");
            BenchmarkScript(domain, "UNIT_LIB", "testClrFunc");
            BenchmarkScript(domain, "UNIT_LIB", "forTest", new NumberValue(10_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkNumbers", new NumberValue(2_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkArrays", new NumberValue(500_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkClosure", new NumberValue(1_000_000));

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

    private static void BenchmarkScript(ScriptDomain domain, string module, string method, params ScriptObject[] args)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var beforeAlloc = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        var context = domain.Execute(module, method, args).Done();
        stopwatch.Stop();
        var afterAlloc = GC.GetAllocatedBytesForCurrentThread();
        var allocatedBytes = afterAlloc - beforeAlloc;
        Console.WriteLine($"{module}.{method} -> status: {context.Status}, time: {stopwatch.ElapsedMilliseconds} ms, allocated: {allocatedBytes / 1024.0:F2} KB");
    }
}