using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{

    public class BooleanConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(double.NaN);


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
            _prototype = Prototype;
        }

        public static ScriptObject PARSE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = bool.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }


        public new static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var booleanValue = bool.Parse(args[0].ToString());
                return booleanValue ? BooleanValue.True : BooleanValue.False;
            }
            return BooleanValue.False;
        }

    }
}
