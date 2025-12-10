using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;


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

        public ScriptObject GetPropertyValue(StringValue key)
        {
            return GetPropertyValue(key.Value);
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
                ScriptDatum datum = ScriptDatum.Null;
                getter.Invoke(this, ref datum);
                return datum.ToObject();
            }
            if (property is BondingFunction clrFunc)
            {
                return clrFunc.Bind(this);
            }
            return property;
        }



        internal ScriptObject _resolveProperty(String key)
        {
            if (_properties != null)
            {
                if (_properties.TryGetValue(key, out var value))
                {
                    if (!value.Readable) throw new AuroraVMException("Property disables write");
                    return value.Value;
                }
            }
            if (_prototype != null)
            {
                return _prototype._resolveProperty(key);
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
            SetPropertyValue(StringValue.Of(key), value);
        }


        public virtual void SetPropertyValue(StringValue key, ScriptObject value)
        {
            if (_properties == null) return;
            if (_isFrozen)
            {
                throw new AuroraVMException("You cannot modify this object");
            }
            if (!_properties.TryGetValue(key.Value, out var existValue))
            {
                existValue = new ObjectProperty();
                existValue.Key = key;
                existValue.Readable = true;
                existValue.Writable = true;
                existValue.Enumerable = true;
                _properties[key.Value] = existValue;
            }
            if (!existValue.Writable) throw new AuroraVMException("Property disables write");
            existValue.Value = value;
        }


        public Boolean DeletePropertyValue(StringValue key)
        {
            return DeletePropertyValue(key.Value);
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




        public void CopyPropertysFrom(ScriptObject scriptObject, Boolean force = false)
        {
            foreach (var item in scriptObject._properties)
            {
                if (!this._properties.ContainsKey(item.Key) || force)
                {
                    this._properties.Add(item.Key, item.Value.Clone());
                }
            }
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
            if (!_properties.TryGetValue(key, out var existValue))
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
            return "[object]";
        }




        public static StringValue operator +(ScriptObject a, ScriptObject b)
        {
            return new StringValue(a.ToString() + b.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Boolean IsTrue()
        {
            return true;
        }


        ItemIterator IEnumerator.GetIterator()
        {
            var result = new List<ScriptDatum>();
            var current = this;
            while (current != null)
            {
                if (current._properties != null)
                {
                    foreach (var item in current._properties)
                    {
                        if (item.Value.Enumerable)
                        {
                            result.Add(ScriptDatum.FromString(item.Value.Key));
                        }
                    }
                }
                current = current._prototype;
            }
            return new ItemIterator(result.ToArray());
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
