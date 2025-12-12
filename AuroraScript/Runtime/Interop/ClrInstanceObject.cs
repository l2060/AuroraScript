using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using System;
using System.Linq;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrInstanceObject : ScriptObject
    {
        public readonly ClrTypeDescriptor Descriptor;

        public readonly object Instance;


        internal ClrInstanceObject(ClrTypeDescriptor descriptor, object instance)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Instance = instance;
        }

        public override ScriptObject GetPropertyValue(string key)
        {
            var property = Descriptor.GetProperty(key);
            if (property != null && property.GetMethod != null && !property.GetMethod.IsStatic)
            {
                var value = property.GetValue(Instance);
                return ClrMarshaller.ToScript(value);
            }

            var field = Descriptor.GetField(key);
            if (field != null && !field.IsStatic)
            {
                var value = field.GetValue(Instance);
                return ClrMarshaller.ToScript(value);
            }

            var methods = Descriptor.GetMethods(key);
            if (methods != null)
            {
                var instanceMethods = methods.Where(m => !m.IsStatic).ToArray();
                if (instanceMethods.Length > 0)
                {
                    return new ClrMethodBinding(Descriptor, instanceMethods, this, false);
                }
            }
            ThrowHelper.ThrowNotfoundProperty(key);
            return ScriptObject.Null;
        }

        public override void SetPropertyValue(string key, ScriptObject value)
        {
            var property = Descriptor.GetProperty(key);
            if (property != null && property.SetMethod != null && !property.SetMethod.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, property.PropertyType, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{property.PropertyType.FullName}'.");
                }
                property.SetValue(Instance, converted);
                return;
            }

            var field = Descriptor.GetField(key);
            if (field != null && !field.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, field.FieldType, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{field.FieldType.FullName}'.");
                }
                field.SetValue(Instance, converted);
                return;
            }
            ThrowHelper.ThrowNotfoundProperty(key);
            //base.SetPropertyValue(key, value);
        }






        public override string ToString()
        {
            return Instance.ToString() ?? base.ToString();
        }
    }
}

