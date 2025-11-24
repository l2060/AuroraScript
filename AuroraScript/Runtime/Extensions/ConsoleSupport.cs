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

        public static ScriptObject LOG(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args != null && args.Length > 0)
            {
                var message = args.Select(arg => arg.ToObject()?.ToString() ?? "null");
                Console.WriteLine(string.Join(", ", message));
            }
            return Null;
        }

        public static ScriptObject TIME(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var label = GetLabel(args);
            if (label != null)
            {
                _times[label] = _stopwatch.ElapsedMilliseconds;
            }
            return Null;
        }

        public static ScriptObject TIMEEND(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var label = GetLabel(args);
            if (label != null && _times.TryGetValue(label, out var start))
            {
                var elapsed = _stopwatch.ElapsedMilliseconds - start;
                _times.Remove(label);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{label} Used {elapsed}ms");
                Console.ResetColor();
            }
            return Null;
        }

        private static String GetLabel(ScriptDatum[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            var datum = args[0];
            if (datum.Kind == ValueKind.String && datum.String != null)
            {
                return datum.String.Value;
            }

            var obj = datum.ToObject();
            return (obj as StringValue)?.Value ?? obj?.ToString();
        }
    }
}

