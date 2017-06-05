using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Glob;

namespace Bumpy.Util
{
    internal class FileUtil : IFileUtil
    {
        private const string _bumpyConfig = ".bumpyconfig";

        // TODO fw https://stackoverflow.com/questions/4385707/c-sharp-detecting-encoding-in-a-file-write-change-to-file-using-the-found-enc
        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string globPattern)
        {
            directory.ThrowIfNull(nameof(directory));
            globPattern.ThrowIfNull(nameof(globPattern));

            return directory.GlobFiles(globPattern);
        }

        public IEnumerable<string> ReadLines(FileInfo file)
        {
            file.ThrowIfNull(nameof(file));

            return File.ReadLines(file.FullName);
        }

        public void WriteLines(FileInfo file, IEnumerable<string> lines)
        {
            file.ThrowIfNull(nameof(file));
            lines.ThrowIfNull(nameof(lines));

            File.WriteAllLines(file.FullName, lines);
        }

        public IEnumerable<BumpyConfiguration> ReadConfig(DirectoryInfo directory)
        {
            directory.ThrowIfNull(nameof(directory));

            var configPath = Path.Combine(directory.FullName, _bumpyConfig);
            List<BumpyConfiguration> config = new List<BumpyConfiguration>();
            var lines = File.ReadAllLines(configPath);

            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var split = line.Split('=');
                var glob = split[0].Trim();
                var regex = string.Join("=", split, 1, split.Length - 1).Trim();
                config.Add(new BumpyConfiguration(glob, regex));
            }

            if (config.Count == 0)
            {
                throw new IOException("The configuration file does not contain any data");
            }

            return config;
        }

        public void CreateConfig(DirectoryInfo directory)
        {
            directory.ThrowIfNull(nameof(directory));

            var configPath = Path.Combine(directory.FullName, _bumpyConfig);

            if (File.Exists(configPath))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("# Configuration file for Bumpy");
            builder.AppendLine();
            builder.AppendLine("# Usage: <glob pattern> = <regular expression>");
            builder.AppendLine("# Note that the regular expression must contain a named group 'version' which contains the actual version information");
            builder.AppendLine();
            builder.AppendLine("# Example: Searches for version information of the format a.b.c.d (e.g. 1.8.3.2) in all AssemblyInfo.cs files");
            builder.AppendLine(@"# AssemblyInfo.cs = (?<version>\d+\.\d+\.\d+\.\d+)");

            File.WriteAllText(configPath, builder.ToString());
        }
    }
}
