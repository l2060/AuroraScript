using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Linq;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrInstanceObject : ScriptObject
    {
        private readonly ClrTypeDescriptor _descriptor;
        private readonly ClrTypeRegistry _registry;

        public object Instance { get; }

        internal ClrTypeDescriptor Descriptor => _descriptor;

        public ClrInstanceObject(ClrTypeDescriptor descriptor, object instance, ClrTypeRegistry registry)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _registry = registry;
            Instance = instance;
        }

        public override ScriptObject GetPropertyValue(string key)
        {
            var property = _descriptor.GetProperty(key);
            if (property != null && property.GetMethod != null && !property.GetMethod.IsStatic)
            {
                var value = property.GetValue(Instance);
                return ClrMarshaller.ToScript(value, _registry);
            }

            var field = _descriptor.GetField(key);
            if (field != null && !field.IsStatic)
            {
                var value = field.GetValue(Instance);
                return ClrMarshaller.ToScript(value, _registry);
            }

            var methods = _descriptor.GetMethods(key);
            if (methods != null)
            {
                var instanceMethods = methods.Where(m => !m.IsStatic).ToArray();
                if (instanceMethods.Length > 0)
            {
                    return new ClrMethodBinding(_descriptor, instanceMethods, this, _registry, false);
                }
            }

            return base.GetPropertyValue(key);
        }

        public override void SetPropertyValue(string key, ScriptObject value)
        {
            var property = _descriptor.GetProperty(key);
            if (property != null && property.SetMethod != null && !property.SetMethod.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, property.PropertyType, _registry, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{property.PropertyType.FullName}'.");
                }
                property.SetValue(Instance, converted);
                return;
            }

            var field = _descriptor.GetField(key);
            if (field != null && !field.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, field.FieldType, _registry, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{field.FieldType.FullName}'.");
                }
                field.SetValue(Instance, converted);
                return;
            }

            base.SetPropertyValue(key, value);
        }

        public override string ToString()
        {
            return Instance?.ToString() ?? base.ToString();
        }
    }
}

