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
        private readonly Action<string> _writeLine;

        public Commands(IFileUtil fileUtil, Action<string> writeLine)
        {
            _fileUtil = fileUtil;
            _writeLine = writeLine;
        }

        public void List()
        {
            List(new BumpyArguments());
        }

        public void List(BumpyArguments arguments)
        {
            var configEntries = _fileUtil.ReadConfigFile(arguments.ConfigFile, arguments.Profile);
            var currentProfile = BumpyConfigEntry.DefaultProfile;

            foreach (var config in configEntries)
            {
                if (!currentProfile.Equals(config.Profile))
                {
                    currentProfile = config.Profile;
                    _writeLine($"[{config.Profile}]");
                }

                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(arguments.WorkingDirectory, glob);

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

                            _writeLine($"{content.File.ToRelativePath(arguments.WorkingDirectory)} ({marker}): {version}");
                        }

                        lineNumber++;
                    }

                    if (!versionFound)
                    {
                        _writeLine($"{content.File.ToRelativePath(arguments.WorkingDirectory)}: no version found");
                    }
                }
            }
        }

        public void Increment(int position)
        {
            Increment(position, new BumpyArguments());
        }

        public void Increment(int position, BumpyArguments arguments)
        {
            WriteTransformation(version => VersionFunctions.Increment(version, position, true), arguments);
        }

        public void IncrementOnly(int position)
        {
            IncrementOnly(position, new BumpyArguments());
        }

        public void IncrementOnly(int position, BumpyArguments arguments)
        {
            WriteTransformation(version => VersionFunctions.Increment(version, position, false), arguments);
        }

        public void Assign(int position, string formattedNumber)
        {
            Assign(position, formattedNumber, new BumpyArguments());
        }

        public void Assign(int position, string formattedNumber, BumpyArguments arguments)
        {
            WriteTransformation(version => VersionFunctions.Assign(version, position, formattedNumber), arguments);
        }

        public void Write(string versionText)
        {
            Write(versionText, new BumpyArguments());
        }

        public void Write(string versionText, BumpyArguments arguments)
        {
            WriteTransformation(version => VersionFunctions.ParseVersion(versionText), arguments);
        }

        public void Label(string versionLabel)
        {
            Label(versionLabel, new BumpyArguments());
        }

        public void Label(string versionLabel, BumpyArguments arguments)
        {
            WriteTransformation(version => VersionFunctions.Label(version, versionLabel), arguments);
        }

        public void Ensure()
        {
            Ensure(new BumpyArguments());
        }

        public void Ensure(BumpyArguments arguments)
        {
            var configEntries = _fileUtil.ReadConfigFile(arguments.ConfigFile, arguments.Profile);
            var versionsPerProfile = new Dictionary<string, List<BumpyVersion>>();

            foreach (var config in configEntries)
            {
                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(arguments.WorkingDirectory, glob);

                if (!versionsPerProfile.ContainsKey(config.Profile))
                {
                    versionsPerProfile.Add(config.Profile, new List<BumpyVersion>());
                }

                foreach (var file in files)
                {
                    var content = _fileUtil.ReadFileContent(file, config.Encoding);

                    foreach (var line in content.Lines)
                    {
                        var success = VersionFunctions.TryParseVersionInText(line, config.Regex, out var version, out _);

                        if (success)
                        {
                            versionsPerProfile[config.Profile].Add(version);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<BumpyVersion>> entry in versionsPerProfile)
            {
                var distinctVersions = entry.Value.Distinct().ToList();

                if (distinctVersions.Count > 1)
                {
                    var profileText = string.IsNullOrEmpty(entry.Key) ? string.Empty : $" in profile '{entry.Key}'";
                    var versions = string.Join(", ", distinctVersions.Select(v => v.ToString()));

                    throw new InvalidDataException($"Found different versions{profileText} ({versions}).");
                }

                if (string.IsNullOrWhiteSpace(entry.Key))
                {
                    _writeLine(entry.Value.First().ToString());
                }
                else
                {
                    _writeLine($"{entry.Key}: {entry.Value.First()}");
                }
            }
        }

        public void New()
        {
            var configFile = new FileInfo(BumpyConfig.ConfigFile);
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

        public void Help()
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
            builder.AppendLine($"    Creates a '{BumpyConfig.ConfigFile}' file if it does not exist");
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
            builder.AppendLine($"    Alternative name/path of a configuration file (default: '{BumpyConfig.ConfigFile}')");
            builder.AppendLine("  -n");
            builder.AppendLine("    No operation: The specified command (e.g. increment) will not perform file changes");

            _writeLine(builder.ToString());
        }

        private void WriteTransformation(Func<BumpyVersion, BumpyVersion> transformFunction, BumpyArguments arguments)
        {
            var configEntries = _fileUtil.ReadConfigFile(arguments.ConfigFile, arguments.Profile);
            var currentProfile = BumpyConfigEntry.DefaultProfile;

            if (arguments.NoOperation)
            {
                _writeLine("(NO-OP MODE: Will not persist changes to disk)");
            }

            foreach (var config in configEntries)
            {
                if (!currentProfile.Equals(config.Profile))
                {
                    currentProfile = config.Profile;
                    _writeLine($"[{config.Profile}]");
                }

                var glob = new Glob(config.Glob);
                var files = _fileUtil.GetFiles(arguments.WorkingDirectory, glob);

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

                            _writeLine($"{file.ToRelativePath(arguments.WorkingDirectory)} ({marker}): {oldVersion} -> {newVersion}");
                        }

                        if (!arguments.NoOperation)
                        {
                            newLines.Add(newLine);
                        }

                        lineNumber++;
                    }

                    if (!versionFound)
                    {
                        _writeLine($"{content.File.ToRelativePath(arguments.WorkingDirectory)}: no version found");
                    }
                    else if (dirty && !arguments.NoOperation)
                    {
                        var newContent = new FileContent(file, newLines, content.Encoding);
                        _fileUtil.WriteFileContent(newContent);
                    }
                }
            }
        }
    }
}
