using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Util;
using Bumpy.Version;

namespace Bumpy.Command
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
            var profile = BumpyConfiguration.DefaultProfile;

            foreach (var configEntry in config.ThrowIfNull(nameof(config)))
            {
                if (!profile.Equals(configEntry.Profile))
                {
                    profile = configEntry.Profile;
                    _writeLine($"[{profile}]");
                }

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

        public void CommandIncrementOnly(IEnumerable<BumpyConfiguration> config, int position)
        {
            WriteContent(config, version => version.Increment(position, false));
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

        public void CommandNew()
        {
            var configFile = new FileInfo(Path.Combine(_directory.FullName, BumpyConfiguration.ConfigFile));
            var created = _fileUtil.CreateConfig(configFile);

            if (created)
            {
                _writeLine($"Created file '{configFile}'");
            }
            else
            {
                _writeLine($"File '{configFile}' already exists");
            }
        }

        public void CommandHelp()
        {
            var builder = new StringBuilder();
            builder.AppendLine("A tool to maintain version information accross multiple files found in the current working directory");
            builder.AppendLine();
            builder.AppendLine("Commands:");
            builder.AppendLine("  help");
            builder.AppendLine("    View all commands and options");
            builder.AppendLine("  list");
            builder.AppendLine("    Lists all versions");
            builder.AppendLine("  new");
            builder.AppendLine($"    Creates a '{BumpyConfiguration.ConfigFile}' file if it does not exist");
            builder.AppendLine("  increment <one-based index number> (e.g. 'bumpy increment 3')");
            builder.AppendLine("    Increments the specified component of each version");
            builder.AppendLine("  incrementonly <one-based index number> (e.g. 'bumpy incrementonly 3')");
            builder.AppendLine("    Increments the specified component of each version, without updating following components");
            builder.AppendLine("  write <version string>");
            builder.AppendLine("    Overwrites a version with another version (e.g. 'bumpy write 1.0.0.0')");
            builder.AppendLine("  assign <one-based index number> <version number> (e.g. 'bumpy assign 3 99')");
            builder.AppendLine("    Replaces the specified component of a version with a new number");
            builder.AppendLine();
            builder.AppendLine("Options: (only available for 'list', 'increment', 'incrementonly', 'write' and 'assign')");
            builder.AppendLine("  -p <profile name>");
            builder.AppendLine("    Limit a command to a profile");
            builder.AppendLine("  -d <directory>");
            builder.AppendLine("    Run a command in a specific folder (the working directory is used by default)");
            builder.AppendLine("  -c <config file path>");
            builder.AppendLine("    Alternative name/path of a configuration file (default: './.bumpyconfig')");

            _writeLine(builder.ToString());
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
            return $".{file.ToRelativePath(_directory)}";
        }
    }
}
