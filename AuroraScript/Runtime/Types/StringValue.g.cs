using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public partial class StringValue
    {
        public readonly static StringValue Empty = new StringValue("");

        public static ScriptObject TOLOWERCASE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            return StringValue.Of(strValue.Value.ToLower());
        }

        public static ScriptObject TOUPPERCASE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            return StringValue.Of(strValue.Value.ToUpper());
        }

        public new static ScriptObject TOSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            return thisObject;
        }

        public new static ScriptObject LENGTH(ScriptObject thisObject)
        {
            var strValue = thisObject as StringValue;
            return NumberValue.Of(strValue.Value.Length);
        }


        public static ScriptObject CONTANINS(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.Contains(str.Value));
        }

        public static ScriptObject INDEXOF(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return NumberValue.Of(strValue.Value.IndexOf(str.Value));
        }

        public static ScriptObject LASTINDEXOF(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return NumberValue.Of(strValue.Value.LastIndexOf(str.Value));
        }




        public static ScriptObject STARTSWITH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.StartsWith(str.Value));
        }


        public static ScriptObject ENDSWITH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var str = args[0] as StringValue;
            return BooleanValue.Of(strValue.Value.EndsWith(str.Value));
        }


        public static ScriptObject SUBSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            var start = 0;
            var end = 0;
            if (args.Length > 0 && args[0] is NumberValue posNum)
            {
                start = posNum.Int32Value;
                if (args.Length > 1 && args[1] is NumberValue lenNum)
                {
                    end = lenNum.Int32Value;
                    if (start > end)
                    {
                        var e = end; end = start; start = e;
                    }
                    return StringValue.Of(strValue.Value.Substring(start, end - start));
                }
                return StringValue.Of(strValue.Value.Substring(start));
            }
            return thisObject;
        }

        public static ScriptObject SPLIT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            if (args.Length > 0 && args[0] is StringValue stringValue)
            {
                return new ScriptArray(strValue.Value.Split(stringValue.Value).Select(e => new StringValue(e)).ToArray());
            }
            return thisObject;
        }


        public static ScriptObject MATCH(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            // TODO
            return thisObject;
        }

        public static ScriptObject REPLACE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            if (args.Length == 2)
            {
                var str1 = args[0] as StringValue;
                var str2 = args[1] as StringValue;
                return StringValue.Of(strValue.Value.Replace(str1.Value, str2.Value));
            }
            return thisObject;
        }
        public static ScriptObject PADLEFT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            if (args.Length == 2)
            {
                var len = args[0] as NumberValue;
                var str2 = args[1] as StringValue;
                return StringValue.Of(strValue.Value.PadLeft(len.Int32Value, str2.Value[0]));
            }
            return thisObject;
        }
        public static ScriptObject PADRIGHT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            if (args.Length == 2)
            {
                var len = args[0] as NumberValue;
                var str2 = args[1] as StringValue;
                return StringValue.Of(strValue.Value.PadRight(len.Int32Value, str2.Value[0]));
            }
            return thisObject;
        }

        public static ScriptObject TRIM(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is StringValue str)
            {
                return StringValue.Of(str.Value.Trim());
            }
            return ScriptObject.Null;
        }

        public static ScriptObject TRIMLEFT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is StringValue str)
            {
                return StringValue.Of(str.Value.TrimStart());
            }
            return ScriptObject.Null;
        }

        public static ScriptObject TRIMRIGHT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (thisObject is StringValue str)
            {
                return StringValue.Of(str.Value.TrimEnd());
            }
            return ScriptObject.Null;
        }



        public static ScriptObject CHARCODEAT(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var strValue = thisObject as StringValue;
            if (args.Length > 0)
            {
                var num = args[0] as NumberValue;
                if (num.Int32Value >= strValue.Value.Length) return NumberValue.NaN;
                var charCode =  strValue.Value[num.Int32Value];
                return NumberValue.Of((Int32)charCode);
            }
            return NumberValue.Negative1;
        }




    }
}
