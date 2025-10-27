using PKHeX.Core;
using System.Text.RegularExpressions;

namespace PokeFilename.API
{
    public sealed partial class CustomNamer(string regular, string gameboy) : IFileNamer<PKM>
    {
        public string Name => "Default";

        public static readonly GameStrings Strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        public string GetName(PKM obj) => NameRegex().Replace(RemapKeywords(obj, obj is GBPKM ? gameboy : regular), " ");
        private static string RemapKeywords(PKM pk, string input) => RemapRegex().Replace(input, match => GetStringValue(pk, match.Groups["exp"].Value));
        private static string GetStringValue(PKM pk, string property) => pk.GetPropertyValue(property) ?? pk.GetValue(property);
        [GeneratedRegex(@"\s+")]
        private static partial Regex NameRegex();
        [GeneratedRegex("{(?<exp>[^}]+)}")]
        private static partial Regex RemapRegex();
    }

    public static class KeywordRemappingExtensions
    {
        public static string GetValue(this PKM pk, string prop) => prop switch
        {
            "ShinyType"             => GetShinyTypeString(pk),
            "CharacteristicText"    => GetCharacteristicText(pk),
            "ConditionalForm"       => GetConditionalForm(pk),
            "FormName"              => GetFormName(pk),
            "ConditionalFormName"   => GetConditionalFormName(pk),
            "Gigantamax"            => GetGigantamax(pk),
            "ConditionalGigantamax" => GetConditionalGigantamax(pk),
            "Alpha"                 => GetAlpha(pk),
            "ConditionalAlpha"      => GetConditionalAlpha(pk),
            "Legality"              => GetLegalityStatus(pk),
            "ItemName"              => GetItemName(pk),
            _                       => $"{{{prop}}}"
        };

        // Extensions
        private static string GetLegalityStatus(PKM pk) => new LegalityAnalysis(pk).Valid ? "Legal" : "Illegal";
        private static string GetConditionalForm(PKM pk) => pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
        private static string GetGigantamax(PKM pk) => pk is IGigantamax { CanGigantamax: true } ? "Gigantamax" : string.Empty;
        private static string GetConditionalGigantamax(PKM pk) => pk is IGigantamax { CanGigantamax: true } ? "(Gigantamax)" : string.Empty;
        private static string GetAlpha(PKM pk) => pk is IAlpha { IsAlpha: true } ? "α" : string.Empty;
        private static string GetConditionalAlpha(PKM pk) => pk is IAlpha { IsAlpha: true } ? "(α)" : string.Empty;
        private static string GetCharacteristicText(PKM pk) => pk.Characteristic >= 0 ? Util.GetCharacteristicsList("en")[pk.Characteristic] : string.Empty;

        private static string GetShinyTypeString(PKM pk) // Copied from AnubisNamer
        {
            if (!pk.IsShiny)
                return string.Empty;
            if (pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == GameVersion.GO))
                return " ■";
            return " ★";
        }

        private static string GetFormName(PKM pk) {
            var Strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);
            string FormString = ShowdownParsing.GetStringFromForm(pk.Form, Strings, pk.Species, pk.Context);
            string FormName = ShowdownParsing.GetShowdownFormName(pk.Species, FormString);
            return FormName;
        }
        private static string GetConditionalFormName(PKM pk) {
            string formName = GetFormName(pk);
            return string.IsNullOrEmpty(formName) ? string.Empty : $"({formName})";
        }

        private static string GetItemName(PKM pk)
        {
            if (pk.HeldItem <= 0)
                return "NoItem";
            var items = CustomNamer.Strings.GetItemStrings(pk.Context);
            return pk.HeldItem < items.Length ? items[pk.HeldItem] : "NoItem";
        }
    }
}
