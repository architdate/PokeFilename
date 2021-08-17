using System.Collections.Generic;
using System.IO;
using System.Linq;
using PKHeX.Core;

namespace PokeFilename.API
{
    /// <summary>
    /// Renames <see cref="PKM"/> files using a provided <see cref="EntityFileNamer"/> (or the current naming style if none provided).
    /// </summary>
    public static class BulkRename
    {
        /// <summary>
        /// Gets a temporary file name within the same directory for a bulk rename operation.
        /// </summary>
        private static string GetTempPath(string fileName, int index) => Path.Combine(Path.GetDirectoryName(fileName), $"{index}.tmp");

        public static void RenameAll(IReadOnlyList<string> files) => RenameAll(files, EntityFileNamer.Namer);
        public static void RenameAll(IEnumerable<string> files) => RenameAll(files, EntityFileNamer.Namer);
        public static void RenameAll(IEnumerable<string> files, IFileNamer<PKM> namer) => RenameAll(files.ToArray(), namer);

        public static void RenameFolder(string path, bool deep = false) => RenameFolder(path, EntityFileNamer.Namer, deep);

        public static void RenameFolder(string path, IFileNamer<PKM> namer, bool deep = false)
        {
            var opt = deep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(path, "*.*", opt);
            RenameAll(files, namer);
        }

        public static void RenameAll(IReadOnlyList<string> files, IFileNamer<PKM> namer)
        {
            // Move to a temporary filename (avoid duplicate file names being overwritten before we remap all names)
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                File.Move(file, GetTempPath(file, i));
            }

            for (int i = 0; i < files.Count; i++)
            {
                var fileName = files[i];
                var tmp = GetTempPath(fileName, i);
                var dir = Path.GetDirectoryName(tmp);
                var fi = new FileInfo(tmp);
                if (PKX.IsPKM(fi.Length))
                {
                    var data = File.ReadAllBytes(tmp);
                    var ext = Path.GetExtension(fileName);
                    var fmt = PKX.GetPKMFormatFromExtension(ext, 6);
                    var pkm = PKMConverter.GetPKMfromBytes(data, fmt);
                    if (pkm is not null)
                        fileName = $"{GetUniqueFileName(pkm, namer)}{ext}";
                }
                File.Move(tmp, Path.Combine(dir, fileName));
            }
        }

        private static string GetUniqueFileName(PKM pk, IFileNamer<PKM> namer)
        {
            var result = namer.GetName(pk);
            result = string.Concat(result.Split(Path.GetInvalidFileNameChars()));
            if (!File.Exists(result))
                return result;

            int index = 2;
            while (true)
            {
                var differentiated = GetDuplicateFileName(result, index);
                if (!File.Exists(differentiated))
                    return differentiated;
                ++index;
            }
        }

        private static string GetDuplicateFileName(string baseName, int index)
        {
            // Insert (index) right before the file extension period.
            var period = baseName.LastIndexOf('.');
            var name = baseName[..period];
            var newExt = baseName[(period + 1)..];
            return $"{name} ({index}).{newExt}";
        }
    }
}
