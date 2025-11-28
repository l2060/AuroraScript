using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace AuroraScript.Runtime.Interop
{
    /// <summary>
    /// Utilities for converting CLR-side data structures into AuroraScript <see cref="ScriptObject"/> instances.
    /// Useful when invoking script callbacks from CLR code or when populating arguments for script methods.
    /// </summary>
    public static class ClrValueConverter
    {
        /// <summary>
        /// Convert a CLR value into a script-visible object representation.
        /// </summary>
        public static ScriptObject ToScriptObject(object value, ClrTypeRegistry registry)
        {
            if (value == null)
            {
                return ScriptObject.Null;
            }

            if (value is ScriptObject scriptObject)
            {
                return scriptObject;
            }

            if (value is ScriptDatum datum)
            {
                return datum.ToObject();
            }

            switch (value)
            {
                case bool boolean:
                    return BooleanValue.Of(boolean);
                case string str:
                    return StringValue.Of(str);
                case char ch:
                    return StringValue.Of(ch.ToString());
            }

            if (IsNumeric(value))
            {
                return NumberValue.Of(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            if (value is Enum enumValue)
            {
                return NumberValue.Of(Convert.ToDouble(enumValue, CultureInfo.InvariantCulture));
            }

            if (value is Delegate handler)
            {
                return WrapDelegate(handler, registry);
            }

            if (value is IDictionary dictionary)
            {
                return ConvertDictionary(dictionary, registry);
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                return ToScriptArray(enumerable, registry);
            }

            if (registry != null && registry.TryGetDescriptor(value.GetType(), out var descriptor))
            {
                return new ClrInstanceObject(descriptor, value, registry);
            }

            throw new InvalidOperationException($"Cannot convert value of type '{value.GetType().FullName}' to ScriptObject. Register the CLR type or provide a custom adapter.");
        }

        /// <summary>
        /// Converts a CLR enumerable (array, list, etc.) into a <see cref="ScriptArray"/>.
        /// </summary>
        public static ScriptArray ToScriptArray(IEnumerable values, ClrTypeRegistry registry)
        {
            var array = new ScriptArray();
            if (values == null)
            {
                return array;
            }

            foreach (var item in values)
            {
                array.PushDatum(ScriptDatum.FromObject(ToScriptObject(item, registry)));
            }

            return array;
        }

        /// <summary>
        /// Converts a set of CLR arguments to script values.
        /// </summary>
        public static ScriptObject[] ToScriptObjectArray(IEnumerable<object> values, ClrTypeRegistry registry)
        {
            if (values == null)
            {
                return Array.Empty<ScriptObject>();
            }
            return values.Select(v => ToScriptObject(v, registry)).ToArray();
        }

        /// <summary>
        /// Converts a set of CLR arguments into <see cref="ScriptDatum"/> instances, ready for VM invocation.
        /// </summary>
        public static ScriptDatum[] ToDatumArray(IEnumerable<object> values, ClrTypeRegistry registry)
        {
            if (values == null)
            {
                return Array.Empty<ScriptDatum>();
            }

            return values.Select(v => ScriptDatum.FromObject(ToScriptObject(v, registry))).ToArray();
        }

        private static ScriptObject ConvertDictionary(IDictionary dictionary, ClrTypeRegistry registry)
        {
            var obj = new ScriptObject();
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key?.ToString() ?? string.Empty;
                obj.SetPropertyValue(key, ToScriptObject(entry.Value, registry));
            }

            return obj;
        }

        private static ScriptObject WrapDelegate(Delegate handler, ClrTypeRegistry registry)
        {
            if (handler is ClrDatumDelegate datumDelegate)
            {
                return new BondingFunction(datumDelegate);
            }

            return new BondingFunction((context, thisObject, args) =>
            {
                var prepared = PrepareDelegateArguments(handler, args, registry);
                var result = handler.DynamicInvoke(prepared);
                return ToScriptObject(result, registry);
            });
        }

        private static object[] PrepareDelegateArguments(Delegate handler, ScriptDatum[] args, ClrTypeRegistry registry)
        {
            var parameters = handler.Method.GetParameters();
            if (parameters.Length == 0)
            {
                return Array.Empty<object>();
            }

            var prepared = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (IsParamArray(parameters[i]))
                {
                    prepared[i] = ConvertParamArray(parameters[i], args, i, registry);
                    continue;
                }

                if (args != null && i < args.Length)
                {
                    if (!ClrMarshaller.TryConvertArgument(args[i], parameters[i].ParameterType, registry, out var converted))
                    {
                        throw new InvalidOperationException($"Cannot convert script argument #{i} to '{parameters[i].ParameterType.FullName}' for delegate '{handler.Method.DeclaringType?.Name}.{handler.Method.Name}'.");
                    }
                    prepared[i] = converted;
                    continue;
                }

                if (parameters[i].HasDefaultValue)
                {
                    prepared[i] = parameters[i].DefaultValue;
                    continue;
                }

                prepared[i] = parameters[i].ParameterType.IsValueType && Nullable.GetUnderlyingType(parameters[i].ParameterType) == null
                    ? Activator.CreateInstance(parameters[i].ParameterType)
                    : null;
            }

            return prepared;
        }

        private static object ConvertParamArray(ParameterInfo parameter, ScriptDatum[] args, int startIndex, ClrTypeRegistry registry)
        {
            var elementType = parameter.ParameterType.GetElementType() ?? typeof(object);
            var available = Math.Max(0, (args?.Length ?? 0) - startIndex);
            var array = Array.CreateInstance(elementType, available);
            for (int offset = 0; offset < available; offset++)
            {
                if (!ClrMarshaller.TryConvertArgument(args[startIndex + offset], elementType, registry, out var converted))
                {
                    throw new InvalidOperationException($"Cannot convert variadic script argument #{startIndex + offset} to '{elementType.FullName}'.");
                }
                array.SetValue(converted, offset);
            }

            return array;
        }

        private static bool IsParamArray(ParameterInfo parameter)
        {
            return parameter.GetCustomAttribute<ParamArrayAttribute>() != null;
        }

        private static bool IsNumeric(object value)
        {
            if (value == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}

