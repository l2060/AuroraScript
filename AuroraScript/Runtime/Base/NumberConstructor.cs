using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{
    public class NumberConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(Double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(Double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(Double.NaN);
        public readonly static NumberValue MAX_VALUE = new NumberValue(Double.MaxValue);
        public readonly static NumberValue MIN_VALUE = new NumberValue(Double.MinValue);

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
            _prototype = NumberConstructor.Prototype;
        }

        public static ScriptObject PARSE(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var dobuleValue = Double.Parse(args[0].ToString());
                return new NumberValue(dobuleValue);
            }
            return null;
        }


        public new static ScriptObject CONSTRUCTOR(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var dobuleValue = Double.Parse(args[0].ToString());
                return new NumberValue(dobuleValue);
            }
            return new NumberValue(0);
        }

    }
}
