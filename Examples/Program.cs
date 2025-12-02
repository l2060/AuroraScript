using AuroraScript;
using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Examples
{
    public class Program
    {
        private static AuroraEngine engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./tests/" });
        private static UserState userState = new UserState();
        private static ExecuteOptions executeOptions = ExecuteOptions.Default.WithUserState(userState);



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
            Console.WriteLine(context.UserState);
            if (args.Length == 0 || args[0].Object is not ClosureFunction callback)
            {
                return ScriptObject.Null;
            }
            callback.InvokeFromClr(context.ExecuteOptions, 123, Array.Empty<object>(), thisObject).Done();
            return ScriptObject.Null;
        }

        public static ScriptObject GIVE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            Console.WriteLine(context.UserState);
            Console.WriteLine($"GIVE {String.Join(" ", args)}");
            return ScriptObject.Null;
        }


        public static ScriptObject CLIENT_INPUT_NUMBER(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            Console.WriteLine(context.UserState);
            Console.WriteLine($"OPEN INPUT {String.Join(" ", args)}");
            var callback = args[3].Object as ClosureFunction;
            Task.Run(async () =>
            {
                // 模拟回调调用
                await Task.Delay(1000);
                callback.InvokeFromClr(123);
            });
            return ScriptObject.Null;
        }






        public static async Task Main()
        {
            Regex regex = new Regex("profile\\.json$", RegexOptions.IgnoreCase);

            engine.RegisterClrType<TestObject>();
            engine.RegisterClrType<UserState>();
            engine.RegisterClrType(typeof(Math));

            await engine.BuildAsync();

            try
            {
                Test();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.ReadKey();
        }

        private static void GlobalConfiguration(ScriptGlobal g)
        {
            g.Define("PI", new NumberValue(Math.PI), readable: true, writeable: false, enumerable: false);

            g.Define("GIVE", new BondingFunction(GIVE), false, true, false);
            g.Define("CREATE_TIMER", new BondingFunction(CREATE_TIMER));
            g.Define("INPUTNUMBER", new BondingFunction(CLIENT_INPUT_NUMBER), false, true, false);

            var fo = new TestObject();
            g.SetPropertyValue("fo", fo);
        }




        public static void Test()
        {
            var domain = engine.CreateDomain(GlobalConfiguration, userState);

            //var testInterruption = domain.Execute("UNIT_LIB", "testInterruption");
            //if (testInterruption.Status  == ExecuteStatus.Error)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(testInterruption.Error.ToString());
            //    Console.ResetColor();
            //    testInterruption.Continue().Done(AbnormalStrategy.Continue);
            //}
            // script function test
            //BenchmarkScript(domain, "MAIN", "main");

            //RunAndReportUnitTests(domain);
            BenchmarkScript(domain, "UNIT_LIB", "testClosure");
            BenchmarkScript(domain, "TIMER_LIB", "testCallback");
            BenchmarkScript(domain, "UNIT_LIB", "testInput");
            BenchmarkScript(domain, "UNIT_LIB", "testDatetime");
            BenchmarkScript(domain, "UNIT_LIB", "testDeConstruct");
            BenchmarkScript(domain, "UNIT_LIB", "testRegex");
            BenchmarkScript(domain, "UNIT_LIB", "testJson");
            BenchmarkScript(domain, "UNIT_LIB", "testClrType", new StringValue("PI"), new NumberValue(Math.PI));
            BenchmarkScript(domain, "UNIT_LIB", "testMD5");
            BenchmarkScript(domain, "UNIT_LIB", "testMD5_1000");
            BenchmarkScript(domain, "UNIT_LIB", "testIterator");
            BenchmarkScript(domain, "UNIT_LIB", "test");
            BenchmarkScript(domain, "UNIT_LIB", "testClrFunc");
            BenchmarkScript(domain, "UNIT_LIB", "testClrFunc");
            BenchmarkScript(domain, "UNIT_LIB", "testFor", new NumberValue(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkNumbers", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkArrays", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkClosure", NumberValue.Of(1_000_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkObjects", NumberValue.Of(200_000));
            BenchmarkScript(domain, "UNIT_LIB", "benchmarkStrings", NumberValue.Of(100_000));




        }

        private static void BenchmarkScript(ScriptDomain domain, string module, string method, params ScriptObject[] args)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var beforeAlloc = GC.GetAllocatedBytesForCurrentThread();
            var context = domain.Execute(module, method, executeOptions, args);
            if (context.Status == ExecuteStatus.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(context.Error.ToString());
                Console.ResetColor();
                context.Continue().Done(AbnormalStrategy.Continue);
            }
            // context.Done();
            var afterAlloc = GC.GetAllocatedBytesForCurrentThread();
            var allocatedBytes = afterAlloc - beforeAlloc;
            WriteBenchmarkResult(module, method, context.Status, context.UsedTime, allocatedBytes / 1024.0);


            //if (context.Status == ExecuteStatus.Complete)
            {
                context.Dispose();
            }
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
            using var context = domain.Execute("UNIT_LIB", "testAllUnits");
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

            var total = summary.GetPropertyValue("total");
            var passed = summary.GetPropertyValue("passed");
            var failed = summary.GetPropertyValue("failed");
            Console.WriteLine($"Unit tests summary -> total: {total}, passed: {passed}, failed: {failed}");
            var success = summary.GetPropertyValue("success") as BooleanValue;
            if (!success.Value)
            {
                if (summary.GetPropertyValue("failedCases") is ScriptArray failedCases)
                {
                    for (int i = 0; i < failedCases.Length; i++)
                    {
                        if (failedCases.Get(i).Object is ScriptObject failedCase)
                        {
                            var name = failedCase.GetPropertyValue("name");
                            var checks = failedCase.GetPropertyValue("checks");
                            ;
                            Console.WriteLine($"  ✖ {name} (checks: {checks})");

                            if (failedCase.GetPropertyValue("failures") is ScriptArray failures)
                            {
                                for (int j = 0; j < failures.Length; j++)
                                {
                                    if (failures.Get(j).Object is ScriptObject failure)
                                    {
                                        var message = failure.GetPropertyValue("message");
                                        var actual = failure.GetPropertyValue("actual");
                                        var expected = failure.GetPropertyValue("expected");
                                        Console.WriteLine($"      - {message}");
                                        Console.WriteLine($"        actual: {actual}");
                                        Console.WriteLine($"        expected: {expected}");
                                    }
                                }
                            }
                        }
                    }
                }

                throw new InvalidOperationException("AuroraScript unit tests reported failures.");
            }
        }

    }


}