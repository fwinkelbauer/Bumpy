using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bumpy.Core.Config;

[assembly: CLSCompliant(true)]
namespace Bumpy.Core
{
    /// <summary>
    /// Bumpy's "main class" which can perform all major operations.
    /// </summary>
    public sealed class Command
    {
        private readonly IFileUtil _fileUtil;
        private readonly Action<string> _writeLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="fileUtil">The <see cref="IFileUtil"/> implementation.</param>
        /// <param name="writeLine">A logging action</param>
        public Command(IFileUtil fileUtil, Action<string> writeLine)
        {
            _fileUtil = fileUtil;
            _writeLine = writeLine;
        }

        /// <summary>
        /// A factory method which provides a default <see cref="Command"/> object.
        /// </summary>
        /// <returns>A default object</returns>
        public static Command Default()
        {
            return new Command(new FileUtil(), Console.WriteLine);
        }

        /// <summary>
        /// Lists all versions.
        /// </summary>
        public void List()
        {
            List(new BumpyArguments());
        }

        /// <summary>
        /// Lists all versions.
        /// </summary>
        /// <param name="arguments">A custom set of arguments</param>
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

        /// <summary>
        /// Increments the position in all versions by one.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <example>
        /// Increment(2):
        ///   1.2.3 -> 1.3.0
        /// </example>
        public void Increment(int position)
        {
            Increment(position, new BumpyArguments());
        }

        /// <summary>
        /// Increments the position in all versions by one.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <param name="arguments">A custom set of arguments</param>
        /// <example>
        /// Increment(2, new BumpyArguments { Profile = "custom" }):
        ///   1.2.3 -> 1.3.0
        /// </example>
        public void Increment(int position, BumpyArguments arguments)
        {
            ApplyTransformation(version => VersionFunctions.Increment(version, position, true), arguments);
        }

        /// <summary>
        /// Increments the position in all versions by one without updating following components.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <example>
        /// IncrementOnly(2):
        ///   1.2.3 -> 1.3.3
        /// </example>
        public void IncrementOnly(int position)
        {
            IncrementOnly(position, new BumpyArguments());
        }

        /// <summary>
        /// Increments the position in all versions by one without updating following components.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <param name="arguments">A custom set of arguments</param>
        /// <example>
        /// IncrementOnly(2, new BumpyArguments { Profile = "custom" }):
        ///   1.2.3 -> 1.3.3
        /// </example>
        public void IncrementOnly(int position, BumpyArguments arguments)
        {
            ApplyTransformation(version => VersionFunctions.Increment(version, position, false), arguments);
        }

        /// <summary>
        /// Assigns a given number to a given position.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <param name="formattedNumber">The number to assign</param>
        /// <example>
        /// Assign(3, 10):
        ///   1.2.3 -> 1.2.10
        /// </example>
        public void Assign(int position, string formattedNumber)
        {
            Assign(position, formattedNumber, new BumpyArguments());
        }

        /// <summary>
        /// Assigns a given number to a given position.
        /// </summary>
        /// <param name="position">A one-based index</param>
        /// <param name="formattedNumber">The number to assign</param>
        /// <param name="arguments">A custom set of arguments</param>
        /// <example>
        /// Assign(3, 10, new BumpyArguments { Profile = "custom" }):
        ///   1.2.3 -> 1.2.10
        /// </example>
        public void Assign(int position, string formattedNumber, BumpyArguments arguments)
        {
            ApplyTransformation(version => VersionFunctions.Assign(version, position, formattedNumber), arguments);
        }

        /// <summary>
        /// Overwrites all versions with a new version.
        /// </summary>
        /// <param name="versionText">The version text to overwrite all other versions</param>
        /// <example>
        /// Write("8.15.7"):
        ///   1.2.3 -> 8.15.7
        /// </example>
        public void Write(string versionText)
        {
            Write(versionText, new BumpyArguments());
        }

        /// <summary>
        /// Overwrites all versions with a new version.
        /// </summary>
        /// <param name="versionText">The version text to overwrite all other versions</param>
        /// <param name="arguments">A custom set of arguments</param>
        /// <example>
        /// Write("8.15.7", new BumpyArguments { Profile = "custom" }):
        ///   1.2.3 -> 8.15.7
        /// </example>
        public void Write(string versionText, BumpyArguments arguments)
        {
            ApplyTransformation(version => VersionFunctions.ParseVersion(versionText), arguments);
        }

        /// <summary>
        /// Replaces the postfix text of a version.
        /// </summary>
        /// <param name="versionLabel">The new postfix text</param>
        /// <example>
        /// Label("-beta"):
        ///   1.2.3 -> 1.2.3-beta
        ///   4.3.7-alpha -> 4.3.7-beta
        ///
        /// Label(""):
        ///   1.2.3-beta -> 1.2.3
        /// </example>
        public void Label(string versionLabel)
        {
            Label(versionLabel, new BumpyArguments());
        }

        /// <summary>
        /// Replaces the postfix text of a version.
        /// </summary>
        /// <param name="versionLabel">The new postfix text</param>
        /// <param name="arguments">A custom set of arguments</param>
        /// <example>
        /// Label("-beta"):
        ///   1.2.3 -> 1.2.3-beta
        ///   4.3.7-alpha -> 4.3.7-beta
        ///
        /// Label(""):
        ///   1.2.3-beta -> 1.2.3
        /// </example>
        public void Label(string versionLabel, BumpyArguments arguments)
        {
            ApplyTransformation(version => VersionFunctions.Label(version, versionLabel), arguments);
        }

        /// <summary>
        /// Checks if all versions in a profile are equal. Throws an Exception if not.
        /// </summary>
        public void Ensure()
        {
            Ensure(new BumpyArguments());
        }

        /// <summary>
        /// Checks if all versions in a profile are equal. Throws an Exception if not.
        /// </summary>
        /// <param name="arguments">A custom set of arguments</param>
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

        /// <summary>
        /// Creates a new configuration file in the current working directory if one does not exist already.
        /// </summary>
        public void New()
        {
            var configFile = new FileInfo(BumpyConfigEntry.DefaultConfigFile);
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

        /// <summary>
        /// A generalized transform method which can be used to create custom behavior.
        /// All file based Bumpy operations (e.g. Increment, Write, ...) are based on this method.
        /// </summary>
        /// <param name="transformFunction">A function which creates a new version based on an old version</param>
        /// <param name="arguments">A custom set of arguments</param>
        public void ApplyTransformation(Func<BumpyVersion, BumpyVersion> transformFunction, BumpyArguments arguments)
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
