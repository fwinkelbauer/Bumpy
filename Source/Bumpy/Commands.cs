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
        // TODO fw test, refactor and simplify this class
        private readonly IEnumerable<BumpyConfiguration> _config;
        private readonly IFileUtil _fileUtil;
        private readonly DirectoryInfo _directory;
        private readonly Action<string> _writeLine;

        public Commands(IEnumerable<BumpyConfiguration> config, DirectoryInfo directory, IFileUtil fileUtil, Action<string> writeLine)
        {
            _config = config.ThrowIfNull(nameof(config));
            _directory = directory.ThrowIfNull(nameof(directory));
            _fileUtil = fileUtil.ThrowIfNull(nameof(fileUtil));
            _writeLine = writeLine.ThrowIfNull(nameof(writeLine));
        }

        public void CommandList()
        {
            foreach (var configEntry in _config)
            {
                PerformOnContent(configEntry, (content, i, readVersion) =>
                {
                    if (readVersion != null)
                    {
                        _writeLine($"{ToRelativePath(content.File)} ({i}): {readVersion}");
                    }
                });
            }
        }

        public void CommandIncrement(int position)
        {
            WriteContent(_config, version => version.Increment(position));
        }

        public void CommandAssign(int position, int number)
        {
            WriteContent(_config, version => version.Assign(position, number));
        }

        public void CommandWrite(string versionText)
        {
            var newVersion = VersionHelper.ParseVersionFromText(versionText);

            WriteContent(_config, version => newVersion);
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

        private void WriteContent(IEnumerable<BumpyConfiguration> config, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            List<FileContent> newContent = new List<FileContent>();

            foreach (var configEntry in config)
            {
                newContent.AddRange(TransformContent(configEntry, transformFunction));
            }

            _fileUtil.WriteFiles(newContent);
        }

        private IEnumerable<FileContent> TransformContent(BumpyConfiguration config, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            var newLinesPerFile = new Dictionary<FileContent, List<string>>();

            PerformOnContent(config, (content, i, readVersion) =>
            {
                var newLine = content.Lines[i];

                if (readVersion != null)
                {
                    var newVersion = transformFunction(readVersion);
                    newLine = VersionHelper.ReplaceVersionInText(content.Lines[i], config.RegularExpression, newVersion);
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

        private void PerformOnContent(BumpyConfiguration config, Action<FileContent, int, BumpyVersion> versionInLineAction)
        {
            var files = _fileUtil.GetFiles(_directory, config.GlobPattern);
            var contents = files.Select(file => _fileUtil.ReadFile(file));

            foreach (var content in contents)
            {
                for (int i = 0; i < content.Lines.Count; i++)
                {
                    var readVersion = VersionHelper.FindVersion(content.Lines[i], config.RegularExpression);
                    versionInLineAction(content, i, readVersion);
                }
            }
        }

        private string ToRelativePath(FileInfo file)
        {
            return file.FullName.Substring(_directory.FullName.Length);
        }
    }
}
