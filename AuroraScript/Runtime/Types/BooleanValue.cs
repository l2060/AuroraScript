using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Types
{
    public sealed class BooleanValue : ScriptValue
    {
        public readonly static BooleanValue True = new BooleanValue(true, 1, new StringValue("true"));
        public readonly static BooleanValue False = new BooleanValue(false, 0, new StringValue("false"));
        public readonly Int32 IntValue;
        public readonly bool Value;
        public readonly StringValue StrValue;

        private BooleanValue(bool val, Int32 intVal, StringValue valueString) : base(Prototypes.BooleanValuePrototype)
        {
            Value = val;
            IntValue = intVal;
            StrValue = valueString;
        }

        internal new static void TOSTRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is BooleanValue boolean)
            {
                result = ScriptDatum.FromString(boolean.StrValue);
            }
            else
            {
                result = ScriptDatum.FromString("false");
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return StrValue.Value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BooleanValue Of(bool value)
        {
            return value ? True : False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsTrue()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is BooleanValue bol)
            {
                return bol.Value == Value;
            }

            if (obj is NumberValue num)
            {
                return num.Int32Value == IntValue;
            }

            return false;
        }
    }
}
