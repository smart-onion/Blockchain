using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockChain
{
    public interface ISendable
    {

    }

    public class ISendableConverter : JsonConverter<ISendable>
    {
        public override ISendable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
            var typeProperty = jsonObject.GetProperty("Type").GetString();

            ISendable? sendable = typeProperty switch
            {
                nameof(Blockchain) => JsonSerializer.Deserialize<Blockchain>(jsonObject.GetRawText(), options),
                nameof(Transaction) => JsonSerializer.Deserialize<Transaction>(jsonObject.GetRawText(), options),
                nameof(WsAddresses) => JsonSerializer.Deserialize<WsAddresses>(jsonObject.GetRawText(), options),
                _ => throw new NotSupportedException($"Type {typeProperty} is not supported")
            };

            return sendable;
        }

        public override void Write(Utf8JsonWriter writer, ISendable value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Type", value.GetType().Name);

            var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(prop => prop.CanRead && prop.GetMethod != null);

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var propValue = prop.GetValue(value);
                writer.WritePropertyName(propName);
                JsonSerializer.Serialize(writer, propValue, prop.PropertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}

