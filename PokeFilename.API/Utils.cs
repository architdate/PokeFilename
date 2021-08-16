using PKHeX.Core;
using System;
using System.Linq;
using System.Reflection;

namespace PokeFilename.API
{
    public static class Utils
    {
        public static string[] GetPropertyList(PKM pk)
        {
            var props = pk.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return props.Select(x => x.Name).ToArray();
        }

        public static bool GetPropertyValue(this PKM pk, string prop, out string value)
        {
            value = "?";
            var p = pk.GetType().GetProperty(prop);
            if (p == null)
                return false;
            var val = p.GetValue(pk);
            value = val.ToString();
            return true;
        }

        public static Properties.PokeFilename Settings { get; set; } = Properties.PokeFilename.Default;

        public static IFileNamer<PKM> Create(string name)
        {
            var type = Type.GetType($"PokeFilename.API.{name}", throwOnError: false);
            if (type == null) return new AnubisNamer();
            return (IFileNamer<PKM>)Activator.CreateInstance(type);
        }
    }
}
