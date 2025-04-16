namespace AuroraScript.Runtime.Base
{
    public class BooleanValue : ScriptValue
    {

        public readonly static BooleanValue True = new BooleanValue(true, new StringValue("true"));
        public readonly static BooleanValue False = new BooleanValue(false, new StringValue("false"));
        private readonly Boolean _value;
        private readonly StringValue _valueString;

        private BooleanValue(Boolean str, StringValue valueString)
        {
            _value = str;
            _valueString = valueString;
            IsFrozen = true;
            this._prototype = new ScriptObject();
            this._prototype.Define("toString", new ClrFunction(TOSTRING), true, false);
            this._prototype.IsFrozen = true;
        }

        public new static ScriptObject TOSTRING(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
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


        public static BooleanValue Of(Boolean value)
        {
            return value ? True : False;
        }

        public override Boolean IsTrue()
        {
            return _value;
        }
    }
}
