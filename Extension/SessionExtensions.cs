using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ASP_MongoDB.Extension
{
    public static class SessionExtensions
    {
        public static void SetJson<T>(this ISession session, string key, T value)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            session.SetString(key, JsonSerializer.Serialize(value, options));
        }

        public static T? GetJson<T>(this ISession session, string key)
        {
            var sessionData = session.GetString(key);
            if (sessionData == null) return default;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            return JsonSerializer.Deserialize<T>(sessionData, options);
        }

    }
}
