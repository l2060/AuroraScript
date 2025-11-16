using AuroraScript;
using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


public class Program
{
    private static AuroraEngine engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });

    public class TestObject
    {
        public string Name { get; set; } = "*";

        public void Say(int n, string s)
        {
            //Console.WriteLine($"Say[{n}]: {s} ({Name})");
        }

        public static String Cat(String[] strings)
        {
            return $"Static Eat: [{String.Join(",", strings)}]";
        }
    }
    public static ScriptObject CREATE_TIMER(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
    {
        if (args == null || args.Length == 0 || args[0].Object is not ClosureFunction callback)
        {
            return ScriptObject.Null;
        }
        callback.InvokeFromClr(123, Array.Empty<object>(), thisObject);
        return ScriptObject.Null;
    }





    public static async Task Main()
    {
        engine.RegisterClrType<TestObject>();

        await engine.BuildAsync("./unit.as");

        var g = engine.NewEnvironment();
        g.Define("PI", new NumberValue(Math.PI), readable: true, writeable: false, enumerable: false);

        engine.Global.Define("PI", g.GetPropertyValue("PI"));
        engine.Global.SetPropertyValue("PI", g.GetPropertyValue("PI"));
        engine.Global.Define("CREATE_TIMER", new ClrFunction(CREATE_TIMER));
        var fo = new TestObject();
        engine.Global.SetPropertyValue("fo", fo);
        //engine.Global.SetPropertyValue("eat", new ClrFunction(TestObject.Eat));

        var domain = engine.CreateDomain(g);
        try
        {


            BenchmarkScript(domain, "TIMER_LIB", "testCallback");
            


            RunAndReportUnitTests(domain);

            var closure = domain.Execute("UNIT_LIB", "testClosure").Done();
            Console.WriteLine(closure);

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
            BenchmarkScript(domain, "UNIT_LIB", "testMD5_1000");
            BenchmarkScript(domain, "UNIT_LIB", "testIterator");
            BenchmarkScript(domain, "UNIT_LIB", "testClosure");
            BenchmarkScript(domain, "UNIT_LIB", "test");
            BenchmarkScript(domain, "UNIT_LIB", "testClrFunc");
            BenchmarkScript(domain, "UNIT_LIB", "testClrFunc");
            BenchmarkScript(domain, "UNIT_LIB", "forTest", new NumberValue(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkNumbers", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkArrays", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkClosure", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkObjects", NumberValue.Of(200_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkStrings", NumberValue.Of(100_000));
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
        using var context = domain.Execute(module, method, args);
        context.Done();
        stopwatch.Stop();
        var afterAlloc = GC.GetAllocatedBytesForCurrentThread();
        var allocatedBytes = afterAlloc - beforeAlloc;
        var elapsedMicroseconds = stopwatch.ElapsedTicks * 1_000.0 / Stopwatch.Frequency;
        WriteBenchmarkResult(module, method, context.Status, context.UsedTime, allocatedBytes / 1024.0);
    }

    private static void WriteBenchmarkResult(string module, string method, ExecuteStatus status, double elapsedMs, double allocatedKb)
    {
        var originalColor = Console.ForegroundColor;
        Console.Write($"{module,-12} | {method,-24} | ");
        Console.ForegroundColor = GetStatusColor(status);
        Console.Write($"{status,-12}");
        Console.ForegroundColor = originalColor;
        Console.WriteLine($" | {elapsedMs,10:F3} ms | {allocatedKb,10:F2} KB");
    }

    private static ConsoleColor GetStatusColor(ExecuteStatus status)
    {
        return status switch
        {
            ExecuteStatus.Complete => ConsoleColor.Green,
            ExecuteStatus.Interrupted => ConsoleColor.Yellow,
            ExecuteStatus.Error => ConsoleColor.Red,
            _ => ConsoleColor.Cyan
        };
    }

    private static void RunAndReportUnitTests(ScriptDomain domain)
    {
        using var context = domain.Execute("UNIT_LIB", "runAllUnitTests");
        context.Done();

        if (context.Status != ExecuteStatus.Complete)
        {
            Console.WriteLine($"Unit tests did not complete. Status: {context.Status}");
            if (context.Status == ExecuteStatus.Error && context.Error != null)
            {
                Console.WriteLine(context.Error.ToString());
            }
            return;
        }

        if (context.Result is not ScriptObject summary)
        {
            Console.WriteLine("Unit tests returned an unexpected result payload.");
            return;
        }

        var total = GetIntProperty(summary, "total");
        var passed = GetIntProperty(summary, "passed");
        var failed = GetIntProperty(summary, "failed");
        var success = GetBooleanProperty(summary, "success");

        Console.WriteLine($"Unit tests summary -> total: {total}, passed: {passed}, failed: {failed}");

        if (!success)
        {
            if (summary.GetPropertyValue("failedCases") is ScriptArray failedCases)
            {
                for (int i = 0; i < failedCases.Length; i++)
                {
                    if (failedCases.GetElement(i) is ScriptObject failedCase)
                    {
                        var name = GetStringProperty(failedCase, "name");
                        var checks = GetIntProperty(failedCase, "checks");
                        Console.WriteLine($"  ✖ {name} (checks: {checks})");

                        if (failedCase.GetPropertyValue("failures") is ScriptArray failures)
                        {
                            for (int j = 0; j < failures.Length; j++)
                            {
                                if (failures.GetElement(j) is ScriptObject failure)
                                {
                                    var message = GetStringProperty(failure, "message");
                                    var actual = failure.GetPropertyValue("actual");
                                    var expected = failure.GetPropertyValue("expected");
                                    Console.WriteLine($"      - {message}");
                                    Console.WriteLine($"        actual: {FormatScriptValue(actual)}");
                                    Console.WriteLine($"        expected: {FormatScriptValue(expected)}");
                                }
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("AuroraScript unit tests reported failures.");
        }
    }

    private static int GetIntProperty(ScriptObject obj, string propertyName)
    {
        var value = obj?.GetPropertyValue(propertyName);
        return value switch
        {
            NumberValue number => number.Int32Value,
            BooleanValue boolean => boolean.Value ? 1 : 0,
            StringValue str when int.TryParse(str.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ when value != null && int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var fallback) => fallback,
            _ => 0
        };
    }

    private static bool GetBooleanProperty(ScriptObject obj, string propertyName)
    {
        var value = obj?.GetPropertyValue(propertyName);
        return value switch
        {
            BooleanValue boolean => boolean.Value,
            NumberValue number => Math.Abs(number.DoubleValue) > double.Epsilon,
            StringValue str => !string.IsNullOrEmpty(str.Value) && !string.Equals(str.Value, "false", StringComparison.OrdinalIgnoreCase),
            null => false,
            _ => value.IsTrue()
        };
    }

    private static string GetStringProperty(ScriptObject obj, string propertyName)
    {
        var value = obj?.GetPropertyValue(propertyName);
        return value switch
        {
            StringValue str => str.Value,
            NumberValue number => number.DoubleValue.ToString(CultureInfo.InvariantCulture),
            BooleanValue boolean => boolean.Value.ToString(),
            null => string.Empty,
            _ => value.ToDisplayString()
        };
    }

    private static string FormatScriptValue(ScriptObject value)
    {
        return value switch
        {
            null => "null",
            ScriptObject obj when ReferenceEquals(obj, ScriptObject.Null) => "null",
            NumberValue number => number.DoubleValue.ToString(CultureInfo.InvariantCulture),
            BooleanValue boolean => boolean.Value.ToString(),
            StringValue str => str.Value,
            ScriptArray array => FormatArray(array),
            _ => value.ToDisplayString()
        };
    }

    private static string FormatArray(ScriptArray array)
    {
        if (array == null || array.Length == 0)
        {
            return "[]";
        }
        var parts = new string[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            parts[i] = FormatScriptValue(array.GetElement(i));
        }
        return "[" + string.Join(", ", parts) + "]";
    }
}