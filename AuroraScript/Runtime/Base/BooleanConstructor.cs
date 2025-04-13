using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{

    public class BooleanConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(Double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(Double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(Double.NaN);


        private new readonly static ScriptObject Prototype;



        public readonly static BooleanConstructor INSTANCE = new BooleanConstructor();


        static BooleanConstructor()
        {
            Prototype = new ScriptObject();
            Prototype.Define("true", BooleanValue.True, readable: true, writeable: false);
            Prototype.Define("false", BooleanValue.False, readable: true, writeable: false);
            Prototype.Define("valueOf", new ClrFunction(PARSE), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }




        public BooleanConstructor() : base(CONSTRUCTOR)
        {
            _prototype = BooleanConstructor.Prototype;
        }

        public static ScriptObject PARSE(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = Boolean.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }


        public new static ScriptObject CONSTRUCTOR(AuroraEngine engine, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = Boolean.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }

    }
}
