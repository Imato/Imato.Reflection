using FastMember;
using System.Text;

namespace Imato.Reflection
{
    public static class Objects
    {
        public static IDictionary<string, object?> GetFields<T>(this T obj)
        {
            return obj.GetFieldsList();
        }

        private static IDictionary<string, object?> GetFieldsList<T>(this T obj, int level = 0)
        {
            var dic = new Dictionary<string, object?>();
            var type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            foreach (var m in accessor.GetMembers())
            {
                if (m.CanRead)
                {
                    var value = accessor[obj, m.Name];
                    if (m.Type.IsValueType || m.Type == typeof(string) || value is null)
                    {
                        dic.Add(m.Name, value);
                    }
                    else
                    {
                        if (level < 10)
                            foreach (var n in value.GetFieldsList(level++))
                            {
                                dic.Add($"{m.Name}.{n.Key}", n.Value);
                            }
                    }
                }
            }
            return dic;
        }

        public static IDictionary<string, object?> GetDiff<T>(T obj1, T obj2)
        {
            var dic1 = obj1.GetFields();
            var dic2 = obj2.GetFields();
            var dif = new Dictionary<string, object?>();
            foreach (var d in dic1)
            {
                if (!(d.Value is null || dic2[d.Key] is null)
                    && !d.Value.Equals(dic2[d.Key]))
                {
                    dif.Add(d.Key, d.Value);
                }
                if (d.Value is null && !(dic2[d.Key] is null))
                {
                    dif.Add(d.Key, null);
                }
                if (!(d.Value is null) && dic2[d.Key] is null)
                {
                    dif.Add(d.Key, d.Value);
                }
            }
            return dif;
        }

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