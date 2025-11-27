using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.IO;
using System.Text.RegularExpressions;


namespace AuroraScript.Runtime.Types
{
    public partial class ScriptRegex : ScriptObject
    {
        private readonly Regex _regex;

        private readonly String _flags;
        public ScriptRegex(Regex regex, String flags)
        {
            this._prototype = Prototypes.RegexPrototype;
            _regex = regex;
            _flags = flags;
        }




        public BooleanValue Test(ScriptDatum value)
        {
            if (value.Kind == ValueKind.String)
            {
                var result = _regex.Match(value.String.Value);
                return BooleanValue.Of(result.Success);
            }
            return BooleanValue.False;
        }


        public ScriptObject Match(StringValue str)
        {
            if (str == null)
            {
                return ScriptObject.Null;
            }

            var input = str.Value ?? String.Empty;
            var match = _regex.Match(input);
            if (!match.Success)
            {
                return ScriptObject.Null;
            }

            return CreateMatchResult(match, input);
        }

        public ScriptObject MatchOfGlobal(StringValue str)
        {
            if (str == null)
            {
                return ScriptObject.Null;
            }
            var input = str.Value ?? String.Empty;
            var matches = _regex.Matches(input ?? String.Empty);
            if (matches.Count == 0)
            {
                return ScriptObject.Null;
            }
            var result = new ScriptArray(matches.Count);
            for (int i = 0; i < matches.Count; i++)
            {
                result.SetDatum(i, ScriptDatum.FromString(StringValue.Of(matches[i].Value)));
            }
            return result;
        }

        public ScriptObject MatchAll(StringValue str)
        {
            if (str == null)
            {
                return ScriptObject.Null;
            }

            var matches = _regex.Matches(str.Value ?? String.Empty);
            if (matches.Count == 0)
            {
                return ScriptObject.Null;
            }

            var outer = new ScriptArray(matches.Count);
            for (int i = 0; i < matches.Count; i++)
            {
                var matchResult = CreateMatchResult(matches[i], str.Value ?? String.Empty);
                outer.SetDatum(i, ScriptDatum.FromObject(matchResult));
            }
            return outer;
        }



        public Boolean HasFlag(String flag)
        {
            return _flags.IndexOf(flag) > -1;
        }

        internal String Replace(String input, String replacement, Boolean replaceAll)
        {
            input ??= String.Empty;
            replacement ??= String.Empty;
            var count = replaceAll ? Int32.MaxValue : 1;
            return _regex.Replace(input, replacement, count);
        }

        internal String Replace(String input, MatchEvaluator evaluator, Boolean replaceAll)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            input ??= String.Empty;
            var count = replaceAll ? Int32.MaxValue : 1;
            return _regex.Replace(input, evaluator, count);
        }


        public static ScriptObject TEST(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args != null && args.Length >= 1)
            {
                var regex = thisObject as ScriptRegex;
                return regex.Test(args[0]);
            }
            return BooleanValue.False;
        }


        public override string ToString()
        {
            return _regex.ToString();
        }

        public override string ToDisplayString()
        {
            return _regex.ToString();
        }

        private ScriptArray CreateMatchResult(Match match, String input)
        {
            var groupCount = match.Groups.Count;
            var result = new ScriptArray(groupCount);
            for (int i = 0; i < groupCount; i++)
            {
                result.SetDatum(i, ScriptDatum.FromString(StringValue.Of(match.Groups[i].Value)));
            }

            result.SetPropertyValue("index", NumberValue.Of(match.Index));
            result.SetPropertyValue("input", StringValue.Of(input ?? String.Empty));

            var groupNames = _regex.GetGroupNames();
            ScriptObject namedGroups = null;
            for (int i = 0; i < groupNames.Length; i++)
            {
                var name = groupNames[i];
                if (String.IsNullOrEmpty(name) || Int32.TryParse(name, out _))
                {
                    continue;
                }

                namedGroups ??= new ScriptObject();
                var capture = match.Groups[name];
                if (capture.Success)
                {
                    namedGroups.SetPropertyValue(name, StringValue.Of(capture.Value));
                }
                else
                {
                    namedGroups.SetPropertyValue(name, ScriptObject.Null);
                }
            }

            if (namedGroups != null)
            {
                result.SetPropertyValue("groups", namedGroups);
            }

            return result;
        }
    }
}
