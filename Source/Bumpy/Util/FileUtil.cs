using System;
using System.Collections.Generic;
using System.IO;
using Glob;

namespace Bumpy.Util
{
    internal class FileUtil : IFileUtil
    {
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

        public IEnumerable<Tuple<string, string>> ReadConfig(DirectoryInfo directory)
        {
            // TODO validation file contains data, ...
            var configPath = Path.Combine(directory.FullName, ".bumpyconfig");
            List<Tuple<string, string>> config = new List<Tuple<string, string>>();
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
                config.Add(new Tuple<string, string>(glob, regex));
            }

            return config;
        }
    }
}
