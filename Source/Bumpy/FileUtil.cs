using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Config;

namespace Bumpy
{
    public sealed class FileUtil : IFileUtil
    {
        private const string AllFilesPattern = "*";

        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, Glob glob)
        {
            return directory
                .EnumerateFiles(AllFilesPattern, SearchOption.AllDirectories)
                .Where(f => glob.IsMatch(f.ToRelativePath(directory)));
        }

        public FileContent ReadFileContent(FileInfo file, Encoding encoding)
        {
            return new FileContent(file, File.ReadLines(file.FullName, encoding), encoding);
        }

        public void WriteFileContent(FileContent fileContent)
        {
            File.WriteAllLines(fileContent.File.FullName, fileContent.Lines, fileContent.Encoding);
        }

        public bool CreateConfigFile(FileInfo configFile)
        {
            if (configFile.Exists)
            {
                return false;
            }

            File.WriteAllText(configFile.FullName, ConfigIO.NewConfigFile());

            return true;
        }

        public IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile, string profile)
        {
            var configEntries = ReadConfigFile(configFile);

            if (!profile.Equals(BumpyConfigEntry.DefaultProfile))
            {
                configEntries = configEntries.Where(c => c.Profile == profile).ToArray();

                if (!configEntries.Any())
                {
                    throw new InvalidOperationException($"Profile '{profile}' does not exist");
                }
            }

            return configEntries
                .Select(c => PostProcessor.Process(c))
                .OrderBy(c => c.Profile);
        }

        private IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile)
        {
            try
            {
                return ConfigIO.ReadConfigFile(configFile.OpenText());
            }
            catch (FileNotFoundException)
            {
                var legacyConfigFile = new FileInfo(Path.Combine(configFile.Directory.FullName, BumpyConfigEntry.LegacyConfigFile));

                // TODO fw When should I remove this?
                Console.WriteLine($"Could not find configuration file: '{configFile}'");
                Console.WriteLine($"Searching for legacy configuration file: '{legacyConfigFile}'");
                return LegacyConfigIO.ReadConfigFile(File.ReadLines(legacyConfigFile.FullName));
            }
        }
    }
}
