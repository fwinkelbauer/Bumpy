using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GlobDir;

namespace Bumpy.Util
{
    internal class FileUtil : IFileUtil
    {
        private const string _bumpyConfig = ".bumpyconfig";

        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string globPattern)
        {
            directory.ThrowIfNull(nameof(directory));
            globPattern.ThrowIfNull(nameof(globPattern));

            var path = Path.Combine(directory.FullName, globPattern);

            return Glob.GetMatches(path.Replace("\\", "/")).Select(s => new FileInfo(s));
        }

        // TODO fw lazy load?
        public FileContent ReadFile(FileInfo file)
        {
            file.ThrowIfNull(nameof(file));

            using (var reader = new StreamReader(file.FullName))
            {
                var lines = new List<string>();
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                var encoding = reader.CurrentEncoding;

                return new FileContent(file, lines, encoding);
            }
        }

        public void WriteFiles(IEnumerable<FileContent> content)
        {
            content.ThrowIfNull(nameof(content));

            foreach (var contentEntry in content)
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

            foreach (var line in lines)
            {
                if (line.StartsWith("#", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var split = line.Split('=');
                var glob = split[0].Trim();
                var regex = string.Join("=", split, 1, split.Length - 1).Trim();

                yield return new BumpyConfiguration(glob, regex);
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
            builder.AppendLine("# Usage: <glob pattern> = <regular expression>");
            builder.AppendLine("# Note that the regular expression must contain a named group 'version' which contains the actual version information");
            builder.AppendLine();
            builder.AppendLine("# Example: Searches for version information of the format a.b.c.d (e.g. 1.22.7.50) in all AssemblyInfo.cs files");
            builder.AppendLine(@"# AssemblyInfo.cs = (?<version>\d+\.\d+\.\d+\.\d+)");

            File.WriteAllText(configPath, builder.ToString());
        }
    }
}
