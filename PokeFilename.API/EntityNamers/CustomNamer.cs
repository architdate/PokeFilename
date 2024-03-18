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
            "ConditionalScale"      => GetConditionalScale(pk),
            "ConditionalTeraType"   => GetConditionalTeraType(pk),
            "Alpha"                 => GetAlpha(pk),
            "ConditionalAlpha"      => GetConditionalAlpha(pk),
            "ConditionalGender"     => GetConditionalGender(pk),
            "ConditionalNickname"   => GetConditionalNickname(pk),
            "ConditionalStatNature" => GetConditionalStatNature(pk),
            "PaddedSID"             => GetPaddedSID(pk),
            "PaddedTID"             => GetPaddedTID(pk),
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
            if (pk.HeldItem < items.Length)
                return items[pk.HeldItem];
            return "NoItem";
        }

        private static string GetConditionalScale(PKM pk)
        {
            if (pk is IScaledSize3 s)
                return $"- {s.Scale}";
            if (pk is IScaledSize h)
                return $"- {h.HeightScalar}";
            return string.Empty;
        }

        private static string GetConditionalTeraType(PKM pk)
        {
            if (pk is not ITeraType t)
                return string.Empty;
            var type = t.GetTeraType();
            var type_str = ((byte) type == TeraTypeUtil.Stellar) ? "Stellar" : type.ToString();
            return $"- Tera {type_str}";
        }

        private static string GetAlpha(PKM pk) => (pk is IAlpha a && a.IsAlpha) ? "Alpha" : string.Empty;
        private static string GetConditionalAlpha(PKM pk) => (pk is IAlpha a && a.IsAlpha) ? "(Alpha)" : string.Empty;

        private static string GetConditionalGender(PKM pk) => ((Gender) pk.Gender is Gender.Male or Gender.Female) ? GameInfo.GenderSymbolUnicode[pk.Gender] : string.Empty;

        private static string GetConditionalNickname(PKM pk) => pk.IsNicknamed ? $"[{pk.Nickname}]" : string.Empty;

        private static string GetConditionalStatNature(PKM pk) => (pk.Nature != pk.StatNature) ? $"➔{pk.StatNature}" : string.Empty;

        private static string GetPaddedSID(PKM pk) => pk.TrainerIDDisplayFormat == TrainerIDFormat.SixDigit ? $"{pk.DisplaySID:0000}" : $"{pk.SID16:00000}";
        private static string GetPaddedTID(PKM pk) => pk.TrainerIDDisplayFormat == TrainerIDFormat.SixDigit ? $"{pk.DisplayTID:000000}" : $"{pk.TID16:00000}";
    }
}
