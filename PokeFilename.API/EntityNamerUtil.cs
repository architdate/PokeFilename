using System;
using System.Collections;
using PKHeX.Core;

namespace PokeFilename.API
{
    public static class EntityNamerUtil
    {
        public static string? GetPropertyValue(this PKM pk, string prop)
        {
            var formatter = GetFormatter(ref prop);
            var enumeration = GetEnumeration(ref prop);
            prop = prop.Trim();

            object? obj = pk;
            foreach (string part in prop.Split('.'))
            {
                if (obj.IsNonStringEnumerable())
                {
                    var toEnumerable = (IEnumerable)obj;
                    var iterator = toEnumerable.GetEnumerator();
                    if (!iterator.MoveNext())
                        return null;

                    obj = iterator.Current;
                }

                if (obj == null)
                    return null;

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                    return null;

                obj = info.GetValue(obj, null);
                if (obj == null)
                    return null;
            }

            if (enumeration != null)
                obj = ParseEnum(enumeration, obj);

            return obj?.CustomFormat(formatter);
        }

        private static string? GetFormatter(ref string prop)
        {
            int colon = prop.IndexOf(':');
            if (colon == -1)
                return null;

            var formatter = prop[(colon + 1)..];
            prop = prop[..colon];
            return formatter;
        }

        private static string? GetEnumeration(ref string prop)
        {
            var openParen = prop.IndexOf('(');
            if (openParen != 0)
                return null;

            int closeParen = prop.IndexOf(')');
            if (closeParen == -1)
                return null;

            var enumeration = prop[1..closeParen];
            prop = prop[(closeParen + 1)..];
            return enumeration;
        }

        private static string CustomFormat(this object obj, string? formatter)
        {
            if (formatter == null)
                return obj.ToString();
            if (obj is IFormattable f)
                return f.ToString(formatter, System.Globalization.CultureInfo.CurrentCulture);
            return obj.ToString();
        }

        private static string? ParseEnum(string enumName, object value) => GetEnumType(enumName)?.GetEnumName(value);

        private static Type? GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var qualified = assembly.FullName.Split(',')[0];
                var typeName = $"{qualified}{'.'}{enumName}";
                var type = assembly.GetType(typeName);
                if (type?.IsEnum == true)
                    return type;
            }
            return null;
        }

        private static bool IsNonStringEnumerable(this object instance) => instance.GetType().IsNonStringEnumerable();
        private static bool IsNonStringEnumerable(this Type type) => type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
}
