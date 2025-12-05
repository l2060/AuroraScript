using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    public sealed class ClrMethodBinding : ScriptObject, IClrInvokable
    {
        private readonly ClrTypeDescriptor _descriptor;
        private readonly MethodInvoker[] _compiledInvokers;
        private readonly ClrInstanceObject _instance;
        private readonly bool _isStatic;

        internal ClrMethodBinding(ClrTypeDescriptor descriptor, MethodBase[] methods, ClrInstanceObject instance, bool isStatic)
        {
            _descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            if (methods == null)
            {
                throw new ArgumentNullException(nameof(methods));
            }
            _instance = instance;
            _isStatic = isStatic;
            _compiledInvokers = CompileInvokers(methods);
        }

        public ScriptDatum Invoke(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args)
        {
            //args ??= Array.Empty<ScriptDatum>();
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

                if (invoker.TryInvoke(targetInstance, args, out var result))
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
        private delegate bool InvokeDelegate(object target, Span<ScriptDatum> args, out ScriptDatum result);

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

            public bool TryInvoke(object target, Span<ScriptDatum> args, out ScriptDatum result)
            {
                if (!IsStatic && target == null)
                {
                    result = ScriptDatum.FromNull();
                    return false;
                }

                var effectiveArgs = args;
                if (_expectedArgumentCount >= 0 && effectiveArgs.Length != _expectedArgumentCount)
                {
                    result = ScriptDatum.FromNull();
                    return false;
                }

                return _invoke(target, effectiveArgs, out result);
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
                        //    invokeDelegate = ExpressionInvokerBuilder.TryBuild(method) ?? new ReflectionInvoker(method).Invoke;

                        var invoker = new ReflectionInvoker(method);
                        invokeDelegate = invoker.Invoke;
                    }

                    return new MethodInvoker(method.IsStatic, expectedArgs, invokeDelegate);
                }

                private static InvokeDelegate CompileNoArgs(MethodInfo method)
                {
                    if (method.ReturnType == typeof(void))
                    {
                        return (object target, Span<ScriptDatum> arguments, out ScriptDatum result) =>
                        {
                            method.Invoke(target, Array.Empty<object>());
                            result = ScriptDatum.FromNull();
                            return true;
                        };
                    }

                    return (object target, Span<ScriptDatum> arguments, out ScriptDatum result) =>
                    {
                        var invocationResult = method.Invoke(target, Array.Empty<object>());
                        result = ClrMarshaller.ToDatum(invocationResult);
                        return true;
                    };
                }

                private static InvokeDelegate CompileSingleArg(MethodInfo method, ParameterInfo parameter)
                {
                    var parameterType = parameter.ParameterType;
                    if (method.ReturnType == typeof(void))
                    {
                        return (object target, Span<ScriptDatum> args, out ScriptDatum result) =>
                        {
                            if (!ClrMarshaller.TryConvertArgument(args[0], parameterType, out var converted))
                            {
                                result = ScriptDatum.FromNull();
                                return false;
                            }

                            method.Invoke(target, new[] { converted });
                            result = ScriptDatum.FromNull();
                            return true;
                        };
                    }

                    return (object target, Span<ScriptDatum> args, out ScriptDatum result) =>
                    {
                        if (!ClrMarshaller.TryConvertArgument(args[0], parameterType, out var converted))
                        {
                            result = ScriptDatum.FromNull();
                            return false;
                        }

                        var invocationResult = method.Invoke(target, new[] { converted });
                        result = ClrMarshaller.ToDatum(invocationResult);
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

                    public bool Invoke(object target, Span<ScriptDatum> args, out ScriptDatum result)
                    {
                        var invokeArgs = new object[_parameters.Length];
                        for (int i = 0; i < _parameters.Length; i++)
                        {
                            if (!ClrMarshaller.TryConvertArgument(args[i], _parameters[i].ParameterType, out var converted))
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

                        result = ClrMarshaller.ToDatum(invocationResult);
                        return true;
                    }
                }

                private static class ExpressionInvokerBuilder
                {
                    private static readonly MethodInfo TryConvertDatumMethod = typeof(ClrMarshaller).GetMethod(
                        nameof(ClrMarshaller.TryConvertArgument),
                        BindingFlags.Public | BindingFlags.Static,
                        binder: null,
                        types: new[] { typeof(ScriptDatum), typeof(Type), typeof(object).MakeByRefType() },
                        modifiers: null);

                    private static readonly MethodInfo ToDatumMethod = typeof(ClrMarshaller).GetMethod(
                        nameof(ClrMarshaller.ToDatum),
                        BindingFlags.Public | BindingFlags.Static,
                        binder: null,
                        types: new[] { typeof(object) },
                        modifiers: null);

                    private static readonly MethodInfo FromNullMethod = typeof(ScriptDatum).GetMethod(
                        nameof(ScriptDatum.FromNull),
                        BindingFlags.Public | BindingFlags.Static);

                    public static InvokeDelegate TryBuild(MethodInfo method)
                    {
                        if (!IsSupported(method))
                        {
                            return null;
                        }

                        if (TryConvertDatumMethod == null || ToDatumMethod == null || FromNullMethod == null)
                        {
                            return null;
                        }

                        try
                        {
                            return Compile(method);
                        }
                        catch
                        {
                            return null;
                        }
                    }

                    private static bool IsSupported(MethodInfo method)
                    {
                        if (method.ContainsGenericParameters)
                        {
                            return false;
                        }

                        var parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var parameter = parameters[i];
                            if (parameter.ParameterType.IsByRef || parameter.IsOut)
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    private static InvokeDelegate Compile(MethodInfo method)
                    {
                        var parameters = method.GetParameters();
                        var targetParameter = Expression.Parameter(typeof(object), "target");
                        var argsParameter = Expression.Parameter(typeof(ScriptDatum[]), "args");

                        var resultParameter = Expression.Parameter(typeof(ScriptDatum).MakeByRefType(), "result");

                        var variables = new List<ParameterExpression>();
                        var bodyExpressions = new List<Expression>();
                        var returnLabel = Expression.Label(typeof(bool), "returnLabel");

                        Expression failureBlock = Expression.Block(
                            Expression.Assign(resultParameter, Expression.Call(FromNullMethod)),
                            Expression.Return(returnLabel, Expression.Constant(false))
                        );

                        Expression instanceExpression = null;
                        if (!method.IsStatic)
                        {
                            var declaringType = method.DeclaringType ?? throw new InvalidOperationException($"Method '{method.Name}' does not have declaring type.");
                            instanceExpression = Expression.Convert(targetParameter, declaringType);
                        }

                        var typedArguments = new Expression[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var parameter = parameters[i];
                            var parameterType = parameter.ParameterType;
                            var convertedVar = Expression.Variable(typeof(object), $"arg{i}Obj");
                            var typedVar = Expression.Variable(parameterType, $"arg{i}");
                            variables.Add(convertedVar);
                            variables.Add(typedVar);

                            var conversionCall = Expression.Call(
                                TryConvertDatumMethod,
                                Expression.ArrayIndex(argsParameter, Expression.Constant(i)),
                                Expression.Constant(parameterType, typeof(Type)),
                                convertedVar);

                            bodyExpressions.Add(Expression.IfThen(Expression.IsFalse(conversionCall), failureBlock));
                            bodyExpressions.Add(Expression.Assign(typedVar, Expression.Convert(convertedVar, parameterType)));
                            typedArguments[i] = typedVar;
                        }

                        var callExpression = Expression.Call(instanceExpression, method, typedArguments);

                        if (method.ReturnType == typeof(void))
                        {
                            bodyExpressions.Add(callExpression);
                            bodyExpressions.Add(Expression.Assign(resultParameter, Expression.Call(FromNullMethod)));
                        }
                        else
                        {
                            var convertedResult = Expression.Call(
                                ToDatumMethod,
                                Expression.Convert(callExpression, typeof(object)));
                            bodyExpressions.Add(Expression.Assign(resultParameter, convertedResult));
                        }

                        bodyExpressions.Add(Expression.Return(returnLabel, Expression.Constant(true)));
                        bodyExpressions.Add(Expression.Label(returnLabel, Expression.Constant(false)));

                        var body = Expression.Block(variables, bodyExpressions);
                        var lambda = Expression.Lambda<InvokeDelegate>(body, targetParameter, argsParameter, resultParameter);
                        return lambda.Compile();
                    }
                }
            }
        }

    }
}

