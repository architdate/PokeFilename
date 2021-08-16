# PokeFilename
PKHeX-Plugin to customize the PKM output filename

# About  
This project uses `PKHeX.Core` and PKHeX's `IPlugin` interface to customize the file name of exported PKM files. Please refer to the [Wiki](https://github.com/architdate/PokeFilename/wiki) for more information regarding the functionalities provided by this project.

This project is owned by [@architdate](https://github.com/architdate) (Discord: thecommondude#8240)

## CI/CD Builds
This project has Azure CI/CD setup to auto build commits on every commit. People who are not keen on building the project themselves, may use the CI/CD to download the built plugin.

## Building  
This project requires an IDE that supports compiling .NET based code (Ideally .NET 4.6+). Recommended IDE is Visual Studio 2019.

**Regular Builds**  
Regular builds will usually succeed unless there are changes that are incompatible with the NuGet [PKHeX.Core](https://www.nuget.org/packages/PKHeX.Core) package dependency specified in the `.csproj` files of the projects.

- Clone the PokeFilename repository using: `$ git clone https://github.com/architdate/PokeFilename.git`.
- Right-click on the solution and click `Rebuild All`.
- These DLLs should be placed into a `plugins` directory where the PKHeX executable is. You may also combine these DLL files using ILMerge.
   - The compiled DLLs for PokeFilename will be in the `PokeFilename.GUI/bin/Release/net46` directory:
     * PokeFilename.GUI.dll
     * PokeFilename.API.dll

## Usage  
To use the plugins:
- Create a folder named `plugins` in the same directory as PKHeX.exe.
- Put the compiled plugins from this project in the `plugins` folder. 
- Start PKHeX.exe.
- The plugins should be available for use in `Tools > PokeFilename` drop-down menu.

## Support Server
Come join the dedicated Discord server for this mod! Ask questions, give suggestions, get help, or just hang out. Don't be shy, we don't bite:

[<img src="https://canary.discordapp.com/api/guilds/401014193211441153/widget.png?style=banner2">](https://discord.gg/tDMvSRv)

## Contributing

To contribute to the repository, you can submit a pull request to the repository. Try to follow a format similar to the current codebase. All contributions are greatly appreciated! If you would like to discuss possible contributions without using GitHub, please contact us on the support server above.

## Credits
**Repository Owners**
- [architdate (thecommondude)](https://github.com/architdate)

**Namer Credits**

| Namer | Author |
| --- | --- |
| [@architdate](https://github.com/architdate) | Creator of CustomNamer Template |
| [@Lusamine](https://github.com/Lusamine) | Creator of AnubisNamer Template |

Feel free to contribute complex Namer templates by opening a Pull Request through GitHub issues!

**Credit must be given where due...**

- [FlatIcon](https://www.flaticon.com/) for their icons. Author credits (Vitaly Gorbachev).
