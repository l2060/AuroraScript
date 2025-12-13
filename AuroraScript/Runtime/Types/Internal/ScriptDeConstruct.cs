using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types.Internal
{
    internal class ScriptDeConstruct : ScriptObject
    {
        internal ScriptDeConstruct(ScriptObject _object, ValueKind kind)
        {
            Object = _object;
            Kind = kind;
        }


        internal readonly ValueKind Kind;

        internal readonly ScriptObject Object;


    }
}
