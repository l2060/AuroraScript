using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Types;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue
    {
        public readonly static StringValue Empty = new StringValue("");

        public static ScriptObject TOLOWERCASE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject is StringValue strValue
                ? StringValue.Of(strValue.Value.ToLowerInvariant())
                : ScriptObject.Null;
        }

        public static ScriptObject TOUPPERCASE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject is StringValue strValue
                ? StringValue.Of(strValue.Value.ToUpperInvariant())
                : ScriptObject.Null;
        }

        public new static ScriptObject TOSTRING(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject;
        }

        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            return thisObject is StringValue strValue
                ? NumberValue.Of(strValue.Value.Length)
                : NumberValue.Zero;
        }

        public static ScriptObject CONTANINS(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                return ScriptObject.Null;
            }
            return BooleanValue.Of(str.Value.Contains(search));
        }

        public static ScriptObject INDEXOF(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                return NumberValue.Negative1;
            }
            return NumberValue.Of(str.Value.IndexOf(search, StringComparison.Ordinal));
        }

        public static ScriptObject LASTINDEXOF(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                return NumberValue.Negative1;
            }
            return NumberValue.Of(str.Value.LastIndexOf(search, StringComparison.Ordinal));
        }

        public static ScriptObject STARTSWITH(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                return BooleanValue.False;
            }
            return BooleanValue.Of(str.Value.StartsWith(search, StringComparison.Ordinal));
        }

        public static ScriptObject ENDSWITH(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var str = thisObject as StringValue;
            var search = GetStringArg(args, 0);
            if (str == null || search == null)
            {
                return BooleanValue.False;
            }
            return BooleanValue.Of(str.Value.EndsWith(search, StringComparison.Ordinal));
        }

        public static ScriptObject SUBSTRING(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str)
            {
                return ScriptObject.Null;
            }

            var startDatum = GetNumberArg(args, 0);
            if (startDatum == null)
            {
                return thisObject;
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
                    return StringValue.Of(str.Value.Substring(Math.Clamp(start, 0, str.Value.Length), length));
                }
            }

            var safeStart = Math.Clamp(start, 0, Math.Max(0, str.Value.Length));
            return StringValue.Of(str.Value.Substring(safeStart));
        }

        public static ScriptObject SPLIT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str)
            {
                return ScriptObject.Null;
            }

            var separator = GetStringArg(args, 0);
            if (separator == null)
            {
                return thisObject;
            }

            var segments = str.Value.Split(new[] { separator }, StringSplitOptions.None)
                .Select(StringValue.Of)
                .Cast<ScriptObject>()
                .ToArray();
            return new ScriptArray(segments);
        }


        public static ScriptObject MATCH(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str)
            {
                return ScriptObject.Null;
            }

            var regex = ResolveRegexArgument(args, requireGlobal: false);
            if (regex == null)
            {
                return ScriptObject.Null;
            }

            return regex.HasFlag("g")
                ? regex.MatchOfGlobal(str)
                : regex.Match(str);
        }

        public static ScriptObject MATCHALL(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str)
            {
                return ScriptObject.Null;
            }

            var regex = ResolveRegexArgument(args, requireGlobal: true);
            return regex?.MatchAll(str) ?? ScriptObject.Null;
        }


        public static ScriptObject REPLACE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                return thisObject;
            }

            var target = str.Value;
            if (args[0].Kind == ValueKind.String)
            {
                var search = GetStringArg(args, 0);
                var replace = GetStringArg(args, 1);
                if (search == null || replace == null)
                {
                    return thisObject;
                }
                target = target.Replace(search, replace);
            }
            else if (args[0].Kind == ValueKind.Regex)
            {
                if (args[0].Object is not ScriptRegex regex)
                {
                    return thisObject;
                }

                var input = target ?? String.Empty;
                var replaceAll = regex.HasFlag("g");

                if (args[1].Kind == ValueKind.String) // replace(/regex/, 'str')
                {
                    var replacement = GetStringArg(args, 1);
                    if (replacement == null)
                    {
                        return thisObject;
                    }

                    target = regex.Replace(input, replacement, replaceAll);
                }
                else if (args[1].Kind == ValueKind.Function && args[1].Object is ClosureFunction callback) // replace(/regex/, (m,p1,p2,p3,offset,str)=>{ })
                {
                    var executeOptions = context?.ExecuteOptions ?? ExecuteOptions.Default;
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
                                : ScriptDatum.FromNull();
                        }
                        callbackArgs[groupCount] = ScriptDatum.FromNumber(match.Index);
                        callbackArgs[groupCount + 1] = ScriptDatum.FromString(originalValue);
                        return InvokeReplaceCallback(callback, executeOptions, callbackArgs);
                    }, replaceAll);
                }
            }

            return StringValue.Of(target);
        }

        public static ScriptObject PADLEFT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                return thisObject;
            }

            var len = GetNumberArg(args, 0);
            var pad = GetStringArg(args, 1);
            if (len == null || pad == null || pad.Length == 0)
            {
                return thisObject;
            }

            return StringValue.Of(str.Value.PadLeft(len.Int32Value, pad[0]));
        }

        public static ScriptObject PADRIGHT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str || args == null || args.Length < 2)
            {
                return thisObject;
            }

            var len = GetNumberArg(args, 0);
            var pad = GetStringArg(args, 1);
            if (len == null || pad == null || pad.Length == 0)
            {
                return thisObject;
            }

            return StringValue.Of(str.Value.PadRight(len.Int32Value, pad[0]));
        }

        public static ScriptObject TRIM(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject is StringValue str
                ? StringValue.Of(str.Value.Trim())
                : ScriptObject.Null;
        }

        public static ScriptObject TRIMLEFT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject is StringValue str
                ? StringValue.Of(str.Value.TrimStart())
                : ScriptObject.Null;
        }

        public static ScriptObject TRIMRIGHT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            return thisObject is StringValue str
                ? StringValue.Of(str.Value.TrimEnd())
                : ScriptObject.Null;
        }

        public static ScriptObject CHARCODEAT(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (thisObject is not StringValue str)
            {
                return NumberValue.NaN;
            }

            var indexDatum = GetNumberArg(args, 0);
            if (indexDatum == null)
            {
                return NumberValue.Negative1;
            }

            var index = indexDatum.Int32Value;
            if (index < 0 || index >= str.Value.Length)
            {
                return NumberValue.NaN;
            }

            return NumberValue.Of((Int32)str.Value[index]);
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

        private static String GetStringArg(ScriptDatum[] args, Int32 index)
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

        private static NumberValue GetNumberArg(ScriptDatum[] args, Int32 index)
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

        private static ScriptRegex ResolveRegexArgument(ScriptDatum[] args, Boolean requireGlobal)
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
