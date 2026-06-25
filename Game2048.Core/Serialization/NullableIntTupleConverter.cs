using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Game2048.Core.Serialization;

public class NullableIntTupleConverter : JsonConverter<(int, int)?>
{
    public override (int, int)? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Should be an array to read");

        reader.Read();
        int id1 = reader.GetInt32();

        reader.Read();
        int id2 = reader.GetInt32();

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray)
            throw new JsonException("Array should be 2 elements only to read");

        return (id1, id2);
    }

    public override void Write(Utf8JsonWriter writer, (int, int)? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        writer.WriteNumberValue(value.Value.Item1);
        writer.WriteNumberValue(value.Value.Item2);
        writer.WriteEndArray();
    }
}