using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Extensions
{
    internal class JsonSupport : ScriptObject
    {

        public JsonSupport()
        {
            Define("parse", new BondingFunction(PARSE), writeable: false, enumerable: false);
            Define("stringify", new BondingFunction(STRINGIFY), writeable: false, enumerable: false);
        }

        public static ScriptObject PARSE(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args != null && args.Length > 0)
            {


            }
            return Null;
        }

        public static ScriptObject STRINGIFY(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            if (args != null && args.Length > 0)
            {


            }
            return Null;
        }
    }
}
