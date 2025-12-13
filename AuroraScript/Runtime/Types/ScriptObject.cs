using AuroraScript.Exceptions;
using AuroraScript.Runtime.Types;
using AuroraScript.Runtime.Types.Internal;
using AuroraScript.Runtime.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace AuroraScript.Runtime.Base
{

    public partial class ScriptObject : IEnumerator
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal Dictionary<String, ObjectProperty> _properties;

        internal ScriptObject _prototype;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _isFrozen = false;

        private Boolean _isValueType = false;

        internal ScriptObject(ScriptObject prototype, Boolean isValueType = false)
        {
            _prototype = prototype;
            if (!isValueType)
            {
                _properties = new Dictionary<string, ObjectProperty>();
            }
            _isValueType = isValueType;
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


        internal ScriptObject GetPropertyValue(StringValue key)
        {
            return GetPropertyValue(key.Value);
        }
        internal void SetPropertyValue(StringValue key, ScriptObject value)
        {
            SetPropertyValue(key.Value, value);
        }


        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="key">属性名</param>
        /// <param name="thisObject"></param>
        /// <returns></returns>
        public virtual ScriptObject GetPropertyValue(String key)
        {
            return GetPropertyInternal(key);
        }

        public virtual void SetPropertyValue(String key, ScriptObject value)
        {
            InternalDefine(key, value);
        }

        internal Boolean DeletePropertyValue(StringValue key)
        {
            return DeletePropertyValue(key.Value);
        }

        private ScriptObject GetPropertyInternal(String key)
        {
            var property = _resolvePropertyValue(key);
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


        public virtual Boolean DeletePropertyValue(String key)
        {
            if (_properties != null && _properties.TryGetValue(key, out var value))
            {
                if (!value.Writable) ThrowDisableWritable();
                _properties.Remove(key);
                return true;
            }
            if (_prototype != null)
            {
                return _prototype.DeletePropertyValue(key);
            }
            return false;
        }


        internal ScriptObject _resolvePropertyValue(String key)
        {
            if (_properties != null && _properties.TryGetValue(key, out var value))
            {
                return value.Value;
            }
            if (_prototype != null)
            {
                return _prototype._resolvePropertyValue(key);
            }
            return Null;
        }

        internal ObjectProperty _resolveProperty(String key)
        {
            if (_properties != null && _properties.TryGetValue(key, out var value))
            {
                return value;
            }
            if (_prototype != null)
            {
                return _prototype._resolveProperty(key);
            }
            return null;
        }




        public void CopyPropertysFrom(ScriptObject scriptObject, Boolean force = false)
        {
            RuntimeHelper.CopyProperties(scriptObject, this, force);
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
        public virtual void Define(String key, ScriptObject value, bool writeable = true, bool enumerable = true)
        {
            InternalDefine(key, value, writeable, enumerable);
        }


        private void InternalDefine(String key, ScriptObject value, bool writeable = true, bool enumerable = true)
        {
            //if (_properties == null) return;
            if (_isValueType) return;
            if (_isFrozen) ThrowFrozen();
            if (!_properties.TryGetValue(key, out var existValue))
            {
                existValue = new ObjectProperty(key, writeable, enumerable);
                _properties[key] = existValue;
            }
            else
            {
                if (!existValue.Writable) ThrowDisableWritable();
            }
            existValue.Value = value;
        }

        protected void ThrowFrozen()
        {
            throw new AuroraVMException("You cannot modify this object");
        }
        protected void ThrowDisableWritable()
        {
            throw new AuroraVMException("Property disables write");
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
                if (!current._isValueType)
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
                if (!current._isValueType)
                {
                    foreach (var item in current._properties)
                    {
                        if (item.Value.Enumerable)
                        {
                            result.Add(StringValue.Of(item.Value.Key));
                        }
                    }
                }
                current = current._prototype;
            }
            return new ScriptArray(result);
        }




        public List<string> EnumerationKeys()
        {
            var list = new List<string>();
            if (_properties != null)
            {
                foreach (var item in _properties)
                {
                    if (item.Value.Enumerable)
                    {
                        list.Add(item.Key);
                    }
                }
            }
            if (_prototype != null)
            {
                var result = _prototype.EnumerationKeys();
                if (result.Count > 0) list.AddRange(result);
            }
            return list;
        }




    }




}
