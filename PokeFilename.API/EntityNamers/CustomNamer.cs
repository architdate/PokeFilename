using PKHeX.Core;
using System.Text.RegularExpressions;

namespace PokeFilename.API
{
    public sealed class CustomNamer : IFileNamer<PKM>
    {
        private readonly string Regular;
        private readonly string Gameboy;
        public static readonly GameStrings Strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        public CustomNamer(string regular, string gameboy)
        {
            Regular = regular;
            Gameboy = gameboy;
        }

        public string GetName(PKM obj) => RemapKeywords(obj, obj is GBPKM ? Gameboy : Regular);
        private static string RemapKeywords(PKM pk, string input) => Regex.Replace(input, "{(?<exp>[^}]+)}", match => GetStringValue(pk, match.Groups["exp"].Value));
        private static string GetStringValue(PKM pk, string property) => pk.GetPropertyValue(property) ?? pk.GetValue(property);
    }

    public static class KeywordRemappingExtensions
    {
        public static string GetValue(this PKM pk, string prop) => prop switch
        {
            "ShinyType"        => GetShinyTypeString(pk),
            "CharacteristicText" => GetCharacteristicText(pk),
            "ConditionalForm"    => GetConditionalForm(pk),
            "Legality"           => GetLegalityStatus(pk),
            "ItemName"           => GetItemName(pk),
            _                    => $"{{{prop}}}"
        };

        // Extensions
        private static string GetLegalityStatus(PKM pk) => new LegalityAnalysis(pk).Valid ? "Legal" : "Illegal";
        private static string GetConditionalForm(PKM pk) => pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
        private static string GetCharacteristicText(PKM pk) => pk.Characteristic >= 0 ? Util.GetCharacteristicsList("en")[pk.Characteristic] : string.Empty;

        private static string GetShinyTypeString(PKM pk) { //Copied from AnubisNamer
          if (!pk.IsShiny)
            return string.Empty;
        if (pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int) GameVersion.GO))
            return " ■";
        return " ★";
      }

        private static string GetItemName(PKM pk)
        {
            if (pk.HeldItem <= 0)
                return "NoItem";
            var items = CustomNamer.Strings.GetItemStrings(pk.Format);
            if (pk.HeldItem < items.Length)
                return items[pk.HeldItem];
            return "NoItem";
        }
    }
}
