using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    internal static class Prototypes
    {
        public static readonly ScriptObject ObjectPrototype = new ScriptObject(null);
        public static readonly ScriptObject ScriptObjectConstructorPrototype = new ScriptObject(ObjectPrototype);
        public static readonly ScriptObject BooleanConstructorPrototype = new ScriptObject(ObjectPrototype);
        public static readonly ScriptObject BooleanValuePrototype = new ScriptObject(ObjectPrototype);
        public static readonly ScriptObject CallablePrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject NullValuePrototype = new ScriptObject(null);
        public static readonly ScriptObject NumberConstructorPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject NumberValuePrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject ScriptArrayPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject ArrayConstructorPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject StringConstructorPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject StringValuePrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject RegexPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject DateConstructorPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        public static readonly ScriptObject DatePrototype = new ScriptObject(Prototypes.ObjectPrototype);




        internal static void Proload()
        {

        }


        static Prototypes()
        {

            // strict equal
            ScriptObjectConstructorPrototype.Define("equal$", new BondingFunction(ScriptObjectConstructor.STRICT_EQUAL), writeable: false, enumerable: false);
            // content equal  
            ScriptObjectConstructorPrototype.Define("equal", new BondingFunction(ScriptObjectConstructor.VALUE_EQUAL), writeable: false, enumerable: false);
            // deep content equal 
            ScriptObjectConstructorPrototype.Define("deepEqual", new BondingFunction(ScriptObjectConstructor.DEEP_EQUAL), writeable: false, enumerable: false);
            ScriptObjectConstructorPrototype.Define("assign", new BondingFunction(ScriptObjectConstructor.ASSIGN), writeable: false, enumerable: false);
            ScriptObjectConstructorPrototype.Define("keys", new BondingFunction(ScriptObjectConstructor.KEYS), writeable: false, enumerable: false);
            
            ScriptObjectConstructorPrototype.Define("clone", new BondingFunction(ScriptObjectConstructor.CLONE), writeable: false, enumerable: false);
            ScriptObjectConstructorPrototype.Define("deepClone", new BondingFunction(ScriptObjectConstructor.DEEP_CLONE), writeable: false, enumerable: false);

            ScriptObjectConstructorPrototype.Frozen();


            // ScriptObject
            ObjectPrototype.Define("toString", new BondingFunction(ScriptObject.TOSTRING), writeable: false, enumerable: false);

            ObjectPrototype.Define("constructor", ScriptObjectConstructor.INSTANCE, writeable: false, enumerable: false);
            ObjectPrototype.Define("length", new BondingGetter(ScriptObject.LENGTH), writeable: false, enumerable: false);
            ObjectPrototype.Frozen();


            // BooleanConstructor
            BooleanConstructorPrototype.Define("true", BooleanValue.True, writeable: false, enumerable: false);
            BooleanConstructorPrototype.Define("false", BooleanValue.False, writeable: false, enumerable: false);
            BooleanConstructorPrototype.Define("valueOf", new BondingFunction(BooleanConstructor.PARSE), writeable: false, enumerable: false);
            BooleanConstructorPrototype.Frozen();


            // BooleanValue
            BooleanValuePrototype.Define("constructor", BooleanConstructor.INSTANCE, writeable: false, enumerable: false);
            BooleanValuePrototype.Define("toString", new BondingFunction(BooleanValue.TOSTRING), writeable: false, enumerable: false);
            BooleanValuePrototype.Frozen();


            // regex
            RegexPrototype.Define("constructor", ScriptRegexConstructor.INSTANCE, writeable: false, enumerable: false);
            RegexPrototype.Define("test", new BondingFunction(ScriptRegex.TEST), writeable: false, enumerable: false);
            RegexPrototype.Frozen();


            //date 
            DateConstructorPrototype.Define("now", new BondingFunction(ScriptDateConstructor.NOW), writeable: false, enumerable: false);
            DateConstructorPrototype.Define("utcNow", new BondingFunction(ScriptDateConstructor.UTC_NOW), writeable: false, enumerable: false);
            DateConstructorPrototype.Define("parse", new BondingFunction(ScriptDateConstructor.PARSE), writeable: false, enumerable: false);
            DateConstructorPrototype.Frozen();

            DatePrototype.Define("constructor", ScriptDateConstructor.INSTANCE, writeable: false, enumerable: false);
            DatePrototype.Define("toString", new BondingFunction(ScriptDateConstructor.TOSTRING), writeable: false, enumerable: false);
            DatePrototype.Frozen();

            // Callable
            //CallablePrototype.Define("bind", new ClrFunction(Callable.BIND), writeable: false, enumerable: false);
            CallablePrototype.Frozen();


            // NullValue
            NullValuePrototype.Define("toString", new BondingFunction(NullValue.TOSTRING), writeable: false, enumerable: false);
            NullValuePrototype.Frozen();


            // NumberConstructor
            NumberConstructorPrototype.Define("MAX_VALUE", NumberConstructor.MAX_VALUE, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("MIN_VALUE", NumberConstructor.MIN_VALUE, writeable: false, enumerable: false);

            NumberConstructorPrototype.Define("MAX_SAFE_INTEGER", NumberConstructor.MAX_SAFE_INTEGER, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("MIN_SAFE_INTEGER", NumberConstructor.MIN_SAFE_INTEGER, writeable: false, enumerable: false);

            NumberConstructorPrototype.Define("NaN", NumberConstructor.NaN, writeable: false, enumerable: false);

            NumberConstructorPrototype.Define("POSITIVE_INFINITY", NumberConstructor.POSITIVE_INFINITY, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("NEGATIVE_INFINITY", NumberConstructor.NEGATIVE_INFINITY, writeable: false, enumerable: false);

            NumberConstructorPrototype.Define("isNaN", new BondingFunction(NumberConstructor.IS_NAN), writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("isInteger", new BondingFunction(NumberConstructor.IS_INTEGER), writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("isInfinity", new BondingFunction(NumberConstructor.IS_INFINITY), writeable: false, enumerable: false);

            NumberConstructorPrototype.Define("parseFloat", new BondingFunction(NumberConstructor.PARSE_FLOAT), writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("parseInt", new BondingFunction(NumberConstructor.PARSE_INTEGER), writeable: false, enumerable: false);

            NumberConstructorPrototype.Frozen();


            // NumberValue
            NumberValuePrototype.Define("constructor", NumberConstructor.INSTANCE, writeable: false, enumerable: false);
            NumberValuePrototype.Define("toString", new BondingFunction(NumberValue.TOSTRING), writeable: false, enumerable: false);
            NumberValuePrototype.Frozen();

            // ScriptArray
            ScriptArrayPrototype.Define("length", new BondingGetter(ScriptArray.LENGTH), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("push", new BondingFunction(ScriptArray.PUSH), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("pop", new BondingFunction(ScriptArray.POP), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("sort", new BondingFunction(ScriptArray.SORT), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("join", new BondingFunction(ScriptArray.JOIN), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("constructor", ArrayConstructor.INSTANCE, writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("slice", new BondingFunction(ScriptArray.SLICE), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("reverse", new BondingFunction(ScriptArray.REVERSE), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("unshift", new BondingFunction(ScriptArray.UNSHIFT), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("shift", new BondingFunction(ScriptArray.SHIFT), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("concat", new BondingFunction(ScriptArray.CONCAT), writeable: false, enumerable: false);
            ScriptArrayPrototype.Frozen();


            // ArrayConstructor
            ArrayConstructorPrototype.Define("from", new BondingFunction(ArrayConstructor.FROM), writeable: false, enumerable: false);
            ArrayConstructorPrototype.Define("isArray", new BondingFunction(ArrayConstructor.IS_ARRAY), writeable: false, enumerable: false);
            ArrayConstructorPrototype.Define("of", new BondingFunction(ArrayConstructor.OF), writeable: false, enumerable: false);
            ArrayConstructorPrototype.Frozen();

            // StringConstructor
            StringConstructorPrototype.Define("fromCharCode", new BondingFunction(StringConstructor.FROMCHARCODE), writeable: false, enumerable: false);
            StringConstructorPrototype.Define("valueOf", new BondingFunction(StringConstructor.CONSTRUCTOR), writeable: false, enumerable: false);
            StringConstructorPrototype.Frozen();


            // StringValue

            StringValuePrototype.Define("length", new BondingGetter(StringValue.LENGTH), writeable: false, enumerable: false);
            StringValuePrototype.Define("contains", new BondingFunction(StringValue.CONTANINS), writeable: false, enumerable: false);
            StringValuePrototype.Define("indexOf", new BondingFunction(StringValue.INDEXOF), writeable: false, enumerable: false);
            StringValuePrototype.Define("lastIndexOf", new BondingFunction(StringValue.LASTINDEXOF), writeable: false, enumerable: false);
            StringValuePrototype.Define("startsWith", new BondingFunction(StringValue.STARTSWITH), writeable: false, enumerable: false);
            StringValuePrototype.Define("endsWith", new BondingFunction(StringValue.ENDSWITH), writeable: false, enumerable: false);
            StringValuePrototype.Define("substring", new BondingFunction(StringValue.SUBSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("split", new BondingFunction(StringValue.SPLIT), writeable: false, enumerable: false);
            StringValuePrototype.Define("match", new BondingFunction(StringValue.MATCH), writeable: false, enumerable: false);
            StringValuePrototype.Define("matchAll", new BondingFunction(StringValue.MATCHALL), writeable: false, enumerable: false);
            StringValuePrototype.Define("replace", new BondingFunction(StringValue.REPLACE), writeable: false, enumerable: false);
            StringValuePrototype.Define("padLeft", new BondingFunction(StringValue.PADLEFT), writeable: false, enumerable: false);
            StringValuePrototype.Define("padRight", new BondingFunction(StringValue.PADRIGHT), writeable: false, enumerable: false);
            StringValuePrototype.Define("trim", new BondingFunction(StringValue.TRIM), writeable: false, enumerable: false);
            StringValuePrototype.Define("trimLeft", new BondingFunction(StringValue.TRIMLEFT), writeable: false, enumerable: false);
            StringValuePrototype.Define("trimRight", new BondingFunction(StringValue.TRIMRIGHT), writeable: false, enumerable: false);
            StringValuePrototype.Define("slice", new BondingFunction(StringValue.SUBSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("toString", new BondingFunction(StringValue.TOSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("charCodeAt", new BondingFunction(StringValue.CHARCODEAT), writeable: false, enumerable: false);
            StringValuePrototype.Define("toLowerCase", new BondingFunction(StringValue.TOLOWERCASE), writeable: false, enumerable: false);
            StringValuePrototype.Define("toUpperCase", new BondingFunction(StringValue.TOUPPERCASE), writeable: false, enumerable: false);
            StringValuePrototype.Frozen();



        }







    }
}
