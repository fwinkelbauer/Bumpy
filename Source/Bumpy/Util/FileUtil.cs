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

        public FileContent ReadFile(FileInfo file, Encoding encoding)
        {
            file.ThrowIfNull(nameof(file));

            return new FileContent(file, File.ReadLines(file.FullName, encoding), encoding);
        }

        public void WriteFiles(IEnumerable<FileContent> content)
        {
            foreach (var contentEntry in content.ThrowIfNull(nameof(content)))
            {
                contentEntry.ThrowIfNull(nameof(contentEntry));
                File.WriteAllLines(contentEntry.File.FullName, contentEntry.Lines, contentEntry.Encoding);
            }
        }

        public IEnumerable<BumpyConfiguration> ReadConfigLazy(DirectoryInfo directory)
        {
            directory.ThrowIfNull(nameof(directory));

            var configPath = Path.Combine(directory.FullName, _bumpyConfig);
            var lines = File.ReadLines(configPath);
            var profile = BumpyConfiguration.DefaultProfile;

            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (line.StartsWith("[", StringComparison.Ordinal) && line.EndsWith("]", StringComparison.Ordinal))
                {
                    profile = line.Replace("[", string.Empty).Replace("]", string.Empty).Trim();
                    continue;
                }

                var split = line.Split('=');
                var leftSplit = split[0].Split('|');
                Encoding encoding = new UTF8Encoding(false);

                if (leftSplit.Length == 2)
                {
                    encoding = Encoding.GetEncoding(leftSplit[1].Trim());
                }

                var searchPattern = leftSplit[0].Trim();
                var regularExpression = string.Join("=", split, 1, split.Length - 1).Trim();

                yield return new BumpyConfiguration(profile, searchPattern, regularExpression, encoding);
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
            builder.AppendLine();
            builder.AppendLine("# Example: The default read/write encoding is UTF-8 without BOM, but you can change this behaviour (e.g. UTF-8 with BOM)");
            builder.AppendLine(@"# AssemblyInfo.cs | UTF-8 = (?<version>\d+\.\d+\.\d+\.\d+)");

            File.WriteAllText(configPath, builder.ToString());
        }
    }
}
