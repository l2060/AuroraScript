using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;

namespace AuroraScript.Runtime
{
    public class NumberConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(double.NaN);
        public readonly static NumberValue MAX_VALUE = new NumberValue(double.MaxValue);
        public readonly static NumberValue MIN_VALUE = new NumberValue(double.MinValue);

        private new readonly static ScriptObject Prototype;



        public readonly static NumberConstructor INSTANCE = new NumberConstructor();


        static NumberConstructor()
        {
            Prototype = new ScriptObject();
            Prototype.Define("maxValue", MAX_VALUE, readable: true, writeable: false);
            Prototype.Define("minValue", MIN_VALUE, readable: true, writeable: false);
            Prototype.Define("NaN", NaN, readable: true, writeable: false);
            Prototype.Define("POSITIVE_INFINITY", POSITIVE_INFINITY, readable: true, writeable: false);
            Prototype.Define("NEGATIVE_INFINITY", NEGATIVE_INFINITY, readable: true, writeable: false);
            Prototype.Define("parse", new ClrFunction(PARSE), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }




        public NumberConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototype;
        }

        public static ScriptObject PARSE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var dobuleValue = double.Parse(args[0].ToString());
                return new NumberValue(dobuleValue);
            }
            return null;
        }


        public new static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var dobuleValue = double.Parse(args[0].ToString());
                return new NumberValue(dobuleValue);
            }
            return new NumberValue(0);
        }

    }
}
