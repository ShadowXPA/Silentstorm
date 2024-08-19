using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace CommonLib.Utils
{
    public static class JsonExtension
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static string? Serialize(this object? source, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            if (source == null) return default;

            return JsonConvert.SerializeObject(source, jsonSerializerSettings ?? JsonSerializerSettings);
        }

        public static T? Deserialize<T>(this string? json, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings ?? JsonSerializerSettings);
        }
    }
}
