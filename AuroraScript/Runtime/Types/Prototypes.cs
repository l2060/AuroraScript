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



        public static void Proload()
        {

        }


        static Prototypes()
        {

            // ScriptObject
            ObjectPrototype.Define("toString", new ClrFunction(ScriptObject.TOSTRING), writeable: false, enumerable: false);
            ObjectPrototype.Define("constructor", ScriptObjectConstructor.INSTANCE, writeable: false, enumerable: false);
            ObjectPrototype.Define("length", new ClrGetter(ScriptObject.LENGTH), writeable: false, enumerable: false);
            ObjectPrototype.Frozen(); ;


            // BooleanConstructor
            BooleanConstructorPrototype.Define("true", BooleanValue.True, writeable: false, enumerable: false);
            BooleanConstructorPrototype.Define("false", BooleanValue.False, writeable: false, enumerable: false);
            BooleanConstructorPrototype.Define("valueOf", new ClrFunction(BooleanConstructor.PARSE), writeable: false, enumerable: false);
            BooleanConstructorPrototype.Frozen(); ;


            // BooleanValue
            BooleanValuePrototype.Define("constructor", BooleanConstructor.INSTANCE, writeable: false, enumerable: false);
            BooleanValuePrototype.Define("toString", new ClrFunction(BooleanValue.TOSTRING), writeable: false, enumerable: false);
            BooleanValuePrototype.Frozen(); ;


            // Callable
            CallablePrototype.Define("bind", new ClrFunction(Callable.BIND), writeable: false, enumerable: false);
            CallablePrototype.Frozen(); ;


            // NullValue
            NullValuePrototype.Define("toString", new ClrFunction(NullValue.TOSTRING), writeable: false, enumerable: false);
            NullValuePrototype.Frozen(); ;


            // NumberConstructor
            NumberConstructorPrototype.Define("maxValue", NumberConstructor.MAX_VALUE, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("minValue", NumberConstructor.MIN_VALUE, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("NaN", NumberConstructor.NaN, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("POSITIVE_INFINITY", NumberConstructor.POSITIVE_INFINITY, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("NEGATIVE_INFINITY", NumberConstructor.NEGATIVE_INFINITY, writeable: false, enumerable: false);
            NumberConstructorPrototype.Define("parse", new ClrFunction(NumberConstructor.PARSE), writeable: false, enumerable: false);
            NumberConstructorPrototype.Frozen(); ;


            // NumberValue
            NumberValuePrototype.Define("constructor", NumberConstructor.INSTANCE, writeable: false, enumerable: false);
            NumberValuePrototype.Define("toString", new ClrFunction(NumberValue.TOSTRING), writeable: false, enumerable: false);
            NumberValuePrototype.Frozen(); ;

            // ScriptArray
            ScriptArrayPrototype.Define("length", new ClrGetter(ScriptArray.LENGTH), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("push", new ClrFunction(ScriptArray.PUSH), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("pop", new ClrFunction(ScriptArray.POP), writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("constructor", ArrayConstructor.INSTANCE, writeable: false, enumerable: false);
            ScriptArrayPrototype.Define("slice", new ClrFunction(ScriptArray.SLICE), writeable: false, enumerable: false);
            ScriptArrayPrototype.Frozen(); ;


            // ArrayConstructor
            ArrayConstructorPrototype.Frozen(); ;


            // StringValue
            StringValuePrototype.Define("constructor", StringConstructor.INSTANCE, writeable: false, enumerable: false);
            StringValuePrototype.Define("length", new ClrGetter(StringValue.LENGTH), writeable: false, enumerable: false);
            StringValuePrototype.Define("contains", new ClrFunction(StringValue.CONTANINS), writeable: false, enumerable: false);
            StringValuePrototype.Define("indexOf", new ClrFunction(StringValue.INDEXOF), writeable: false, enumerable: false);
            StringValuePrototype.Define("lastIndexOf", new ClrFunction(StringValue.LASTINDEXOF), writeable: false, enumerable: false);
            StringValuePrototype.Define("startsWith", new ClrFunction(StringValue.STARTSWITH), writeable: false, enumerable: false);
            StringValuePrototype.Define("endsWith", new ClrFunction(StringValue.ENDSWITH), writeable: false, enumerable: false);
            StringValuePrototype.Define("substring", new ClrFunction(StringValue.SUBSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("split", new ClrFunction(StringValue.SPLIT), writeable: false, enumerable: false);
            StringValuePrototype.Define("match", new ClrFunction(StringValue.MATCH), writeable: false, enumerable: false);
            StringValuePrototype.Define("replace", new ClrFunction(StringValue.REPLACE), writeable: false, enumerable: false);
            StringValuePrototype.Define("padLeft", new ClrFunction(StringValue.PADLEFT), writeable: false, enumerable: false);
            StringValuePrototype.Define("padRight", new ClrFunction(StringValue.PADRIGHT), writeable: false, enumerable: false);
            StringValuePrototype.Define("trim", new ClrFunction(StringValue.TRIM), writeable: false, enumerable: false);
            StringValuePrototype.Define("trimLeft", new ClrFunction(StringValue.TRIMLEFT), writeable: false, enumerable: false);
            StringValuePrototype.Define("trimRight", new ClrFunction(StringValue.TRIMRIGHT), writeable: false, enumerable: false);
            StringValuePrototype.Define("slice", new ClrFunction(StringValue.SUBSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("toString", new ClrFunction(StringValue.TOSTRING), writeable: false, enumerable: false);
            StringValuePrototype.Define("charCodeAt", new ClrFunction(StringValue.CHARCODEAT), writeable: false, enumerable: false);
            StringValuePrototype.Define("toLowerCase", new ClrFunction(StringValue.TOLOWERCASE), writeable: false, enumerable: false);
            StringValuePrototype.Define("toUpperCase", new ClrFunction(StringValue.TOUPPERCASE), writeable: false, enumerable: false);
            StringValuePrototype.Frozen(); ;


            // StringConstructor
            StringConstructorPrototype.Define("fromCharCode", new ClrFunction(StringConstructor.FROMCHARCODE), writeable: false, enumerable: false);
            StringConstructorPrototype.Define("valueOf", new ClrFunction(StringConstructor.CONSTRUCTOR), writeable: false, enumerable: false);
            //Prototype.Define("fromCodePoint", new ClrFunction(PARSE),  writeable: false, enumerable: false);
            StringConstructorPrototype.Frozen(); ;
        }







    }
}
