using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Globalization;

namespace AuroraScript.Runtime.Interop
{
    internal static class ClrMarshaller
    {
        public static bool TryConvertArgument(ScriptObject scriptValue, Type targetType, ClrTypeRegistry registry, out object result)
        {
            result = null;
            if (targetType == typeof(ScriptObject) || targetType == typeof(ScriptObject[]))
            {
                result = scriptValue;
                return true;
            }

            if (scriptValue == ScriptObject.Null)
            {
                if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
                {
                    result = null;
                    return true;
                }
                return false;
            }

            if (targetType == typeof(string))
            {
                if (scriptValue is StringValue str)
                {
                    result = str.Value;
                    return true;
                }
                result = scriptValue.ToString();
                return true;
            }

            if (IsNumericType(targetType))
            {
                if (scriptValue is NumberValue number)
                {
                    try
                    {
                        result = Convert.ChangeType(number.DoubleValue, Nullable.GetUnderlyingType(targetType) ?? targetType, CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }

            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                if (scriptValue is BooleanValue booleanValue)
                {
                    result = booleanValue.Value;
                    return true;
                }
                result = scriptValue.IsTrue();
                return true;
            }

            if (targetType.IsAssignableFrom(typeof(ScriptObject)))
            {
                result = scriptValue;
                return true;
            }

            if (scriptValue is ClrInstanceObject clrInstance)
            {
                var instance = clrInstance.Instance;
                if (instance == null)
                {
                    result = null;
                    return !targetType.IsValueType;
                }
                if (targetType.IsInstanceOfType(instance))
                {
                    result = instance;
                    return true;
                }
            }

            return false;
        }

        public static bool TryConvertArgument(ScriptDatum datum, Type targetType, ClrTypeRegistry registry, out object result)
        {
            switch (datum.Kind)
            {
                case ValueKind.Null:
                    if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null)
                    {
                        result = null;
                        return true;
                    }
                    result = null;
                    return false;
                case ValueKind.Boolean:
                    if (targetType == typeof(bool) || targetType == typeof(bool?))
                    {
                        result = datum.Boolean;
                        return true;
                    }
                    if (IsNumericType(targetType))
                    {
                        result = Convert.ChangeType(datum.Boolean ? 1 : 0, Nullable.GetUnderlyingType(targetType) ?? targetType, CultureInfo.InvariantCulture);
                        return true;
                    }
                    break;
                case ValueKind.Number:
                    if (IsNumericType(targetType))
                    {
                        result = Convert.ChangeType(datum.Number, Nullable.GetUnderlyingType(targetType) ?? targetType, CultureInfo.InvariantCulture);
                        return true;
                    }
                    if (targetType == typeof(bool) || targetType == typeof(bool?))
                    {
                        result = datum.Number != 0 && !double.IsNaN(datum.Number);
                        return true;
                    }
                    break;
                case ValueKind.String:
                    if (targetType == typeof(string))
                    {
                        result = datum.String?.Value;
                        return true;
                    }
                    if (targetType == typeof(ScriptObject))
                    {
                        result = datum.String;
                        return true;
                    }
                    break;
                case ValueKind.Object:
                    return TryConvertArgument(datum.Object, targetType, registry, out result);
            }
            result = null;
            return false;
        }

        public static ScriptObject ToScript(object value, ClrTypeRegistry registry)
        {
            if (value == null)
            {
                return ScriptObject.Null;
            }

            if (value is ScriptObject scriptObject)
            {
                return scriptObject;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return BooleanValue.Of((bool)value);
                case TypeCode.String:
                    return StringValue.Of((string)value);
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return NumberValue.Of(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            if (value is Enum enumValue)
            {
                return NumberValue.Of(Convert.ToDouble(enumValue, CultureInfo.InvariantCulture));
            }

            if (registry != null && registry.TryGetDescriptor(value.GetType(), out var descriptor))
            {
                return new ClrInstanceObject(descriptor, value, registry);
            }

            throw new InvalidOperationException($"The return type '{value.GetType().FullName}' is not registered for CLR interop.");
        }

        private static bool IsNumericType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            switch (Type.GetTypeCode(type))
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

