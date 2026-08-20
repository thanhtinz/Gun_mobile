using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnityEngine
{
    public static class JsonUtility
    {
        static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        static readonly JsonSerializerOptions PrettyOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        public static string ToJson(object obj, bool prettyPrint)
        {
            if (obj == null)
            {
                return "{}";
            }

            return JsonSerializer.Serialize(obj, prettyPrint ? PrettyOptions : Options);
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, Options);
        }
    }
}
