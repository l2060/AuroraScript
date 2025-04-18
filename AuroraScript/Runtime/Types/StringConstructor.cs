using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Types
{
 
    public class StringConstructor : ClrFunction
    {

        public readonly static StringConstructor INSTANCE = new StringConstructor();


        public StringConstructor() : base(CONSTRUCTOR)
        {
            _prototype = new ScriptObject();
            _prototype.Define("fromCharCode", new ClrFunction(FROMCHARCODE), readable: true, writeable: false);
            _prototype.Define("valueOf", new ClrFunction(CONSTRUCTOR), readable: true, writeable: false);
            //Prototype.Define("fromCodePoint", new ClrFunction(PARSE), readable: true, writeable: false);
            _prototype._prototype = ScriptObject.Prototype;
            _prototype.IsFrozen = true;
        }


        public static ScriptObject FROMCHARCODE(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0 && args[0] is NumberValue numberValue)
            {
                Char _char = (Char)numberValue.Int64Value;
                return StringValue.Of(_char.ToString());
            }
            return StringValue.Empty;
        }




        public new static ScriptObject CONSTRUCTOR(ScriptDomain domain, ScriptObject thisObject, ScriptObject[] args)
        {
            if (args.Length > 0)
            {
                return StringValue.Of(args[0].ToString());
            }
            return StringValue.Empty;
        }
    }
}
