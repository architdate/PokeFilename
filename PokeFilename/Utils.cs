using PKHeX.Core;
using System.Linq;
using System.Reflection;

namespace PokeFilename
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
    }
}
