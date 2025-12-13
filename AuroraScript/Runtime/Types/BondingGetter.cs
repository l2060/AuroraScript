using AuroraScript.Runtime.Base;

namespace AuroraScript.Runtime.Types
{

    public delegate void ClrGetterDelegate(ScriptObject @object, ref ScriptDatum result);

    /// <summary>
    /// 原型对象的clr粘合属性获取
    /// </summary>
    public class BondingGetter : ScriptObject
    {

        private readonly ClrGetterDelegate _callback;
        public readonly string Name;

        public BondingGetter(ClrGetterDelegate callback)
        {
            var method = callback.Method;
            Name = method.DeclaringType.Name + "." + method.Name;
            _callback = callback;
        }


        public void Invoke(ScriptObject @object, ref ScriptDatum result)
        {
            _callback.Invoke(@object, ref result);
        }


        public override string ToString()
        {
            return "ClrGetter: " + Name;
        }


    }
}
