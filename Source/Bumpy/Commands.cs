using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Util;
using Bumpy.Version;

namespace Bumpy
{
    public class Commands
    {
        private readonly IFileUtil _fileUtil;
        private readonly DirectoryInfo _directory;
        private readonly Action<string> _writeLine;

        public Commands(DirectoryInfo directory, IFileUtil fileUtil, Action<string> writeLine)
        {
            _directory = directory.ThrowIfNull(nameof(directory));
            _fileUtil = fileUtil.ThrowIfNull(nameof(fileUtil));
            _writeLine = writeLine.ThrowIfNull(nameof(writeLine));
        }

        public void CommandList(IEnumerable<BumpyConfiguration> config)
        {
            foreach (var configEntry in config.ThrowIfNull(nameof(config)))
            {
                PerformOnContent(configEntry, (content, line, i, readVersion) =>
                {
                    if (readVersion != null)
                    {
                        _writeLine($"{ToRelativePath(content.File)} ({i}): {readVersion}");
                    }
                });
            }
        }

        public void CommandIncrement(IEnumerable<BumpyConfiguration> config, int position)
        {
            WriteContent(config, version => version.Increment(position));
        }

        public void CommandAssign(IEnumerable<BumpyConfiguration> config, int position, int number)
        {
            WriteContent(config, version => version.Assign(position, number));
        }

        public void CommandWrite(IEnumerable<BumpyConfiguration> config, string versionText)
        {
            var newVersion = VersionHelper.ParseVersionFromText(versionText);

            WriteContent(config, version => newVersion);
        }

        public void CommandCreateConfig()
        {
            _fileUtil.CreateConfig(_directory);
        }

        public void CommandPrintHelp()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Bumpy is a tool to maintain version information accross multiple files found in the current working directory");
            builder.AppendLine();
            builder.AppendLine("Usage:");
            builder.AppendLine("  -l");
            builder.AppendLine("    Lists all versions");
            builder.AppendLine("  -c");
            builder.AppendLine("    Creates a .bumpyconfig file if it does not exist");
            builder.AppendLine("  -i <one-based index number>");
            builder.AppendLine("    Increments the specified component of each version");
            builder.AppendLine("  -w <version string>");
            builder.AppendLine("    Overwrites a version with another version");
            builder.AppendLine("  -a <one-based index number> <version number>");
            builder.AppendLine("    Replaces the specified component of a version with a new number");

            _writeLine(builder.ToString());
        }

        public void CommandPrintProfiles(IEnumerable<BumpyConfiguration> config)
        {
            var profiles = config.Select(c => c.Profile).Distinct();

            foreach (var profile in profiles)
            {
                _writeLine(profile);
            }
        }

        private void WriteContent(IEnumerable<BumpyConfiguration> config, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            List<FileContent> newContent = new List<FileContent>();

            foreach (var configEntry in config.ThrowIfNull(nameof(config)))
            {
                newContent.AddRange(TransformContent(configEntry, transformFunction));
                _fileUtil.WriteFiles(newContent);
            }
        }

        private IEnumerable<FileContent> TransformContent(BumpyConfiguration config, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            var newLinesPerFile = new Dictionary<FileContent, List<string>>();

            PerformOnContent(config, (content, line, i, readVersion) =>
            {
                var newLine = line;

                if (readVersion != null)
                {
                    var newVersion = transformFunction(readVersion);
                    newLine = VersionHelper.ReplaceVersionInText(line, config.RegularExpression, newVersion);
                    _writeLine($"{ToRelativePath(content.File)} ({i}): {readVersion} -> {newVersion}");
                }

                if (!newLinesPerFile.ContainsKey(content))
                {
                    newLinesPerFile.Add(content, new List<string>());
                }

                newLinesPerFile[content].Add(newLine);
            });

            return newLinesPerFile.Select(dict => new FileContent(dict.Key.File, dict.Value, dict.Key.Encoding));
        }

        private void PerformOnContent(BumpyConfiguration config, Action<FileContent, string, int, BumpyVersion> versionInLineAction)
        {
            var files = _fileUtil.GetFiles(_directory, config.SearchPattern);
            var contents = files.Select(file => _fileUtil.ReadFile(file, config.Encoding));

            foreach (var content in contents)
            {
                int i = 0;

                foreach (var line in content.Lines)
                {
                    var readVersion = VersionHelper.FindVersion(line, config.RegularExpression);
                    versionInLineAction(content, line, i, readVersion);
                    i++;
                }
            }
        }

        private string ToRelativePath(FileInfo file)
        {
            return file.FullName.Substring(_directory.FullName.Length);
        }
    }
}
