using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy
{
    public sealed class Commands
    {
        private readonly IFileUtil _fileUtil;
        private readonly FileInfo _configFile;
        private readonly DirectoryInfo _directory;
        private readonly Action<string> _writeLine;

        public Commands(IFileUtil fileUtil, FileInfo configurationFile, DirectoryInfo directory, Action<string> writeLine)
        {
            _fileUtil = fileUtil;
            _configFile = configurationFile;
            _directory = directory;
            _writeLine = writeLine;
        }

        public void CommandList(string profile)
        {
            var configEntries = _fileUtil.ReadConfigFile(_configFile, profile);
            var currentProfile = BumpyConfiguration.DefaultProfile;

            foreach (var config in configEntries)
            {
                if (!currentProfile.Equals(config.Profile))
                {
                    currentProfile = config.Profile;
                    _writeLine($"[{config.Profile}]");
                }

                var glob = new Glob(config.SearchPattern);
                var files = _fileUtil.GetFiles(_directory, glob);

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    var lineNumber = 1;

                    foreach (var line in content.Lines)
                    {
                        var success = VersionFunctions.TryParseVersionInText(line, config.RegularExpression, out var version);

                        if (success)
                        {
                            _writeLine($"{content.File.ToRelativePath(_directory)} ({lineNumber}): {version}");
                        }

                        lineNumber++;
                    }
                }
            }
        }

        public void CommandIncrement(string profile, int position)
        {
            WriteTransformation(profile, version => VersionFunctions.Increment(version, position, true));
        }

        public void CommandIncrementOnly(string profile, int position)
        {
            WriteTransformation(profile, version => VersionFunctions.Increment(version, position, false));
        }

        public void CommandAssign(string profile, int position, string formattedNumber)
        {
            WriteTransformation(profile, version => VersionFunctions.Assign(version, position, formattedNumber));
        }

        public void CommandWrite(string profile, string versionText)
        {
            WriteTransformation(profile, version => VersionFunctions.ParseVersion(versionText));
        }

        public void CommandNew()
        {
            var configFile = new FileInfo(Path.Combine(_directory.FullName, BumpyConfiguration.ConfigFile));
            var created = _fileUtil.CreateConfigFile(configFile);

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

        private void WriteTransformation(string profile, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            var configEntries = _fileUtil.ReadConfigFile(_configFile, profile);

            foreach (var config in configEntries)
            {
                var glob = new Glob(config.SearchPattern);
                var files = _fileUtil.GetFiles(_directory, glob);

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    var lineNumber = 1;
                    var newLines = new List<string>();

                    foreach (var line in content.Lines)
                    {
                        var newLine = line;
                        var success = VersionFunctions.TryParseVersionInText(line, config.RegularExpression, out var oldVersion);

                        if (success)
                        {
                            var newVersion = transformFunction(oldVersion);
                            newLine = line.Replace(oldVersion.ToString(), newVersion.ToString());
                            VersionFunctions.EnsureExpectedVersion(newLine, config.RegularExpression, newVersion);
                            _writeLine($"{file.ToRelativePath(_directory)} ({lineNumber}): {oldVersion} -> {newVersion}");
                        }

                        newLines.Add(newLine);
                        lineNumber++;
                    }

                    var newContent = new FileContent(file, newLines, content.Encoding);
                    _fileUtil.WriteFileContent(newContent);
                }
            }
        }
    }
}
