using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Util
{
    internal class FileUtil : IFileUtil
    {
        private const string _bumpyConfig = ".bumpyconfig";

        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string searchPattern)
        {
            directory.ThrowIfNull(nameof(directory));
            searchPattern.ThrowIfNull(nameof(searchPattern));

            return directory.EnumerateFiles(searchPattern, SearchOption.AllDirectories);
        }

        public FileContent ReadFile(FileInfo file)
        {
            file.ThrowIfNull(nameof(file));

            return new FileContent(file, File.ReadLines(file.FullName));
        }

        public void WriteFiles(IEnumerable<FileContent> content)
        {
            foreach (var contentEntry in content.ThrowIfNull(nameof(content)))
            {
                contentEntry.ThrowIfNull(nameof(contentEntry));

                File.WriteAllLines(contentEntry.File.FullName, contentEntry.Lines, Encoding.UTF8);
            }
        }

        public IEnumerable<BumpyConfiguration> ReadConfigLazy(DirectoryInfo directory)
        {
            directory.ThrowIfNull(nameof(directory));

            var configPath = Path.Combine(directory.FullName, _bumpyConfig);
            var lines = File.ReadLines(configPath);

            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var split = line.Split('=');
                var searchPattern = split[0].Trim();
                var regularExpression = string.Join("=", split, 1, split.Length - 1).Trim();

                yield return new BumpyConfiguration(searchPattern, regularExpression);
            }
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
            builder.AppendLine("# Usage: <search pattern> = <regular expression>");
            builder.AppendLine("# Note that the regular expression must contain a named group 'version' which contains the actual version information");
            builder.AppendLine();
            builder.AppendLine("# Example: Searches for version information of the format a.b.c.d (e.g. 1.22.7.50) in all AssemblyInfo.cs files");
            builder.AppendLine(@"# AssemblyInfo.cs = (?<version>\d+\.\d+\.\d+\.\d+)");

            File.WriteAllText(configPath, builder.ToString());
        }
    }
}
