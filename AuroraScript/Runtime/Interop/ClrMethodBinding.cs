using AuroraScript.Core;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using System;
using System.Linq;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrMethodBinding : ScriptObject, IClrInvokable
    {
        private readonly ClrTypeDescriptor _descriptor;
        private readonly MethodBase[] _methods;
        private readonly ClrInstanceObject _instance;
        private readonly ClrTypeRegistry _registry;
        private readonly bool _isStatic;

        internal ClrMethodBinding(ClrTypeDescriptor descriptor, MethodBase[] methods, ClrInstanceObject instance, ClrTypeRegistry registry, bool isStatic)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            _methods = methods ?? throw new ArgumentNullException(nameof(methods));
            _instance = instance;
            _registry = registry;
            _isStatic = isStatic;
        }

        public ScriptDatum Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            args ??= Array.Empty<ScriptDatum>();
            var targetHolder = _instance ?? thisObject as ClrInstanceObject;
            var targetInstance = _isStatic ? null : targetHolder?.Instance;
            foreach (var method in _methods)
            {
                if (_isStatic != method.IsStatic) continue;
                if (TryInvoke(method, targetInstance, args, out var result))
                {
                    return result;
                }
            }

            throw new InvalidOperationException($"No matching method overload found on '{_descriptor.Type.FullName}'.");
        }

        private bool TryInvoke(MethodBase method, object target, ScriptDatum[] args, out ScriptDatum result)
        {
            if (!TryBuildArguments(method, args, out var invokeArgs))
            {
                result = ScriptDatum.FromNull();
                return false;
            }

            var invocationResult = method.Invoke(target, invokeArgs);
            if (method is MethodInfo methodInfo && methodInfo.ReturnType == typeof(void))
            {
                result = ScriptDatum.FromNull();
            }
            else
            {
                result = ClrMarshaller.ToDatum(invocationResult, _registry);
            }
            return true;
        }

        private bool TryBuildArguments(MethodBase method, ScriptDatum[] args, out object[] invokeArgs)
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
                if (!ClrMarshaller.TryConvertArgument(args[i], parameters[i].ParameterType, _registry, out var converted))
                {
                    invokeArgs = null;
                    return false;
                }
                invokeArgs[i] = converted;
            }
            return true;
        }
    }
}

