using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Core.Config;

namespace Bumpy.Core
{
    /// <summary>
    /// An interface implementation which uses the file system.
    /// </summary>
    public sealed class FileUtil : IFileUtil
    {
        private const string AllFilesPattern = "*";

        /// <inheritdoc/>
        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, Glob glob)
        {
            return directory
                .EnumerateFiles(AllFilesPattern, SearchOption.AllDirectories)
                .Where(f => glob.IsMatch(f.ToRelativePath(directory)));
        }

        /// <inheritdoc/>
        public FileContent ReadFileContent(FileInfo file, Encoding encoding)
        {
            return new FileContent(file, File.ReadLines(file.FullName, encoding), encoding);
        }

        /// <inheritdoc/>
        public void WriteFileContent(FileContent fileContent)
        {
            File.WriteAllLines(fileContent.File.FullName, fileContent.Lines, fileContent.Encoding);
        }

        /// <inheritdoc/>
        public bool CreateConfigFile(FileInfo configFile)
        {
            if (configFile.Exists)
            {
                return false;
            }

            File.WriteAllText(configFile.FullName, ConfigIO.NewConfigFile());

            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile, string profile)
        {
            var configEntries = ReadConfigFile(configFile);

            if (!profile.Equals(BumpyConfigEntry.DefaultProfile))
            {
                configEntries = configEntries.Where(c => c.Profile == profile).ToList();

                if (!configEntries.Any())
                {
                    throw new InvalidOperationException($"Profile '{profile}' does not exist");
                }
            }

            // OrderBy is used to improve the formatting of all Bumpy commands
            return configEntries
                .Select(ConfigProcessor.Process)
                .OrderBy(c => c.Profile);
        }

        private IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile)
        {
            var lines = File.ReadLines(configFile.FullName).ToList();

            try
            {
                return ConfigIO.ReadConfigFile(lines);
            }
            catch (ConfigException e)
            {
                // TODO fw When should I remove this?
                // If this is kept for a long period (e.g. using a legacy flag) consider a
                // decorator pattern for `ConfigIO`
                Console.WriteLine($"Could not parse '{configFile.Name}'. Error: {e.Message}");
                Console.WriteLine("Trying legacy configuration format (prior to Bumpy 0.11.0)");
                return LegacyConfigIO.ReadConfigFile(lines);
            }
        }
    }
}
