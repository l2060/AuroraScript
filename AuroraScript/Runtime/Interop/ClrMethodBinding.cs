using AuroraScript.Core;
using AuroraScript.Runtime;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrMethodBinding : ScriptObject, IClrInvokable
    {
        private readonly ClrTypeDescriptor _descriptor;
        private readonly MethodInvoker[] _compiledInvokers;
        private readonly ClrInstanceObject _instance;
        private readonly ClrTypeRegistry _registry;
        private readonly bool _isStatic;

        internal ClrMethodBinding(ClrTypeDescriptor descriptor, MethodBase[] methods, ClrInstanceObject instance, ClrTypeRegistry registry, bool isStatic)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (methods == null)
            {
                throw new ArgumentNullException(nameof(methods));
            }
            _instance = instance;
            _registry = registry;
            _isStatic = isStatic;

            _compiledInvokers = CompileInvokers(methods);
        }

        public ScriptDatum Invoke(ExecuteContext context, ScriptObject thisObject, ScriptDatum[] args)
        {
            args ??= Array.Empty<ScriptDatum>();
            var targetHolder = _instance ?? thisObject as ClrInstanceObject;
            if (!_isStatic && targetHolder == null)
            {
                throw new InvalidOperationException($"Instance method '{_descriptor.Type.FullName}' requires a CLR target. Ensure the object is bound correctly before invoking.");
            }
            var targetInstance = _isStatic ? null : targetHolder.Instance;
            var invokers = _compiledInvokers;
            for (int i = 0; i < invokers.Length; i++)
            {
                ref readonly var invoker = ref invokers[i];
                if (_isStatic != invoker.IsStatic)
                {
                    continue;
                }

                if (invoker.TryInvoke(targetInstance, args, _registry, out var result))
                {
                    return result;
                }
            }

            throw new InvalidOperationException($"No matching method overload found on '{_descriptor.Type.FullName}'.");
        }

        private static MethodInvoker[] CompileInvokers(MethodBase[] methods)
        {
            var invokers = new List<MethodInvoker>(methods.Length);
            foreach (var method in methods)
            {
                invokers.Add(MethodInvoker.Compiler.Compile(method));
            }
            return invokers.ToArray();
        }
        private delegate bool InvokeDelegate(object target, ScriptDatum[] args, ClrTypeRegistry registry, out ScriptDatum result);

        private readonly struct MethodInvoker
        {
            private readonly InvokeDelegate _invoke;
            private readonly int _expectedArgumentCount;

            public bool IsStatic { get; }

            private MethodInvoker(bool isStatic, int expectedArgumentCount, InvokeDelegate invoke)
            {
                IsStatic = isStatic;
                _expectedArgumentCount = expectedArgumentCount;
                _invoke = invoke;
            }

            public bool TryInvoke(object target, ScriptDatum[] args, ClrTypeRegistry registry, out ScriptDatum result)
            {
                if (!IsStatic && target == null)
                {
                    result = ScriptDatum.FromNull();
                    return false;
                }

                var effectiveArgs = args ?? Array.Empty<ScriptDatum>();
                if (_expectedArgumentCount >= 0 && effectiveArgs.Length != _expectedArgumentCount)
                {
                    result = ScriptDatum.FromNull();
                    return false;
                }

                return _invoke(target, effectiveArgs, registry, out result);
            }

            public static class Compiler
            {
                public static MethodInvoker Compile(MethodBase method)
                {
                    if (method is MethodInfo methodInfo)
                    {
                        return CompileMethod(methodInfo);
                    }

                    throw new NotSupportedException($"Unsupported method type '{method.GetType().FullName}'.");
                }

                private static MethodInvoker CompileMethod(MethodInfo method)
                {
                    var parameters = method.GetParameters();
                    var expectedArgs = parameters.Length;
                    InvokeDelegate invokeDelegate;

                    if (expectedArgs == 0)
                    {
                        invokeDelegate = CompileNoArgs(method);
                    }
                    else if (expectedArgs == 1)
                    {
                        invokeDelegate = CompileSingleArg(method, parameters[0]);
                    }
                    else
                    {
                        var invoker = new ReflectionInvoker(method);
                        invokeDelegate = invoker.Invoke;
                    }

                    return new MethodInvoker(method.IsStatic, expectedArgs, invokeDelegate);
                }

                private static InvokeDelegate CompileNoArgs(MethodInfo method)
                {
                    if (method.ReturnType == typeof(void))
                    {
                        return (object target, ScriptDatum[] arguments, ClrTypeRegistry registry, out ScriptDatum result) =>
                        {
                            method.Invoke(target, Array.Empty<object>());
                            result = ScriptDatum.FromNull();
                            return true;
                        };
                    }

                    return (object target, ScriptDatum[] arguments, ClrTypeRegistry registry, out ScriptDatum result) =>
                    {
                        var invocationResult = method.Invoke(target, Array.Empty<object>());
                        result = ClrMarshaller.ToDatum(invocationResult, registry);
                        return true;
                    };
                }

                private static InvokeDelegate CompileSingleArg(MethodInfo method, ParameterInfo parameter)
                {
                    var parameterType = parameter.ParameterType;
                    if (method.ReturnType == typeof(void))
                    {
                        return (object target, ScriptDatum[] args, ClrTypeRegistry registry, out ScriptDatum result) =>
                        {
                            if (!ClrMarshaller.TryConvertArgument(args[0], parameterType, registry, out var converted))
                            {
                                result = ScriptDatum.FromNull();
                                return false;
                            }

                            method.Invoke(target, new[] { converted });
                            result = ScriptDatum.FromNull();
                            return true;
                        };
                    }

                    return (object target, ScriptDatum[] args, ClrTypeRegistry registry, out ScriptDatum result) =>
                    {
                        if (!ClrMarshaller.TryConvertArgument(args[0], parameterType, registry, out var converted))
                        {
                            result = ScriptDatum.FromNull();
                            return false;
                        }

                        var invocationResult = method.Invoke(target, new[] { converted });
                        result = ClrMarshaller.ToDatum(invocationResult, registry);
                        return true;
                    };
                }

                private sealed class ReflectionInvoker
                {
                    private readonly MethodInfo _method;
                    private readonly ParameterInfo[] _parameters;

                    public ReflectionInvoker(MethodInfo method)
                    {
                        _method = method;
                        _parameters = method.GetParameters();
                    }

                    public bool Invoke(object target, ScriptDatum[] args, ClrTypeRegistry registry, out ScriptDatum result)
                    {
                        var invokeArgs = new object[_parameters.Length];
                        for (int i = 0; i < _parameters.Length; i++)
                        {
                            if (!ClrMarshaller.TryConvertArgument(args[i], _parameters[i].ParameterType, registry, out var converted))
                            {
                                result = ScriptDatum.FromNull();
                                return false;
                            }
                            invokeArgs[i] = converted;
                        }

                        var invocationResult = _method.Invoke(target, invokeArgs);
                        if (_method.ReturnType == typeof(void))
                        {
                            result = ScriptDatum.FromNull();
                            return true;
                        }

                        result = ClrMarshaller.ToDatum(invocationResult, registry);
                        return true;
                    }
                }
            }
        }

    }
}

