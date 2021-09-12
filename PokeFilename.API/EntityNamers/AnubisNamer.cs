using PKHeX.Core;

namespace PokeFilename.API
{
    public sealed class AnubisNamer : IFileNamer<PKM>
    {
        public string GetName(PKM obj)
        {
            if (obj is GBPKM gb)
                return GetGBPKM(gb);
            return GetRegular(obj);
        }

        private static string GetRegular(PKM pk)
        {
            string form = pk.Form > 0 ? $"-{pk.Form:00}" : string.Empty;
            string shinytype = GetShinyTypeString(pk);

            string IVList = $"{pk.IV_HP}.{pk.IV_ATK}.{pk.IV_DEF}.{pk.IV_SPA}.{pk.IV_SPD}.{pk.IV_SPE}";

            string TIDFormatted = pk.Generation >= 7 ? $"{pk.TrainerID7:000000}" : $"{pk.TID:00000}";
            var balllist = GameInfo.Strings.balllist;
            string ballFormatted = pk.Ball < balllist.Length ? balllist[pk.Ball].Split(' ')[0] : "???";

            string speciesName = SpeciesName.GetSpeciesNameGeneration(pk.Species, (int)LanguageID.English, pk.Format);
            if (pk is IGigantamax { CanGigantamax: true })
                speciesName += "-Gmax";

            string OTInfo = string.IsNullOrEmpty(pk.OT_Name) ? "" : $" - {pk.OT_Name} - {TIDFormatted} - {ballFormatted}";

            return $"{pk.Species:000}{form}{shinytype} - {speciesName} - {GetNature(pk)} - {IVList}{OTInfo} - {pk.Checksum:X4}{pk.EncryptionConstant:X8}";
        }

        private static string GetNature(INature pk)
        {
            var nature = pk.Nature;
            var strings = Util.GetNaturesList("en");
            if ((uint) nature >= strings.Length)
                nature = 0;
            return strings[nature];
        }

        private static string GetShinyTypeString(PKM pk)
        {
            if (!pk.IsShiny)
                return string.Empty;
            if (pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int) GameVersion.GO))
                return " ■";
            return " ★";
        }

        private static string GetGBPKM(GBPKM gb)
        {
            string form = gb.Form > 0 ? $"-{gb.Form:00}" : string.Empty;
            string star = gb.IsShiny ? " ★" : string.Empty;

            string IVList = $"{gb.IV_HP}.{gb.IV_ATK}.{gb.IV_DEF}.{gb.IV_SPA}.{gb.IV_SPD}.{gb.IV_SPE}";
            string speciesName = SpeciesName.GetSpeciesNameGeneration(gb.Species, (int)LanguageID.English, gb.Format);
            string OTInfo = string.IsNullOrEmpty(gb.OT_Name) ? "" : $" - {gb.OT_Name} - {gb.TID:00000}";

            var raw = gb switch
            {
                PK1 pk1 => new PokeList1(pk1).Write(),
                PK2 pk2 => new PokeList2(pk2).Write(),
                _ => gb.Data
            };
            var checksum = Checksums.CRC16_CCITT(raw);
            return $"{gb.Species:000}{form}{star} - {speciesName} - {IVList}{OTInfo} - {checksum:X4}";
        }
    }
}
