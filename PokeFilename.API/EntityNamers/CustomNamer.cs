using PKHeX.Core;
using System.Text.RegularExpressions;

namespace PokeFilename.API
{
    public sealed class CustomNamer : IFileNamer<PKM>
    {
        public readonly string Regular;
        public readonly string Gameboy;

        public CustomNamer(string regular, string gameboy)
        {
            Regular = regular;
            Gameboy = gameboy;
        }

        public string GetName(PKM obj)
        {
            string pattern = obj is GBPKM ? Gameboy : Regular;
            return RemapKeywords(obj, pattern);
        }

        private static string RemapKeywords(PKM pk, string input) => Regex.Replace(input, "{(?<exp>[^}]+)}", match => GetStringValue(pk, match.Groups["exp"].Value));

        private static string GetStringValue(PKM pk, string property)
        {
            var check = pk.GetPropertyValue(property, out string? text);
            if (check)
                return text!; // use ! because the above method has no annotations for NotNullWhen
            return pk.GetValue(property);
        }
    }

    public static class KeywordRemappingExtensions
    {
        public static string GetValue(this PKM pk, string prop) => prop switch
        {
            "ShinySymbol"        => GetShinySymbol(pk),
            "CharacteristicText" => GetCharacteristicText(pk),
            "ConditionalForm"    => GetConditionalForm(pk),
            "Legality"           => GetLegalityStatus(pk),
            _                    => $"{{{prop}}}"
        };

        // Extensions
        private static string GetLegalityStatus(PKM pk) => new LegalityAnalysis(pk).Valid ? "Legal" : "Illegal";
        private static string GetConditionalForm(PKM pk) => pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
        private static string GetCharacteristicText(PKM pk) => pk.Characteristic >= 0 ? Util.GetCharacteristicsList("en")[pk.Characteristic] : string.Empty;
        private static string GetShinySymbol(PKM pk) => pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int)GameVersion.GO) ? "■" : "★";
    }
}
