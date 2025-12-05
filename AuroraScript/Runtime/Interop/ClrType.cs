using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Linq;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrType : ScriptObject, IClrInvokable
    {
        private readonly ClrTypeDescriptor _descriptor;
        private readonly Lazy<ConstructorInfo[]> _constructors;

        public ClrType(Type type, ClrTypeDescriptor descriptor)
        {
            _constructors = new Lazy<ConstructorInfo[]>(() => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Frozen();
        }

        public ScriptObject Construct(Span<ScriptDatum> args)
        {
            var constructors = _constructors.Value;
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"CLR type '{_descriptor.Type.FullName}' does not expose public constructors.");
            }
            //args ??= Array.Empty<ScriptDatum>();

            foreach (var ctor in constructors)
            {
                if (!TryBuildArguments(ctor, args, out var invokeArgs))
                {
                    continue;
                }
                var instance = ctor.Invoke(invokeArgs);
                return new ClrInstanceObject(_descriptor, instance);
            }
            throw new InvalidOperationException($"No matching constructor found for '{_descriptor.Type.FullName}'.");
        }

        public ScriptDatum Invoke(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            return ScriptDatum.FromObject(Construct(args));
        }

        public override ScriptObject GetPropertyValue(string key)
        {
            var property = _descriptor.GetProperty(key);
            if (property != null && property.GetMethod != null && property.GetMethod.IsStatic)
            {
                var value = property.GetValue(null);
                return ClrMarshaller.ToScript(value);
            }

            var field = _descriptor.GetField(key);
            if (field != null && field.IsStatic)
            {
                var value = field.GetValue(null);
                return ClrMarshaller.ToScript(value);
            }

            var methods = _descriptor.GetMethods(key);
            if (methods != null)
            {
                var staticMethods = methods.Where(m => m.IsStatic).ToArray();
                if (staticMethods.Length > 0)
                {
                    return new ClrMethodBinding(_descriptor, staticMethods, null, true);
                }
            }

            return base.GetPropertyValue(key);
        }

        public override void SetPropertyValue(string key, ScriptObject value)
        {
            var property = _descriptor.GetProperty(key);
            if (property != null && property.SetMethod != null && property.SetMethod.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, property.PropertyType, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{property.PropertyType.FullName}'.");
                }
                property.SetValue(null, converted);
                return;
            }

            var field = _descriptor.GetField(key);
            if (field != null && field.IsStatic)
            {
                if (!ClrMarshaller.TryConvertArgument(value, field.FieldType, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert script value to '{field.FieldType.FullName}'.");
                }
                field.SetValue(null, converted);
                return;
            }

            base.SetPropertyValue(key, value);
        }

        internal bool TryBuildArguments(MethodBase method, Span<ScriptDatum> args, out object[] invokeArgs)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != args.Length)
            {
                invokeArgs = null;
                return false;
            }

            invokeArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (!ClrMarshaller.TryConvertArgument(args[i], parameters[i].ParameterType, out var converted))
                {
                    invokeArgs = null;
                    return false;
                }
                invokeArgs[i] = converted;
            }
            return true;
        }

        public override string ToString()
        {
            return $"[clr type {_descriptor.Type.FullName}]";
        }
    }
}

