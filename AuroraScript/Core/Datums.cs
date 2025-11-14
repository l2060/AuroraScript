using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using AuroraScript.Compiler.Emits;
using AuroraScript.Compiler.Exceptions;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Core
{
    public static class Datums
    {
        public static readonly ScriptDatum Function = ScriptDatum.FromString(new StringValue("function"));
        public static readonly ScriptDatum Object = ScriptDatum.FromString(new StringValue("object"));
        public static readonly ScriptDatum Array = ScriptDatum.FromString(new StringValue("array"));
        public static readonly ScriptDatum String = ScriptDatum.FromString(new StringValue("string"));
        public static readonly ScriptDatum Number = ScriptDatum.FromString(new StringValue("number"));
        public static readonly ScriptDatum Boolean = ScriptDatum.FromString(new StringValue("boolean"));
        public static readonly ScriptDatum Null = ScriptDatum.FromString(new StringValue("null"));
        public static readonly ScriptDatum Clr = ScriptDatum.FromString(new StringValue("clr"));
    }


}