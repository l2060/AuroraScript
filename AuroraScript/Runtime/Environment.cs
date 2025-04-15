using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示变量环境，用于存储和查找变量
    /// </summary>
    internal class Environment: ScriptObject
    {
        /// <summary>
        /// 父环境
        /// </summary>
        private readonly Environment _parent;

        /// <summary>
        /// 环境所属对象
        /// </summary>
        public readonly ScriptObject This;


        /// <summary>
        /// 创建一个新的环境
        /// </summary>
        /// <param name="parent">父环境，可以为null表示全局环境</param>
        public Environment(Environment parent)
        {
            _parent = parent;
        }


        public override ScriptObject GetPropertyValue(String key, ScriptObject thisObject = null)
        {
            ScriptObject own = thisObject != null ? thisObject : this;
            _properties.TryGetValue(key, out var value);
            if (value != null)
            {
                if (!value.Readable) throw new Exception("Property disables write");

                if (value.Value is ClrGetter getter)
                {
                    return getter.Invoke(own);
                }
                return value.Value;
            }
            if (_prototype != null)
            {
                return _prototype.GetPropertyValue(key, own);
            }
            return Null;
        }

        public override void SetPropertyValue(String key, ScriptObject value)
        {
            if (IsFrozen)
            {
                throw new Exception("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Readable = true;
                existValue.Writeeable = true;
                _properties[key] = existValue;
            }
            if (!existValue.Writeeable) throw new Exception("Property disables write");
            existValue.Value = value;
        }

        public override void Define(String key, ScriptObject value, bool readable = true, bool writeable = true)
        {
            if (IsFrozen)
            {
                throw new Exception("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Readable = readable;
                existValue.Writeeable = writeable;
                _properties[key] = existValue;
            }
            else
            {
                if (!existValue.Writeeable) throw new Exception("Property disables write");
            }
            existValue.Value = value;
        }
    }
}
