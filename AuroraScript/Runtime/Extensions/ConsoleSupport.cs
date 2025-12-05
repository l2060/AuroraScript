using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AuroraScript.Runtime.Extensions
{
    internal class ConsoleSupport : ScriptObject
    {
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private static readonly Dictionary<string, long> _times = new();

        public ConsoleSupport()
        {
            Define("log", new BondingFunction(LOG), writeable: false, enumerable: false);
            Define("time", new BondingFunction(TIME), writeable: false, enumerable: false);
            Define("timeEnd", new BondingFunction(TIMEEND), writeable: false, enumerable: false);
        }

        public static ScriptObject LOG(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    Console.Write(DatumToString(arg) ?? "null");
                    Console.Write(", ");
                }
                Console.WriteLine();
            }
            return Null;
        }




        private static String DatumToString(ScriptDatum datum)
        {
            if (datum.Kind.Include(ValueKind.Object))
            {
                return JsonSupport.Serialize(datum, false).Value;
            }
            return datum.ToString();
        }


        public static ScriptObject TIME(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.TryGetString(0, out var label))
            {
                _times[label] = _stopwatch.ElapsedMilliseconds;
            }
            return Null;
        }

        public static ScriptObject TIMEEND(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            if (args.TryGetString(0, out var label) && _times.TryGetValue(label, out var start))
            {
                var elapsed = _stopwatch.ElapsedMilliseconds - start;
                _times.Remove(label);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{label} Used {elapsed}ms");
                Console.ResetColor();
            }
            return Null;
        }
    }
}

