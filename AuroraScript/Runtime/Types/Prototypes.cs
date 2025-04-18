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
            ObjectPrototype.Define("toString", new ClrFunction(ScriptObject.TOSTRING), writeable: false);
            ObjectPrototype.Define("constructor", ScriptObjectConstructor.INSTANCE, writeable: false);
            ObjectPrototype.Define("length", new ClrGetter(ScriptObject.LENGTH), writeable: false);
            ObjectPrototype.IsFrozen = true;


            // BooleanConstructor
            BooleanConstructorPrototype.Define("true", BooleanValue.True, writeable: false);
            BooleanConstructorPrototype.Define("false", BooleanValue.False, writeable: false);
            BooleanConstructorPrototype.Define("valueOf", new ClrFunction(BooleanConstructor.PARSE), writeable: false);
            BooleanConstructorPrototype.IsFrozen = true;


            // BooleanValue
            BooleanValuePrototype.Define("constructor", BooleanConstructor.INSTANCE, writeable: false);
            BooleanValuePrototype.Define("toString", new ClrFunction(BooleanValue.TOSTRING), writeable: false);
            BooleanValuePrototype.IsFrozen = true;


            // Callable
            CallablePrototype.Define("bind", new ClrFunction(Callable.BIND), writeable: false);
            CallablePrototype.IsFrozen = true;


            // NullValue
            NullValuePrototype.Define("toString", new ClrFunction(NullValue.TOSTRING), writeable: false);
            NullValuePrototype.IsFrozen = true;


            // NumberConstructor
            NumberConstructorPrototype.Define("maxValue", NumberConstructor.MAX_VALUE, writeable: false);
            NumberConstructorPrototype.Define("minValue", NumberConstructor.MIN_VALUE, writeable: false);
            NumberConstructorPrototype.Define("NaN", NumberConstructor.NaN, writeable: false);
            NumberConstructorPrototype.Define("POSITIVE_INFINITY", NumberConstructor.POSITIVE_INFINITY, writeable: false);
            NumberConstructorPrototype.Define("NEGATIVE_INFINITY", NumberConstructor.NEGATIVE_INFINITY, writeable: false);
            NumberConstructorPrototype.Define("parse", new ClrFunction(NumberConstructor.PARSE), writeable: false);
            NumberConstructorPrototype.IsFrozen = true;


            // NumberValue
            NumberValuePrototype.Define("constructor", NumberConstructor.INSTANCE, writeable: false);
            NumberValuePrototype.Define("toString", new ClrFunction(NumberValue.TOSTRING), writeable: false);
            NumberValuePrototype.IsFrozen = true;

            // ScriptArray
            ScriptArrayPrototype.Define("length", new ClrGetter(ScriptArray.LENGTH), writeable: false);
            ScriptArrayPrototype.Define("push", new ClrFunction(ScriptArray.PUSH), writeable: false);
            ScriptArrayPrototype.Define("pop", new ClrFunction(ScriptArray.POP), writeable: false);
            ScriptArrayPrototype.Define("constructor", ArrayConstructor.INSTANCE, writeable: false);
            ScriptArrayPrototype.Define("slice", new ClrFunction(ScriptArray.SLICE), writeable: false);
            ScriptArrayPrototype.IsFrozen = true;


            // ArrayConstructor
            ArrayConstructorPrototype.IsFrozen = true;


            // StringValue
            StringValuePrototype.Define("constructor", StringConstructor.INSTANCE, writeable: false);
            StringValuePrototype.Define("length", new ClrGetter(StringValue.LENGTH), writeable: false);
            StringValuePrototype.Define("contains", new ClrFunction(StringValue.CONTANINS), writeable: false);
            StringValuePrototype.Define("indexOf", new ClrFunction(StringValue.INDEXOF), writeable: false);
            StringValuePrototype.Define("lastIndexOf", new ClrFunction(StringValue.LASTINDEXOF), writeable: false);
            StringValuePrototype.Define("startsWith", new ClrFunction(StringValue.STARTSWITH), writeable: false);
            StringValuePrototype.Define("endsWith", new ClrFunction(StringValue.ENDSWITH), writeable: false);
            StringValuePrototype.Define("substring", new ClrFunction(StringValue.SUBSTRING), writeable: false);
            StringValuePrototype.Define("split", new ClrFunction(StringValue.SPLIT), writeable: false);
            StringValuePrototype.Define("match", new ClrFunction(StringValue.MATCH), writeable: false);
            StringValuePrototype.Define("replace", new ClrFunction(StringValue.REPLACE), writeable: false);
            StringValuePrototype.Define("padLeft", new ClrFunction(StringValue.PADLEFT), writeable: false);
            StringValuePrototype.Define("padRight", new ClrFunction(StringValue.PADRIGHT), writeable: false);
            StringValuePrototype.Define("trim", new ClrFunction(StringValue.TRIM), writeable: false);
            StringValuePrototype.Define("trimLeft", new ClrFunction(StringValue.TRIMLEFT), writeable: false);
            StringValuePrototype.Define("trimRight", new ClrFunction(StringValue.TRIMRIGHT), writeable: false);
            StringValuePrototype.Define("slice", new ClrFunction(StringValue.SUBSTRING), writeable: false);
            StringValuePrototype.Define("toString", new ClrFunction(StringValue.TOSTRING), writeable: false);
            StringValuePrototype.Define("charCodeAt", new ClrFunction(StringValue.CHARCODEAT), writeable: false);
            StringValuePrototype.Define("toLowerCase", new ClrFunction(StringValue.TOLOWERCASE), writeable: false);
            StringValuePrototype.Define("toUpperCase", new ClrFunction(StringValue.TOUPPERCASE), writeable: false);
            StringValuePrototype.IsFrozen = true;


            // StringConstructor
            StringConstructorPrototype.Define("fromCharCode", new ClrFunction(StringConstructor.FROMCHARCODE), writeable: false);
            StringConstructorPrototype.Define("valueOf", new ClrFunction(StringConstructor.CONSTRUCTOR), writeable: false);
            //Prototype.Define("fromCodePoint", new ClrFunction(PARSE),  writeable: false);
            StringConstructorPrototype.IsFrozen = true;
        }







    }
}
