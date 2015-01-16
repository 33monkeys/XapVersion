using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Monads.NET;

namespace XapVersion
{
    public static class VersionOf
    {
        private static readonly Guid TmpUnpackedDir = Guid.Parse("F8B2CDB0-C6A1-41D5-9918-591071309816");

        public static string XapFile(string xapFileName)
        {
            try
            {
                var unpackDir = UnpackXap(xapFileName);
                var entryPointAssembly = GetEntryPointAssembly(unpackDir);
                var dllVersion = GetDllVersion(unpackDir, entryPointAssembly);

                return dllVersion;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static void CleanUnpackXap(string tempXapDir)
        {
            if (!Directory.Exists(tempXapDir))
                return;

            DeleteDirectory(tempXapDir);
        }

        private static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
                DeleteDirectory(dir);

            Directory.Delete(targetDir, false);
        }

        private static string UnpackXap(string xapFileName)
        {
            var tempXapDir = Path.Combine(Path.GetTempPath(), TmpUnpackedDir.ToString());
            CleanUnpackXap(tempXapDir);

            if (!Directory.Exists(tempXapDir))
                Directory.CreateDirectory(tempXapDir);

            ZipFile.ExtractToDirectory(xapFileName, tempXapDir);

            return tempXapDir;
        }

        private static string GetDllVersion(string unpackDir, string entryPointAssembly)
        {
            var directoryInfo = new DirectoryInfo(unpackDir);
            var xapAssembly = directoryInfo.GetFiles(entryPointAssembly).FirstOrDefault();

            var version = xapAssembly.With(x =>
            {
                var fileVersion = FileVersionInfo.GetVersionInfo(x.FullName);
                return fileVersion.ProductVersion;
            });

            return version;
        }

        private static string GetEntryPointAssembly(string unpackDir)
        {
            const string appManifest = "AppManifest.xaml";
            var manifestFile = Path.Combine(unpackDir, appManifest);

            var doc = XDocument.Load(manifestFile);
            var entryPointAssembly = doc
                .With(x => x.Root)
                .With(x => x.Attribute("EntryPointAssembly"));

            var dllName = entryPointAssembly
                .With(x => x.Value)
                .With(x => Path.ChangeExtension(x, ".dll"));

            return dllName;
        }
    }
}
