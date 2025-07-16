#nullable disable
namespace OneID.Shared.Tools
{
    using Newtonsoft.Json;
    using OneID.Domain.ValueObjects;

    public class TypeUserAccountNewtonsoftConverter : JsonConverter<TypeUserAccount>
    {
        public override TypeUserAccount ReadJson(JsonReader reader, Type objectType, TypeUserAccount existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value!;
                return TypeUserAccount.From(value);
            }

            throw new JsonSerializationException($"Tipo inválido para TypeUserAccount. Esperado string, recebido: {reader.TokenType} com valor {reader.Value}");
        }

        public override void WriteJson(JsonWriter writer, TypeUserAccount value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.Value);
        }
    }

}
