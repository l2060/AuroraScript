using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;


namespace AuroraScript.Runtime.Base
{

    public partial class ScriptObject : IEnumerator
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private Dictionary<String, ObjectProperty> _properties;

        internal ScriptObject _prototype;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _isFrozen = false;

        internal ScriptObject(ScriptObject prototype, Boolean initProperties = true)
        {
            _prototype = prototype;
            if (initProperties)
            {
                _properties = new Dictionary<string, ObjectProperty>();
            }
        }

        public ScriptObject()
        {
            _prototype = Prototypes.ObjectPrototype;
            _properties = new Dictionary<string, ObjectProperty>();
        }

        [DebuggerHidden] // JUMP BREAK CALLER
        public void Frozen()
        {
            _isFrozen = true;
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
            if (property is BondingGetter getter)
            {
                return getter.Invoke(this);
            }
            if (property is BondingFunction clrFunc)
            {
                property = clrFunc.Bind(this);
                //if (!IsFrozen) SetPropertyValue(key, property);
            }
            return property;
        }



        private ScriptObject _resolveProperty(String key, ScriptObject thisObject = null)
        {
            ScriptObject own = thisObject != null ? thisObject : this;
            if (_properties != null)
            {
                _properties.TryGetValue(key, out var value);
                if (value != null)
                {
                    if (!value.Readable) throw new AuroraVMException("Property disables write");
                    return value.Value;
                }
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
        /// <exception cref="AuroraRuntimeException"></exception>
        public virtual void SetPropertyValue(String key, ScriptObject value)
        {
            if (_properties == null) return;
            if (_isFrozen)
            {
                throw new AuroraVMException("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Key = StringValue.Of(key);
                existValue.Readable = true;
                existValue.Writable = true;
                existValue.Enumerable = true;
                _properties[key] = existValue;
            }
            if (!existValue.Writable) throw new AuroraVMException("Property disables write");
            existValue.Value = value;
        }


        public virtual Boolean DeletePropertyValue(String key)
        {
            if (_properties != null && _properties.TryGetValue(key, out var value))
            {
                if (!value.Writable) throw new AuroraVMException("Property disables write");
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
        /// <param name="enumerable">属性是否可枚举</param>
        /// <exception cref="AuroraRuntimeException"></exception>
        public virtual void Define(String key, ScriptObject value, bool writeable = true, bool readable = true, bool enumerable = true)
        {
            if (_properties == null) return;
            if (_isFrozen)
            {
                throw new AuroraVMException("You cannot modify this object");
            }
            _properties.TryGetValue(key, out var existValue);
            if (existValue == null)
            {
                existValue = new ObjectProperty();
                existValue.Key = StringValue.Of(key);
                existValue.Readable = readable;
                existValue.Writable = writeable;
                existValue.Enumerable = enumerable;
                _properties[key] = existValue;
            }
            else
            {
                if (!existValue.Writable) throw new AuroraVMException("Property disables write");
            }
            existValue.Value = value;
        }



        public override string ToString()
        {
            if (_properties == null || _properties.Count == 0) return "[object]";
            return ToDisplayString();
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


        ItemIterator IEnumerator.GetIterator()
        {
            var result = new List<ScriptObject>();
            var current = this;
            while (current != null)
            {
                if (current._properties != null)
                {
                    foreach (var item in current._properties)
                    {
                        if (item.Value.Enumerable)
                        {
                            result.Add(item.Value.Key);
                        }
                    }
                }
                current = current._prototype;
            }
            return ItemIterator.FromObjects(result);
        }



        public ScriptArray GetKeys()
        {
            var result = new List<ScriptObject>();
            var current = this;
            while (current != null)
            {
                if (current._properties != null)
                {
                    foreach (var item in current._properties)
                    {
                        if (item.Value.Enumerable)
                        {
                            result.Add(item.Value.Key);
                        }
                    }
                }
                current = current._prototype;
            }
            return new ScriptArray(result);
        }
    }




}
