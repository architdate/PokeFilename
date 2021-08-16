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
                    var check = obj.GetPropertyValue(p, out string? v);
                    if (check) return v;
                    return CustomExtensions.GetValue(obj, p);
                }
            );
            return finstr;
        }

        private string GetGBPKM(GBPKM gb)
        {
            var finstr = Regex.Replace(Gameboy, @"{(?<exp>[^}]+)}", match =>
                {
                    var p = match.Groups["exp"].Value;
                    var check = gb.GetPropertyValue(p, out string? v);
                    if (check) return v;
                    return CustomExtensions.GetValue(gb, p);
                }
            );
            return finstr;
        }
    }

    public static class CustomExtensions
    {
        public static string GetValue(PKM pk, string prop)
        {
            return prop switch
            {
                "ShinySymbol" => GetShinySymbol(pk),
                "CharacteristicText" => GetCharacteristicText(pk),
                "ConditionalForm" => GetConditionalForm(pk),
                "Legality" => GetLegalityStatus(pk),
                _ => "{" + prop + "}"
            };
        }

        // Extensions
        private static string GetLegalityStatus(PKM pk) => new LegalityAnalysis(pk).Valid ? "Legal" : "Illegal";
        private static string GetConditionalForm(PKM pk) => pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
        private static string GetCharacteristicText(PKM pk) => pk.Characteristic >= 0 ? Util.GetCharacteristicsList("en")[pk.Characteristic] : string.Empty;
        private static string GetShinySymbol(PKM pk) => pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int)GameVersion.GO) ? "■" : "★";
    }
}
