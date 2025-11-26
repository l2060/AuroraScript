using AuroraScript.Runtime.Base;

namespace AuroraScript.Core
{
    public static class Datums
    {

        public static readonly ScriptDatum Object = ScriptDatum.FromString(new StringValue("object"));
        public static readonly ScriptDatum Array = ScriptDatum.FromString(new StringValue("array"));
        public static readonly ScriptDatum String = ScriptDatum.FromString(new StringValue("string"));
        public static readonly ScriptDatum Number = ScriptDatum.FromString(new StringValue("number"));
        public static readonly ScriptDatum Boolean = ScriptDatum.FromString(new StringValue("boolean"));
        public static readonly ScriptDatum Null = ScriptDatum.FromString(new StringValue("null"));
        public static readonly ScriptDatum Regex = ScriptDatum.FromString(new StringValue("regex"));
        public static readonly ScriptDatum Function = ScriptDatum.FromString(new StringValue("function"));
        public static readonly ScriptDatum ClrFunction = ScriptDatum.FromString(new StringValue("clr:function"));
        public static readonly ScriptDatum ClrBonding = ScriptDatum.FromString(new StringValue("clr:bonding"));
        public static readonly ScriptDatum ClrType = ScriptDatum.FromString(new StringValue("clr:type"));

    }


}