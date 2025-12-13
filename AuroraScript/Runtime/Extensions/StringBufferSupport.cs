using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Text;


namespace AuroraScript.Runtime.Extensions
{

    internal class StringBufferConstructor : BondingFunction
    {
        public static readonly ScriptObject StringBufferPrototype = new ScriptObject(Prototypes.ObjectPrototype);
        internal static StringBufferConstructor INSTANCE = new StringBufferConstructor();

        static StringBufferConstructor()
        {

            StringBufferPrototype.Define("toString", new BondingFunction(StringBuffer.TO_STRING), writeable: false, enumerable: false);
            StringBufferPrototype.Define("append", new BondingFunction(StringBuffer.APPEND), writeable: false, enumerable: false);
            StringBufferPrototype.Define("insert", new BondingFunction(StringBuffer.INSERT), writeable: false, enumerable: false);
            StringBufferPrototype.Define("appendLine", new BondingFunction(StringBuffer.APPEND_LINE), writeable: false, enumerable: false);
            StringBufferPrototype.Define("clear", new BondingFunction(StringBuffer.CLEAR), writeable: false, enumerable: false);


            StringBufferPrototype.Frozen();
        }





        internal StringBufferConstructor() : base(CONSTRUCTOR)
        {
            _prototype = Prototypes.ScriptObjectConstructorPrototype;
        }

        internal static void CONSTRUCTOR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (args.TryGetString(0, out var initialValue))
            {
                result = ScriptDatum.FromObject(new StringBuffer(initialValue));
            }
            else
            {
                result = ScriptDatum.FromObject(new StringBuffer());
            }
        }

    }








    public class StringBuffer : ScriptObject
    {
        private readonly StringBuilder _builder;

        public StringBuffer()
        {
            _prototype = StringBufferConstructor.StringBufferPrototype;
            _builder = new StringBuilder();
        }

        public StringBuffer(String initialValue)
        {
            _prototype = StringBufferConstructor.StringBufferPrototype;
            _builder = new StringBuilder(initialValue);
        }


        internal static void TO_STRING(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringBuffer builder)
            {
                result = ScriptDatum.FromString(builder._builder.ToString());
            }
        }

        internal static void APPEND(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringBuffer builder)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    builder._builder.Append(args[i].ToString());
                }
            }
        }
        internal static void INSERT(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringBuffer builder)
            {
                if (args.TryGetInteger(0, out var index) && args.TryGetString(1, out var str))
                {
                    builder._builder.Insert((int)index, str);
                }
            }
        }
        internal static void APPEND_LINE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringBuffer builder)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    builder._builder.Append(args[i].ToString());
                }
                builder._builder.AppendLine();
            }
        }
        internal static void CLEAR(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            if (thisObject is StringBuffer builder)
            {
                builder._builder.Clear();
            }
        }

    }
}
