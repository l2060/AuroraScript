using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptDeConstruct:ScriptObject
    {
        public ScriptDeConstruct(ScriptObject _object, ValueKind kind)
        {
            Object = _object;
            Kind = kind;
        }


        public readonly ValueKind Kind;

        public readonly ScriptObject Object;


    }
}
