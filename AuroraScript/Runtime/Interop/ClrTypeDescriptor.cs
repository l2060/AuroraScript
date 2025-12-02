using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    /// <summary>
    /// 描述通过宿主注册的 CLR 类型，并维护成员元数据缓存。
    /// </summary>
    public sealed class ClrTypeDescriptor
    {
        private readonly ConcurrentDictionary<string, MethodBase[]> _methodCache = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, FieldInfo> _fieldCache = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, EventInfo> _eventCache = new(StringComparer.Ordinal);
        private readonly BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        public readonly Type Type;


        internal ClrTypeDescriptor(Type type)
        {
            Type = type;
        }


        /// <summary>
        /// 获取指定名称的方法集合，后续可用于生成动态桩。
        /// </summary>
        public MethodBase[] GetMethods(string name)
        {
            return _methodCache.GetOrAdd(name, ResolveMethods);
        }

        /// <summary>
        /// 获取字段信息。
        /// </summary>
        public FieldInfo GetField(string name)
        {
            return _fieldCache.GetOrAdd(name, ResolveField);
        }

        /// <summary>
        /// 获取属性信息。
        /// </summary>
        public PropertyInfo GetProperty(string name)
        {
            return _propertyCache.GetOrAdd(name, ResolveProperty);
        }

        public EventInfo GetEvent(string name)
        {
            return _eventCache.GetOrAdd(name, ResolveEvent);
        }





        private MethodBase[] ResolveMethods(string name)
        {
            var members = Type.GetMember(name, MemberTypes.Method, _bindingFlags);
            return Array.ConvertAll(members, m => (MethodBase)m);
        }

        private FieldInfo ResolveField(string name)
        {
            return Type.GetField(name, _bindingFlags);
        }

        private PropertyInfo ResolveProperty(string name)
        {
            return Type.GetProperty(name, _bindingFlags);
        }

        private EventInfo ResolveEvent(string name)
        {
            return Type.GetEvent(name, _bindingFlags);
        }
    }
}

