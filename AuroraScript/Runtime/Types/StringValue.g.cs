using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Pool;
using AuroraScript.Runtime.Types;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue
    {
        internal readonly static StringValue EMPTY = new StringValue("");
        internal readonly static StringValue NULL = new StringValue("null");
        internal readonly static StringValue TRUE = new StringValue("true");
        internal readonly static StringValue FALSE = new StringValue("false");
        internal readonly static StringValue OBJECT = new StringValue("[object]");



        internal static void TOLOWERCASE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue strValue)
            {
                result = ScriptDatum.FromString(strValue.Value.ToLowerInvariant());
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal static void TOUPPERCASE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue strValue)
            {
                result = ScriptDatum.FromString(strValue.Value.ToUpperInvariant());
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal new static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue strValue)
            {
                result = ScriptDatum.FromString(strValue);
            }
            else
            {
                result = ScriptDatum.FromString(thisObject.ToString());
            }
        }

        internal new static void LENGTH(ScriptObject thisObject, ref ScriptDatum result)
        {
            if (thisObject is StringValue strValue)
            {
                result = ScriptDatum.FromNumber(strValue.Value.Length);
            }
            else
            {
                result = ScriptDatum.FromNumber(0);
            }
        }

        internal static void CONTANINS(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                result = ScriptDatum.Null;
            }
            else
            {
                result = ScriptDatum.FromBoolean(str.Value.Contains(search));
            }

        }

        internal static void INDEXOF(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                result = ScriptDatum.FromNumber(-1);
            }
            else
            {
                result = ScriptDatum.FromNumber(str.Value.IndexOf(search, StringComparison.Ordinal));
            }
        }

        internal static void LASTINDEXOF(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                result = ScriptDatum.FromNumber(-1);
            }
            else
            {
                result = ScriptDatum.FromNumber(str.Value.LastIndexOf(search, StringComparison.Ordinal));
            }

        }

        internal static void STARTSWITH(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                result = ScriptDatum.FromBoolean(false);
            }
            else
            {
                result = ScriptDatum.FromBoolean(str.Value.StartsWith(search, StringComparison.Ordinal));
            }
        }

        internal static void ENDSWITH(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                result = ScriptDatum.FromBoolean(false);
            }
            else
            {
                result = ScriptDatum.FromBoolean(str.Value.EndsWith(search, StringComparison.Ordinal));
            }

        }

        internal static void SUBSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str)
            {
                result = ScriptDatum.Null;
                return;
            }

            var startDatum = GetNumberArg(args, 0);
            if (startDatum == null)
            {
                result = ScriptDatum.FromString(str);
                return;
            }

            var start = startDatum.Int32Value;
            if (args != null && args.Length > 1)
            {
                var endDatum = GetNumberArg(args, 1);
                if (endDatum != null)
                {
                    var end = endDatum.Int32Value;
                    if (start > end)
                    {
                        (start, end) = (end, start);
                    }
                    var length = Math.Clamp(end - start, 0, Math.Max(0, str.Value.Length - start));
                    result = ScriptDatum.FromString(str.Value.Substring(Math.Clamp(start, 0, str.Value.Length), length));
                    return;
                }
            }
            var safeStart = Math.Clamp(start, 0, Math.Max(0, str.Value.Length));
            result = ScriptDatum.FromString(str.Value.Substring(safeStart));
        }

        internal static void SPLIT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str)
            {
                result = ScriptDatum.Null;
                return;
            }

            var separator = GetStringArg(args, 0);
            if (separator == null)
            {
                result = ScriptDatum.FromArray(new ScriptArray(new ScriptObject[] { thisObject }));
                return;
            }

            var segments = str.Value.Split(new[] { separator }, StringSplitOptions.None)
                .Select(StringValue.Of)
                .Cast<ScriptObject>()
                .ToArray();
            result = ScriptDatum.FromArray(new ScriptArray(segments));
        }


        internal static void MATCH(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str)
            {
                result = ScriptDatum.Null;
                return;
            }

            var regex = ResolveRegexArgument(args, requireGlobal: false);
            if (regex == null)
            {
                result = ScriptDatum.Null;
                return;
            }

            result = ScriptDatum.FromObject(regex.HasFlag("g") ? regex.MatchOfGlobal(str) : regex.Match(str));
        }

        internal static void MATCHALL(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str)
            {
                result = ScriptDatum.Null;
                return;
            }

            var regex = ResolveRegexArgument(args, requireGlobal: true);
            result = ScriptDatum.FromObject(regex.MatchAll(str) ?? ScriptObject.Null);
        }


        internal static void REPLACE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                result = ScriptDatum.FromObject(thisObject);
                return;
            }

            var target = str.Value;
            if (args[0].Kind == ValueKind.String)
            {
                var search = GetStringArg(args, 0);
                var replace = GetStringArg(args, 1);
                if (search == null || replace == null)
                {
                    result = ScriptDatum.FromString(str);
                    return;
                }
                target = target.Replace(search, replace);
            }
            else if (args[0].Kind == ValueKind.Regex)
            {
                if (args[0].Object is not ScriptRegex regex)
                {
                    result = ScriptDatum.FromString(str);
                    return;
                }

                var input = target ?? String.Empty;
                var replaceAll = regex.HasFlag("g");

                if (args[1].Kind == ValueKind.String) // replace(/regex/, 'str')
                {
                    var replacement = GetStringArg(args, 1);
                    if (replacement == null)
                    {
                        result = ScriptDatum.FromString(str);
                        return;
                    }

                    target = regex.Replace(input, replacement, replaceAll);
                }
                else if (args[1].Kind == ValueKind.Function && args[1].Object is ClosureFunction callback) // replace(/regex/, (m,p1,p2,p3,offset,str)=>{ })
                {
                    var executeOptions = context.ExecuteOptions ?? ExecuteOptions.Default;
                    var originalValue = StringValue.Of(input);

                    target = regex.Replace(input, match =>
                    {
                        var groupCount = match.Groups.Count;
                        var callbackArgs = new ScriptDatum[groupCount + 2];

                        callbackArgs[0] = ScriptDatum.FromString(match.Value);
                        for (var i = 1; i < groupCount; i++)
                        {
                            callbackArgs[i] = match.Groups[i].Success
                                ? ScriptDatum.FromString(match.Groups[i].Value)
                                : ScriptDatum.Null;
                        }
                        callbackArgs[groupCount] = ScriptDatum.FromNumber(match.Index);
                        callbackArgs[groupCount + 1] = ScriptDatum.FromString(originalValue);
                        return InvokeReplaceCallback(callback, executeOptions, callbackArgs);
                    }, replaceAll);
                }
            }
            result = ScriptDatum.FromString(target);
        }

        internal static void PADLEFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                result = ScriptDatum.FromObject(thisObject);
                return;
            }

            var len = GetNumberArg(args, 0);
            var pad = GetStringArg(args, 1);
            if (len == null || pad == null || pad.Length == 0)
            {
                result = ScriptDatum.FromString(str);
                return;
            }
            result = ScriptDatum.FromString(str.Value.PadLeft(len.Int32Value, pad[0]));
        }

        internal static void PADRIGHT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                result = ScriptDatum.FromObject(thisObject);
                return;
            }

            var len = GetNumberArg(args, 0);
            var pad = GetStringArg(args, 1);
            if (len == null || pad == null || pad.Length == 0)
            {
                result = ScriptDatum.FromString(str);
                return;
            }
            result = ScriptDatum.FromString(str.Value.PadRight(len.Int32Value, pad[0]));
        }

        internal static void TRIM(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue str)
            {
                result = ScriptDatum.FromString(str.Value.Trim());
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal static void TRIMLEFT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue str)
            {
                result = ScriptDatum.FromString(str.Value.TrimStart());
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal static void TRIMRIGHT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringValue str)
            {
                result = ScriptDatum.FromString(str.Value.TrimEnd());
            }
            else
            {
                result = ScriptDatum.Null;
            }
        }

        internal static void CHARCODEAT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is not StringValue str)
            {
                result = ScriptDatum.FromNumber(Double.NaN);
                return;
            }

            var indexDatum = GetNumberArg(args, 0);
            if (indexDatum == null)
            {
                result = ScriptDatum.FromNumber(-1);
                return;
            }

            var index = indexDatum.Int32Value;
            if (index < 0 || index >= str.Value.Length)
            {
                result = ScriptDatum.FromNumber(Double.NaN);
                return;
            }
            result = ScriptDatum.FromNumber((Int32)str.Value[index]);
        }

        private static String InvokeReplaceCallback(ClosureFunction callback, ExecuteOptions executeOptions, ScriptDatum[] parameters)
        {
            executeOptions ??= ExecuteOptions.Default;
            var execContext = callback.Invoke(executeOptions, parameters);
            try
            {
                execContext.Done();
                if (execContext.Status == ExecuteStatus.Error)
                {
                    throw execContext.Error ?? new AuroraRuntimeException("The replace callback threw an error.");
                }

                var result = execContext.Result;
                if (result == null || result == ScriptObject.Null)
                {
                    return String.Empty;
                }

                if (result is StringValue stringResult)
                {
                    return stringResult.Value;
                }

                return result.ToString() ?? String.Empty;
            }
            finally
            {
                execContext.Dispose();
            }
        }

        private static String GetStringArg(Span<ScriptDatum> args, Int32 index)
        {
            if (args == null || index < 0 || index >= args.Length)
            {
                return null;
            }
            var datum = args[index];
            if (datum.Kind == ValueKind.String && datum.String != null)
            {
                return datum.String.Value;
            }
            return null;
        }

        private static NumberValue GetNumberArg(Span<ScriptDatum> args, Int32 index)
        {
            if (args == null || index < 0 || index >= args.Length)
            {
                return null;
            }

            var datum = args[index];
            if (datum.Kind == ValueKind.Number)
            {
                return NumberValue.Of(datum.Number);
            }
            if (datum.Kind == ValueKind.Boolean)
            {
                return NumberValue.Of(datum.Boolean ? 1 : 0);
            }
            return null;
        }

        private static ScriptRegex ResolveRegexArgument(Span<ScriptDatum> args, Boolean requireGlobal)
        {
            ScriptRegex regex = null;
            if (args != null && args.Length > 0)
            {
                var candidate = args[0];
                if (candidate.Kind == ValueKind.Regex && candidate.Object is ScriptRegex scriptRegex)
                {
                    if (requireGlobal && !scriptRegex.HasFlag("g"))
                    {
                        throw new AuroraRuntimeException("String.matchAll requires a global regular expression");
                    }
                    regex = scriptRegex;
                }
                else
                {
                    var pattern = CoercePatternFromDatum(candidate);
                    var flags = requireGlobal ? "g" : "";
                    regex = RegexManager.Resolve(pattern, flags);
                }
            }
            else
            {
                regex = RegexManager.Resolve("undefined", requireGlobal ? "g" : "");
            }

            return regex;
        }

        private static String CoercePatternFromDatum(ScriptDatum datum)
        {
            switch (datum.Kind)
            {
                case ValueKind.String when datum.String != null:
                    return datum.String.Value;
                case ValueKind.Number:
                    return datum.Number.ToString(System.Globalization.CultureInfo.InvariantCulture);
                case ValueKind.Boolean:
                    return datum.Boolean ? "true" : "false";
                default:
                    return String.Empty;
            }
        }
    }
}
