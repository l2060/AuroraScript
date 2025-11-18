using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;

namespace AuroraScript.Runtime.Types
{

    /// <summary>
    /// 原型对象的Clr粘合函数
    /// </summary>
    public class BondingFunction : Callable
    {
        public ScriptObject Target;

        public BondingFunction(ClrDatumDelegate callback) : base(callback)
        {

        }

        public override ScriptObject Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            var target = (thisObject == null) ? Target : thisObject;
            return DatumMethod(context, target, args);
        }

        public BondingFunction Bind(ScriptObject target)
        {
            var bind = new BondingFunction(DatumMethod);
            // 传递原型链
            bind._prototype = this._prototype;
            bind.Target = target;
            return bind;
        }

        public override string ToString()
        {
            return "ClrFunction: " + Name;
        }
    }
}
