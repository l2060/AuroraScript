using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuroraScript.Runtime.Base
{

    public partial class ScriptObject
    {

        private Dictionary<String, ObjectProperty> _properties = new Dictionary<String, ObjectProperty>();

        internal ScriptObject _prototype;

        internal Boolean IsFrozen { get; set; } = false;

        internal ScriptObject(ScriptObject prototype)
        {
            _prototype = prototype;
        }

        public ScriptObject()
        {
            _prototype = Prototypes.ObjectPrototype;
        }


        public ScriptObject this[String key]
        {
            get
            {
                return GetPropertyValue(key);
            }
            set
            {
                SetPropertyValue(key, value);
            }
        }


        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="key">属性名</param>
        /// <param name="thisObject"></param>
        /// <returns></returns>
        public virtual ScriptObject GetPropertyValue(String key)
        {
            var property = _resolveProperty(key);
            if (property is ClrGetter getter)
            {
                return getter.Invoke(this);
            }
            if (property is ClrFunction clrFunc)
            {
                property = clrFunc.Bind(this);
                //if (!IsFrozen) SetPropertyValue(key, property);
            }
            return property;
        }



        private ScriptObject _resolveProperty(String key, ScriptObject thisObject = null)
        {
            ScriptObject own = thisObject != null ? thisObject : this;
            _properties.TryGetValue(key, out var value);
            if (value != null)
            {
                if (!value.Readable) throw new RuntimeException("Property disables write");
                return value.Value;
            }
            if (_prototype != null)
            {
                return _prototype._resolveProperty(key, own);
            }
            return Null;
        }

        /// <summary>
        /// 设置对象属性（可读、可写）
        /// </summary>
        /// <param name="key">属性名</param>
        /// <param name="value">属性值</param>
        /// <exception cref="RuntimeException"></exception>
        public virtual void SetPropertyValue(String key, ScriptObject value)
        {
            if (IsFrozen)
            {
                throw new RuntimeException("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Readable = true;
                existValue.Writeeable = true;
                _properties[key] = existValue;
            }
            if (!existValue.Writeeable) throw new RuntimeException("Property disables write");
            existValue.Value = value;
        }


        public virtual Boolean DeletePropertyValue(String key)
        {
            if (_properties.TryGetValue(key, out var value))
            {
                if (!value.Writeeable) throw new RuntimeException("Property disables write");
                _properties.Remove(key);
                return true;
            }
            if (_prototype != null)
            {
                return _prototype.DeletePropertyValue(key);
            }
            return false;
        }






        /// <summary>
        /// 定义对象属性的高级实现
        /// </summary>
        /// <param name="key">属性名</param>
        /// <param name="value">属性值</param>
        /// <param name="writeable">属性定义后是否可修改</param>
        /// <param name="readable">属性是否可读</param>
        /// <exception cref="RuntimeException"></exception>
        public virtual void Define(String key, ScriptObject value, bool writeable = true, bool readable = true)
        {
            if (IsFrozen)
            {
                throw new RuntimeException("You cannot modify this object");
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
                if (!existValue.Writeeable) throw new RuntimeException("Property disables write");
            }
            existValue.Value = value;
        }



        public override string ToString()
        {
            return "[object]";
        }




        public virtual string ToDisplayString()
        {
            if (_properties.Count == 0) return "";
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var pair in _properties)
            {
                sb.Append($"\"{pair.Key}\": ");
                sb.Append(pair.Value.Value.ToDisplayString());
                sb.Append(", ");
            }
            sb.Length -= 2;
            sb.Append("}");
            return sb.ToString();
        }


        public static StringValue operator +(ScriptObject a, ScriptObject b)
        {
            return new StringValue(a?.ToString() + b?.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Boolean IsTrue()
        {
            return true;
        }

    }




}
