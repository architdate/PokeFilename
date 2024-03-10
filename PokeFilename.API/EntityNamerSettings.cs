using System.ComponentModel;
using PKHeX.Core;

namespace PokeFilename.API
{
    public class EntityNamerSettings
    {
        [Category("Entity Namer Selection"), Description("Select an entity namer preset")]
        public EntityNamers Namer { get; set; } = EntityNamers.GengarNamer;

        [Category("CustomNamer Settings"), Description("PKM Name format for Regular Pokemon")]
        public string CustomPatternRegular { get; set; } = "{Species} - {Nickname} - {PID}";

        [Category("CustomNamer Settings"), Description("PKM Name format for Gameboy Pokemon")]
        public string CustomPatternGameBoy { get; set; } = "{Species} - {Nickname} - {DV16}";

        public IFileNamer<PKM> Create() => Namer switch
        {
            EntityNamers.AnubisNamer => new AnubisNamer(),
            EntityNamers.GengarNamer => new GengarNamer(),
            EntityNamers.CustomNamer => new CustomNamer(CustomPatternRegular, CustomPatternGameBoy),
            _ => new DefaultEntityNamer(),
        };
    }
}
