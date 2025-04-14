using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AuroraScript.Runtime.Base
{
    public partial class NumberValue
    {

        private new readonly static ScriptObject Prototype;


        static NumberValue()
        {
            Prototype = new ScriptObject();
            Prototype.Define("constructor", new NumberConstructor(), readable: true, writeable: false);
            Prototype._prototype = ScriptObject.Prototype;
            Prototype.IsFrozen = true;
        }








    }
}
