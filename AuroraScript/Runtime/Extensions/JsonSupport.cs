using AuroraScript.Core;
using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;


namespace AuroraScript.Runtime.Extensions
{
    internal class JsonSupport : ScriptObject
    {

        public JsonSupport()
        {
            Define("parse", new BondingFunction(PARSE), writeable: false, enumerable: false);
            Define("stringify", new BondingFunction(STRINGIFY), writeable: false, enumerable: false);
        }

        public static void PARSE(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            try
            {
                if (args.TryGetString(0, out var jsonText))
                {
                    using var document = JsonDocument.Parse(jsonText);
                    result = ScriptDatum.FromObject(ConvertElement(document.RootElement));
                }
            }
            catch (JsonException ex)
            {
                throw new AuroraRuntimeException($"JSON.parse error: {ex.Message}");
            }
        }

        public static void STRINGIFY(ExecuteContext context, ScriptObject thisObject, Span<ScriptDatum> args, ref ScriptDatum result)
        {
            args.TryGetBoolean(1, out var indented);
            if (args.TryGet(0, out var datum))
            {
                result = ScriptDatum.FromObject(Serialize(datum, indented));
                return;
            }
            throw new AuroraRuntimeException($"JSON.stringify error.");
        }

        public static StringValue Serialize(ScriptDatum datum, Boolean indented = false)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using var jsonWriter = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { Indented = indented });
            var visited = new HashSet<ScriptObject>(ReferenceComparer.Instance);
            WriteDatum(jsonWriter, datum, visited);
            jsonWriter.Flush();
            return StringValue.Of(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
        }




        private static ScriptObject ConvertElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var obj = new ScriptObject();
                    foreach (var property in element.EnumerateObject())
                    {
                        obj.Define(property.Name, ConvertElement(property.Value));
                    }
                    return obj;
                case JsonValueKind.Array:
                    var array = new ScriptArray(element.GetArrayLength());
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        array.Set(index++, ScriptDatum.FromObject(ConvertElement(item)));
                    }
                    return array;
                case JsonValueKind.String:
                    return StringValue.Of(element.GetString());
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longValue))
                    {
                        return NumberValue.Of(longValue);
                    }
                    if (element.TryGetDouble(out var doubleValue))
                    {
                        return NumberValue.Of(doubleValue);
                    }
                    return NumberValue.Zero;
                case JsonValueKind.True:
                    return BooleanValue.True;
                case JsonValueKind.False:
                    return BooleanValue.False;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return Null;
            }
        }

        private static void WriteDatum(Utf8JsonWriter writer, ScriptDatum datum, HashSet<ScriptObject> visited)
        {
            switch (datum.Kind)
            {
                case ValueKind.Null:
                    writer.WriteNullValue();
                    return;
                case ValueKind.Boolean:
                    writer.WriteBooleanValue(datum.Boolean);
                    return;
                case ValueKind.Number:
                    if (double.IsNaN(datum.Number) || double.IsInfinity(datum.Number))
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteNumberValue(datum.Number);
                    }
                    return;
                case ValueKind.String:
                    writer.WriteStringValue(datum.String.Value ?? string.Empty);
                    return;
                case ValueKind.Regex:
                case ValueKind.Array:
                case ValueKind.Object:
                case ValueKind.Function:
                case ValueKind.ClrType:
                case ValueKind.ClrFunction:
                case ValueKind.ClrBonding:
                    WriteScriptObject(writer, datum.ToObject(), visited);
                    return;
                default:
                    writer.WriteStringValue(datum.ToString());
                    return;
            }
        }

        private static void WriteScriptObject(Utf8JsonWriter writer, ScriptObject value, HashSet<ScriptObject> visited)
        {
            if (value == null || value == Null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value is ScriptGlobal scriptGlobal)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (value is StringValue stringValue)
            {
                writer.WriteStringValue(stringValue.Value);
                return;
            }

            if (value is NumberValue numberValue)
            {
                var doubleValue = numberValue.DoubleValue;
                if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteNumberValue(doubleValue);
                }
                return;
            }

            if (value is BooleanValue booleanValue)
            {
                writer.WriteBooleanValue(booleanValue.Value);
                return;
            }

            if (value is ScriptArray scriptArray)
            {
                WriteArray(writer, scriptArray, visited);
                return;
            }
            if (value is ClrInstanceObject clrInstanceObject)
            {
                WriteClrInstanceObject(writer, clrInstanceObject, visited);
                return;
            }

            if (!visited.Add(value))
            {
                throw new AuroraRuntimeException("JSON.stringify cannot serialize circular references");
            }

            writer.WriteStartObject();
            var keys = value.GetKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                var propertyName = keys.Get(i).String.Value;
                var propertyValue = value.GetPropertyValue(propertyName);
                writer.WritePropertyName(propertyName);
                WriteDatum(writer, ScriptDatum.FromObject(propertyValue), visited);
            }
            writer.WriteEndObject();

            visited.Remove(value);
        }

        private static void WriteClrInstanceObject(Utf8JsonWriter writer, ClrInstanceObject clrInstanceObject, HashSet<ScriptObject> visited)
        {
            if (clrInstanceObject == null)
            {
                writer.WriteNullValue();
                return;
            }
            writer.WriteStartObject();
            // TODO clrInstanceObject.Instance
            writer.WriteEndObject();
        }





        private static void WriteArray(Utf8JsonWriter writer, ScriptArray array, HashSet<ScriptObject> visited)
        {
            if (array == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (!visited.Add(array))
            {
                throw new AuroraRuntimeException("JSON.stringify cannot serialize circular references");
            }

            writer.WriteStartArray();
            for (int i = 0; i < array.Length; i++)
            {
                WriteDatum(writer, array.Get(i), visited);
            }
            writer.WriteEndArray();

            visited.Remove(array);
        }


        private sealed class ReferenceComparer : IEqualityComparer<ScriptObject>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();

            public bool Equals(ScriptObject x, ScriptObject y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(ScriptObject obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }
    }
}
