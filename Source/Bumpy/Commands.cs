using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Config;

namespace Bumpy
{
    public sealed class Commands
    {
        private readonly IFileUtil _fileUtil;
        private readonly FileInfo _configFile;
        private readonly DirectoryInfo _directory;
        private readonly bool _noOperation;
        private readonly Action<string> _writeLine;

        public Commands(IFileUtil fileUtil, FileInfo configurationFile, DirectoryInfo directory, bool noOperation, Action<string> writeLine)
        {
            _fileUtil = fileUtil;
            _configFile = configurationFile;
            _directory = directory;
            _noOperation = noOperation;
            _writeLine = writeLine;
        }

        public void CommandList(string profile)
        {
            var configEntries = _fileUtil.ReadConfigFile(_configFile, profile);
            var currentProfile = BumpyConfigEntry.DefaultProfile;

            foreach (var config in configEntries)
            {
                if (!currentProfile.Equals(config.Profile))
                {
                    currentProfile = config.Profile;
                    _writeLine($"[{config.Profile}]");
                }

                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(_directory, glob);

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    var lineNumber = 1;
                    var versionFound = false;

                    foreach (var line in content.Lines)
                    {
                        var success = VersionFunctions.TryParseVersionInText(line, config.Regex, out var version, out var marker);

                        if (success)
                        {
                            versionFound = true;

                            if (string.IsNullOrEmpty(marker))
                            {
                                marker = lineNumber.ToString();
                            }

                            _writeLine($"{content.File.ToRelativePath(_directory)} ({marker}): {version}");
                        }

                        lineNumber++;
                    }

                    if (!versionFound)
                    {
                        _writeLine($"{content.File.ToRelativePath(_directory)}: no version found");
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

        public void CommandLabel(string profile, string versionLabel)
        {
            WriteTransformation(profile, version => VersionFunctions.Label(version, versionLabel));
        }

        public void CommandEnsure(string profile)
        {
            var configEntries = _fileUtil.ReadConfigFile(_configFile, profile);
            var versionsPerProfile = new Dictionary<string, List<BumpyVersion>>();

            foreach (var config in configEntries)
            {
                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(_directory, glob);

                if (!versionsPerProfile.ContainsKey(config.Profile))
                {
                    versionsPerProfile.Add(config.Profile, new List<BumpyVersion>());
                }

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    foreach (var line in content.Lines)
                    {
                        var success = VersionFunctions.TryParseVersionInText(line, config.Regex, out var version, out var marker);

                        if (success)
                        {
                            versionsPerProfile[config.Profile].Add(version);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<BumpyVersion>> entry in versionsPerProfile)
            {
                var distinctVersions = entry.Value.Distinct();

                if (distinctVersions.Count() > 1)
                {
                    var profileText = string.IsNullOrEmpty(entry.Key) ? string.Empty : $" in profile '{entry.Key}'";
                    var versions = string.Join(", ", distinctVersions.Select(v => v.ToString()));

                    throw new InvalidDataException($"Found different versions{profileText} ({versions}). See 'bumpy list'.");
                }
            }

            _writeLine("Success");
        }

        public void CommandNew()
        {
            var configFile = new FileInfo(Path.Combine(_directory.FullName, BumpyConfigEntry.ConfigFile));
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
            builder.AppendLine($"    Creates a '{BumpyConfigEntry.ConfigFile}' file if it does not exist");
            builder.AppendLine("  increment <one-based index number> (e.g. 'bumpy increment 3')");
            builder.AppendLine("    Increments the specified component of each version");
            builder.AppendLine("  incrementonly <one-based index number> (e.g. 'bumpy incrementonly 3')");
            builder.AppendLine("    Increments the specified component of each version, without updating following components");
            builder.AppendLine("  write <version string>");
            builder.AppendLine("    Overwrites a version with another version (e.g. 'bumpy write 1.0.0.0')");
            builder.AppendLine("  assign <one-based index number> <version number> (e.g. 'bumpy assign 3 99')");
            builder.AppendLine("    Replaces the specified component of a version with a new number");
            builder.AppendLine("  label <suffix version text>");
            builder.AppendLine("    Replaces the suffix text of a version (e.g. 'bumpy label \"-beta\"')");
            builder.AppendLine("  ensure");
            builder.AppendLine("    Checks that all versions in a profile are equal");
            builder.AppendLine();
            builder.AppendLine("Options: (only available for 'list', 'increment', 'incrementonly', 'write', 'assign', 'label' and 'ensure')");
            builder.AppendLine("  -p <profile name>");
            builder.AppendLine("    Limit a command to a profile");
            builder.AppendLine("  -d <directory>");
            builder.AppendLine("    Run a command in a specific folder (the working directory is used by default)");
            builder.AppendLine("  -c <config file path>");
            builder.AppendLine("    Alternative name/path of a configuration file (default: './.bumpyconfig')");
            builder.AppendLine("  -n");
            builder.AppendLine("    No operation: The specified command (e.g. increment) will not perform file changes");

            _writeLine(builder.ToString());
        }

        private void WriteTransformation(string profile, Func<BumpyVersion, BumpyVersion> transformFunction)
        {
            var configEntries = _fileUtil.ReadConfigFile(_configFile, profile);

            if (_noOperation)
            {
                _writeLine("(NO-OP MODE: Will not persist changes to disk)");
            }

            foreach (var config in configEntries)
            {
                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(_directory, glob);

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    var lineNumber = 1;
                    var newLines = new List<string>();
                    var versionFound = false;
                    var dirty = false;

                    foreach (var line in content.Lines)
                    {
                        var newLine = line;
                        var success = VersionFunctions.TryParseVersionInText(line, config.Regex, out var oldVersion, out var marker);

                        if (success)
                        {
                            versionFound = true;
                            var newVersion = transformFunction(oldVersion);

                            if (!newVersion.Equals(oldVersion))
                            {
                                newLine = line.Replace(oldVersion.ToString(), newVersion.ToString());
                                VersionFunctions.EnsureExpectedVersion(newLine, config.Regex, newVersion);
                                dirty = true;
                            }

                            if (string.IsNullOrEmpty(marker))
                            {
                                marker = lineNumber.ToString();
                            }

                            _writeLine($"{file.ToRelativePath(_directory)} ({marker}): {oldVersion} -> {newVersion}");
                        }

                        if (!_noOperation)
                        {
                            newLines.Add(newLine);
                        }

                        lineNumber++;
                    }

                    if (!versionFound)
                    {
                        _writeLine($"{content.File.ToRelativePath(_directory)}: no version found");
                    }
                    else if (dirty && !_noOperation)
                    {
                        var newContent = new FileContent(file, newLines, content.Encoding);
                        _fileUtil.WriteFileContent(newContent);
                    }
                }
            }
        }
    }
}
