using AuroraScript.Runtime.Base;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Types
{
    public class BooleanValue : ScriptValue
    {

        public readonly static BooleanValue True = new BooleanValue(true, new StringValue("true"));
        public readonly static BooleanValue False = new BooleanValue(false, new StringValue("false"));
        private readonly bool _value;
        private readonly StringValue _valueString;

        private BooleanValue(bool str, StringValue valueString) : base()
        {
            _value = str;
            _valueString = valueString;
            _prototype = Prototypes.BooleanValuePrototype;
        }

        public new static ScriptObject TOSTRING(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            var boolean = thisObject as BooleanValue;
            return boolean._valueString;
        }


        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }


        public override string ToString()
        {
            return _valueString.Value;
        }

        public override string ToDisplayString()
        {
            return _valueString.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BooleanValue Of(bool value)
        {
            return value ? True : False;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool IsTrue()
        {
            return _value;
        }
    }
}
