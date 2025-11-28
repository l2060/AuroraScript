using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Collections;
using System.Collections.Generic;
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

            if (scriptValue is ScriptArray scriptArray)
            {
                if (TryConvertScriptArray(scriptArray, targetType, registry, out result))
                {
                    return true;
                }
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

                case ValueKind.Array:
                    if (datum.Object is ScriptArray arrayValue && TryConvertScriptArray(arrayValue, targetType, registry, out result))
                    {
                        return true;
                    }
                    break;


            }
            result = null;
            return false;
        }

        public static ScriptDatum ToDatum(object value, ClrTypeRegistry registry)
        {
            if (value == null)
            {
                return ScriptDatum.FromNull();
            }

            if (value is ScriptDatum datum)
            {
                return datum;
            }

            if (value is ScriptObject scriptObject)
            {
                return ScriptDatum.FromObject(scriptObject);
            }

            if (value is bool boolean)
            {
                return ScriptDatum.FromBoolean(boolean);
            }

            if (value is StringValue stringValue)
            {
                return ScriptDatum.FromString(stringValue);
            }

            if (value is string str)
            {
                return ScriptDatum.FromString(StringValue.Of(str));
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    return ScriptDatum.FromNumber(Convert.ToDouble(value, CultureInfo.InvariantCulture));
            }

            if (value is Enum enumValue)
            {
                return ScriptDatum.FromNumber(Convert.ToDouble(enumValue, CultureInfo.InvariantCulture));
            }

            if (registry != null && registry.TryGetDescriptor(value.GetType(), out var descriptor))
            {
                return ScriptDatum.FromObject(new ClrInstanceObject(descriptor, value, registry));
            }

            throw new InvalidOperationException($"The return type '{value.GetType().FullName}' is not registered for CLR interop.");
        }

        public static ScriptObject ToScript(object value, ClrTypeRegistry registry)
        {
            return ToDatum(value, registry).ToObject();
        }


        public static ScriptDatum[] ConvertArguments(ScriptObject[] arguments)
        {

            if (arguments == null || arguments.Length == 0)
            {
                return Array.Empty<ScriptDatum>();
            }
            var result = new ScriptDatum[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = ScriptDatum.FromObject(arguments[i]);
            }
            return result;
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

        private static bool TryConvertScriptArray(ScriptArray scriptArray, Type targetType, ClrTypeRegistry registry, out object result)
        {
            if (targetType == typeof(ScriptArray) || typeof(ScriptArray).IsAssignableFrom(targetType))
            {
                result = scriptArray;
                return true;
            }

            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType() ?? typeof(object);
                return TryConvertToClrArray(scriptArray, elementType, registry, out result);
            }

            if (TryGetGenericEnumerableElementType(targetType, out var element))
            {
                if (!TryConvertToTypedList(scriptArray, element, registry, out var listObject))
                {
                    result = null;
                    return false;
                }

                if (targetType.IsInterface || targetType.IsAssignableFrom(listObject.GetType()))
                {
                    result = listObject;
                    return true;
                }

                var enumerableType = typeof(IEnumerable<>).MakeGenericType(element);
                var ctor = targetType.GetConstructor(new[] { enumerableType });
                if (ctor != null)
                {
                    result = ctor.Invoke(new[] { listObject });
                    return true;
                }

                if (!targetType.IsAbstract && typeof(IList).IsAssignableFrom(targetType))
                {
                    var concreteList = (IList)Activator.CreateInstance(targetType);
                    foreach (var item in (IEnumerable)listObject)
                    {
                        concreteList.Add(item);
                    }
                    result = concreteList;
                    return true;
                }

                result = null;
                return false;
            }

            if (typeof(IList).IsAssignableFrom(targetType))
            {
                IList listInstance;
                if (targetType.IsInterface || targetType.IsAbstract)
                {
                    listInstance = new ArrayList();
                }
                else
                {
                    listInstance = (IList)Activator.CreateInstance(targetType);
                }

                for (int i = 0; i < scriptArray.Length; i++)
                {
                    var datum = scriptArray.Get(i);
                    listInstance.Add(datum.ToObject());
                }

                result = listInstance;
                return true;
            }

            if (typeof(IEnumerable).IsAssignableFrom(targetType) && (targetType.IsInterface || targetType == typeof(IEnumerable)))
            {
                var arrayList = new ArrayList();
                for (int i = 0; i < scriptArray.Length; i++)
                {
                    arrayList.Add(scriptArray.Get(i).ToObject());
                }
                result = arrayList;
                return true;
            }

            result = null;
            return false;
        }

        private static bool TryConvertToClrArray(ScriptArray scriptArray, Type elementType, ClrTypeRegistry registry, out object result)
        {
            var length = scriptArray.Length;
            var arrayInstance = Array.CreateInstance(elementType, length);

            for (int i = 0; i < length; i++)
            {
                var datum = scriptArray.Get(i);
                if (!TryConvertArgument(datum, elementType, registry, out var converted))
                {
                    if (!TryFallbackArrayConversion(datum, elementType, out converted))
                    {
                        result = null;
                        return false;
                    }
                }
                arrayInstance.SetValue(converted, i);
            }

            result = arrayInstance;
            return true;
        }

        private static bool TryConvertToTypedList(ScriptArray scriptArray, Type elementType, ClrTypeRegistry registry, out object listObject)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            for (int i = 0; i < scriptArray.Length; i++)
            {
                var datum = scriptArray.Get(i);
                if (!TryConvertArgument(datum, elementType, registry, out var converted))
                {
                    if (!TryFallbackArrayConversion(datum, elementType, out converted))
                    {
                        listObject = null;
                        return false;
                    }
                }
                list.Add(converted);
            }

            listObject = list;
            return true;
        }

        private static bool TryFallbackArrayConversion(ScriptDatum datum, Type elementType, out object converted)
        {
            if (elementType == typeof(object) || elementType == typeof(ScriptObject))
            {
                converted = datum.ToObject();
                return true;
            }

            if (elementType == typeof(ScriptDatum))
            {
                converted = datum;
                return true;
            }

            converted = null;
            return false;
        }

        private static bool TryGetGenericEnumerableElementType(Type targetType, out Type elementType)
        {
            if (targetType.IsGenericType && IsSupportedGenericEnumerable(targetType.GetGenericTypeDefinition()))
            {
                elementType = targetType.GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in targetType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && IsSupportedGenericEnumerable(interfaceType.GetGenericTypeDefinition()))
                {
                    elementType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }

            elementType = null;
            return false;
        }

        private static bool IsSupportedGenericEnumerable(Type genericDefinition)
        {
            return genericDefinition == typeof(IEnumerable<>) ||
                   genericDefinition == typeof(ICollection<>) ||
                   genericDefinition == typeof(IList<>) ||
                   genericDefinition == typeof(IReadOnlyCollection<>) ||
                   genericDefinition == typeof(IReadOnlyList<>);
        }
    }
}

