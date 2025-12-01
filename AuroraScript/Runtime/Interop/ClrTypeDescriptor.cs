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
        private readonly Lazy<ConstructorInfo[]> _constructors;

        private readonly ClrTypeRegistry _registry;
        private ClrTypeObject _typeObject;

        public string Alias { get; }

        public Type Type { get; }

        public ClrTypeOptions Options { get; }

        internal ClrTypeDescriptor(string alias, Type type, ClrTypeOptions options, ClrTypeRegistry registry)
        {
            Alias = alias;
            Type = type;
            Options = options;
            _registry = registry;
            _constructors = new Lazy<ConstructorInfo[]>(() => type.GetConstructors(options.Binding));
        }

        internal ClrTypeDescriptor(Type type)
        {
            Type = type;
            _constructors = new Lazy<ConstructorInfo[]>(() => []);
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

        public ConstructorInfo[] GetConstructors()
        {
            return _constructors.Value;
        }

        public ClrTypeObject GetOrCreateTypeObject()
        {
            return _typeObject ??= new ClrTypeObject(this);
        }

        private MethodBase[] ResolveMethods(string name)
        {
            var members = Type.GetMember(name, MemberTypes.Method, Options.Binding);
            return Array.ConvertAll(members, m => (MethodBase)m);
        }

        private FieldInfo ResolveField(string name)
        {
            return Type.GetField(name, Options.Binding);
        }

        private PropertyInfo ResolveProperty(string name)
        {
            return Type.GetProperty(name, Options.Binding);
        }

        private EventInfo ResolveEvent(string name)
        {
            return Type.GetEvent(name, Options.Binding);
        }
    }
}

