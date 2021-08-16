using PKHeX.Core;
using System;
using System.Collections;
using System.Linq;

namespace PokeFilename.API
{
    public static class EntityNamerUtil
    {
        public static bool GetPropertyValue(this PKM pk, string prop, out string? value)
        {
            value = null;
            string? formatter = null;
            string? enumeration = null;
            if (prop.Contains(':'))
            {
                formatter = prop.Split(':')[1];
                prop = prop.Split(':')[0];
            }
            if (prop.StartsWith("(") && prop.Contains(')'))
            {
                enumeration = prop.Split(')')[0].Split('(')[1];
                prop = prop.Split(new[] { ')' }, count: 2)[1];
            }

            prop = prop.Trim();
            object? obj = pk;
            foreach (string part in prop.Split('.'))
            {
                if (obj == null)
                    return false;

                if (obj.IsNonStringEnumerable())
                {
                    var toEnumerable = (IEnumerable)obj;
                    var iterator = toEnumerable.GetEnumerator();
                    if (!iterator.MoveNext())
                        return false;

                    obj = iterator.Current;
                }

                if (obj == null)
                    return false;

                Type type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                    return false;

                obj = info.GetValue(obj, null);
            }

            if (enumeration != null)
                obj = ParseEnum(enumeration, obj);
            if (obj == null)
                return false;

            value = obj.CustomFormat(formatter);
            return true;
        }

        private static string CustomFormat(this object obj, string? formatter)
        {
            if (formatter == null)
                return obj.ToString();

            var type = obj.GetType();
            return Type.GetTypeCode(type) switch
            {
                TypeCode.SByte => ((sbyte)obj).ToString(formatter),
                TypeCode.Byte => ((byte)obj).ToString(formatter),
                TypeCode.Int16 => ((short)obj).ToString(formatter),
                TypeCode.UInt16 => ((ushort)obj).ToString(formatter),
                TypeCode.Int32 => ((int)obj).ToString(formatter),
                TypeCode.UInt32 => ((uint)obj).ToString(formatter),
                TypeCode.Int64 => ((long)obj).ToString(formatter),
                TypeCode.UInt64 => ((ulong)obj).ToString(formatter),
                _ => obj.ToString()
            };
        }

        private static string? ParseEnum(string enumName, object value)
        {
            Type? type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.FullName.Split(',')[0];
                type = assembly.GetType(name + '.' + enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    break;
            }
            if (type is not { IsEnum: true })
                return null;
            return type.GetEnumName(value);
        }

        private static bool IsNonStringEnumerable(this object? instance) => instance != null && instance.GetType().IsNonStringEnumerable();

        private static bool IsNonStringEnumerable(this Type? type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}
