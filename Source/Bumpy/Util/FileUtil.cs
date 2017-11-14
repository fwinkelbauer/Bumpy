using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bumpy.Util
{
    internal class FileUtil : IFileUtil
    {
        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string searchPattern)
        {
            directory.ThrowIfNull(nameof(directory));
            searchPattern.ThrowIfNull(nameof(searchPattern));

            var glob = new GlobUtil(searchPattern);

            return directory.EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(f => glob.IsMatch(f.ToRelativePath(directory)));
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

        public IEnumerable<BumpyConfiguration> ReadConfigLazy(FileInfo configFile)
        {
            configFile.ThrowIfNull(nameof(configFile));

            var lines = File.ReadLines(configFile.FullName);
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
                    var possibleEncoding = leftSplit[1].Trim();
                    int codePage = 0;
                    bool isCodingPage = int.TryParse(possibleEncoding, out codePage);

                    if (isCodingPage)
                    {
                        encoding = Encoding.GetEncoding(codePage);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(possibleEncoding);
                    }
                }

                var searchPattern = leftSplit[0].Trim();
                var regularExpression = string.Join("=", split, 1, split.Length - 1).Trim();

                yield return new BumpyConfiguration(profile, searchPattern, regularExpression, encoding);
            }
        }

        public bool CreateConfig(FileInfo configPath)
        {
            configPath.ThrowIfNull(nameof(configPath));

            if (configPath.Exists)
            {
                return false;
            }

            var builder = new StringBuilder();
            builder.AppendLine("# Configuration file for Bumpy");
            builder.AppendLine();
            builder.AppendLine("# Usage: <file glob pattern> = <regular expression>");
            builder.AppendLine("# Note that the regular expression must contain a named group 'version' which contains the actual version information");
            builder.AppendLine();
            builder.AppendLine("# Example: Searches for version information of the format a.b.c.d (e.g. 1.22.7.50) in all AssemblyInfo.cs files");
            builder.AppendLine(@"# AssemblyInfo.cs = (?<version>\d+\.\d+\.\d+\.\d+)");
            builder.AppendLine();
            builder.AppendLine("# Example: The default read/write encoding is UTF-8 without BOM, but you can change this behaviour (e.g. UTF-8 with BOM)");
            builder.AppendLine(@"# AssemblyInfo.cs | UTF-8 = (?<version>\d+\.\d+\.\d+\.\d+)");
            builder.AppendLine();
            builder.AppendLine("# Example: Search for all .nuspec files in the NuSpec directory");
            builder.AppendLine(@"# NuSpec\**\*.nuspec = <version>(?<version>\d+(\.\d+)+)");

            File.WriteAllText(configPath.FullName, builder.ToString());

            return true;
        }
    }
}
