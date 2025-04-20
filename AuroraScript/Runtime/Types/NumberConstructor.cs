using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;

namespace AuroraScript.Runtime
{
    public class NumberConstructor : ClrFunction
    {
        public readonly static NumberValue POSITIVE_INFINITY = new NumberValue(double.PositiveInfinity);
        public readonly static NumberValue NEGATIVE_INFINITY = new NumberValue(double.NegativeInfinity);
        public readonly static NumberValue NaN = new NumberValue(double.NaN);
        public readonly static NumberValue MAX_VALUE = new NumberValue(double.MaxValue);
        public readonly static NumberValue MIN_VALUE = new NumberValue(double.MinValue);
        public readonly static NumberConstructor INSTANCE = new NumberConstructor();






        public NumberConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.NumberConstructorPrototype;
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                var dobuleValue = double.Parse(args[0].ToString());
                return new NumberValue(dobuleValue);
            }
            return null;
        }


        public static ScriptObject CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, ScriptObject[] args)
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
