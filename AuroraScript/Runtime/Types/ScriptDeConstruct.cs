using AuroraScript.Core;
using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{
    internal class ScriptDeConstruct : ScriptObject
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
