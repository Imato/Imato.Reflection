using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Imato.Reflection
{
    public static class Strings
    {
        private static JsonSerializerOptions _jOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        /// <summary>
        /// Serialize object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize(value, _jOptions);
        }

        /// <summary>
        /// Get dictionary string representation
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string ToCsvString(this IDictionary<string, object?> dic)
        {
            var sb = new StringBuilder();
            var first = true;
            foreach (var d in dic)
            {
                if (!first)
                {
                    sb.Append("; ");
                }
                else
                {
                    first = false;
                }
                sb.Append(@"""");
                sb.Append(d.Key);
                sb.Append(@"""");
                sb.Append(": ");
                sb.Append(@"""");
                if (d.Value is DateTime)
                {
                    sb.Append(((DateTime)d.Value).ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                }
                else
                {
                    sb.Append(d.Value?.ToString() ?? "null");
                }
                sb.Append(@"""");
            }
            return sb.ToString();
        }
    }
}