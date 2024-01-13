using PKHeX.Core;
using System.Text.RegularExpressions;

namespace PokeFilename.API
{
    public sealed class CustomNamer : IFileNamer<PKM>
    {
        public string Name => "Default";

        private readonly string Regular;
        private readonly string Gameboy;
        public static readonly GameStrings Strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        public CustomNamer(string regular, string gameboy)
        {
            Regular = regular;
            Gameboy = gameboy;
        }

        public string GetName(PKM obj) => Regex.Replace(RemapKeywords(obj, obj is GBPKM ? Gameboy : Regular), @"\s+", " ");
        private static string RemapKeywords(PKM pk, string input) => Regex.Replace(input, "{(?<exp>[^}]+)}", match => GetStringValue(pk, match.Groups["exp"].Value));
        private static string GetStringValue(PKM pk, string property) => pk.GetPropertyValue(property) ?? pk.GetValue(property);
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
            "Legality"              => GetLegalityStatus(pk),
            "ItemName"              => GetItemName(pk),
            _                       => $"{{{prop}}}"
        };

        // Extensions
        private static string GetLegalityStatus(PKM pk) => new LegalityAnalysis(pk).Valid ? "Legal" : "Illegal";
        private static string GetConditionalForm(PKM pk) => pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
        private static string GetGigantamax(PKM pk) => (pk is IGigantamax g && g.CanGigantamax) ? "Gigantamax" : string.Empty;
        private static string GetConditionalGigantamax(PKM pk) => (pk is IGigantamax g && g.CanGigantamax) ? "(Gigantamax)" : string.Empty;
        private static string GetCharacteristicText(PKM pk) => pk.Characteristic >= 0 ? Util.GetCharacteristicsList("en")[pk.Characteristic] : string.Empty;

        private static string GetShinyTypeString(PKM pk) { //Copied from AnubisNamer
            if (!pk.IsShiny)
                return string.Empty;
            if (pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int) GameVersion.GO))
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
            if (pk.HeldItem < items.Length)
                return items[pk.HeldItem];
            return "NoItem";
        }
    }
}
