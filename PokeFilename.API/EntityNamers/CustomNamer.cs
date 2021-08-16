using PKHeX.Core;
using System.Text.RegularExpressions;

namespace PokeFilename.API
{
    public sealed class CustomNamer : IFileNamer<PKM>
    {
        public static string Regular = Properties.PokeFilename.Default.RegularFormat;
        public static string Gameboy = Properties.PokeFilename.Default.GameboyFormat;
        public string GetName(PKM obj)
        {
            if (obj is GBPKM gb)
                return GetGBPKM(gb);
            return GetRegular(obj);
        }

        private string GetRegular(PKM obj)
        {
            var finstr = Regex.Replace(Regular, @"{(?<exp>[^}]+)}", match =>
                {
                    var p = match.Groups["exp"].Value;
                    var check = obj.GetPropertyValue(p, out string v);
                    if (check) return v;
                    return "{" + p + "}";
                }
            );
            return finstr;
        }

        private string GetGBPKM(GBPKM gb)
        {
            var finstr = Regex.Replace(Gameboy, @"{(?<exp>[^}]+)}", match =>
                {
                    var p = match.Groups["exp"].Value;
                    var check = gb.GetPropertyValue(p, out string v);
                    if (check) return v;
                    return "{" + p + "}";
                }
            );
            return finstr;
        }
    }
}
