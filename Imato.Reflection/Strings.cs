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
        public static string ToCsv(
            this IDictionary<string, object?> dic,
            bool withNames = true)
        {
            var sb = new StringBuilder();
            var first = true;
            var done = false;
            foreach (var d in dic)
            {
                done = false;
                if (!first)
                {
                    sb.Append(";");
                }
                else
                {
                    first = false;
                }

                if (withNames)
                {
                    sb.Append(@"""");
                    sb.Append(d.Key);
                    sb.Append(@"""");
                    sb.Append(":");
                }

                if (d.Value == null)
                {
                    sb.Append("null");
                    done = true;
                }

                if (!done && d.Value is DateTime)
                {
                    sb.Append(@"""");
                    sb.Append(((DateTime)d.Value).ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    sb.Append(@"""");
                    done = true;
                }

                if (!done && d.Value is ValueType && d.Value is not string)
                {
                    sb.Append(d.Value?.ToString() ?? "null");
                    done = true;
                }

                if (!done)
                {
                    sb.Append(@"""");
                    sb.Append(d.Value?.ToString() ?? "null");
                    sb.Append(@"""");
                }
            }
            return sb.ToString();
        }

        public static string ToCsvString(object? o)
        {
            if (o == null)
            {
                return "null";
            }

            if (o is DateTime)
            {
                return $"\"{(DateTime)o:yyyy-MM-ddTHH:mm:ss.fff}\"";
            }

            if (o is ValueType && o is not string)
            {
                return o?.ToString() ?? "null";
            }

            return $"\"{o}\"";
        }

        public static string ToCsv<T>(
            this T obj,
            bool withNames = true)
        {
            if (obj == null)
            {
                return "null";
            }

            var dic = (obj as IDictionary<string, object>)
                ?? obj.GetFields();
            if (dic != null)
            {
                return dic.ToCsv(withNames);
            }

            return "null";
        }
    }
}