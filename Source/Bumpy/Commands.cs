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
        private readonly IFileUtil _fileUtil;
        private readonly Action<string> _writeLine;

        public Commands(IFileUtil fileUtil, Action<string> writeLine)
        {
            _fileUtil = fileUtil.ThrowIfNull(nameof(fileUtil));
            _writeLine = writeLine.ThrowIfNull(nameof(writeLine));
        }

        public void Increment(DirectoryInfo directory, string globPattern, string regexPattern, int position)
        {
            var files = _fileUtil.GetFiles(directory, globPattern);

            foreach (var file in files)
            {
                var lines = _fileUtil.ReadLines(file).ToList();
                List<string> newLines = new List<string>();

                for (int i = 0; i < lines.Count(); i++)
                {
                    var readVersion = VersionHelper.FindVersion(lines[i], regexPattern);
                    var newLine = lines[i];

                    if (readVersion != null)
                    {
                        var newVersion = readVersion.Increment(position);
                        newLine = VersionHelper.ReplaceVersionInText(lines[i], regexPattern, newVersion);
                        _writeLine($"{ToRelativePath(directory, file)} ({i}): {readVersion} -> {newVersion}");
                    }

                    newLines.Add(newLine);
                }

                _fileUtil.WriteLines(file, newLines);
            }
        }

        public void List(DirectoryInfo directory, string globPattern, string regexPattern)
        {
            var files = _fileUtil.GetFiles(directory, globPattern);

            foreach (var file in files)
            {
                var lines = _fileUtil.ReadLines(file).ToList();

                for (int i = 0; i < lines.Count(); i++)
                {
                    var readVersion = VersionHelper.FindVersion(lines[i], regexPattern);

                    if (readVersion != null)
                    {
                        _writeLine($"{ToRelativePath(directory, file)} ({i}): {readVersion}");
                    }
                }
            }
        }

        public void Assign(DirectoryInfo directory, string globPattern, string regexPattern, int position, int number)
        {
            var files = _fileUtil.GetFiles(directory, globPattern);

            foreach (var file in files)
            {
                var lines = _fileUtil.ReadLines(file).ToList();
                List<string> newLines = new List<string>();

                for (int i = 0; i < lines.Count(); i++)
                {
                    var readVersion = VersionHelper.FindVersion(lines[i], regexPattern);
                    var newLine = lines[i];

                    if (readVersion != null)
                    {
                        var newVersion = readVersion.Assign(position, number);
                        newLine = VersionHelper.ReplaceVersionInText(lines[i], regexPattern, newVersion);
                        _writeLine($"{ToRelativePath(directory, file)} ({i}): {readVersion} -> {newVersion}");
                    }

                    newLines.Add(newLine);
                }

                _fileUtil.WriteLines(file, newLines);
            }
        }

        public void Write(DirectoryInfo directory, string globPattern, string regexPattern, string versionText)
        {
            var files = _fileUtil.GetFiles(directory, globPattern);
            var newVersion = VersionHelper.ParseVersionFromText(versionText);

            foreach (var file in files)
            {
                var lines = _fileUtil.ReadLines(file).ToList();
                List<string> newLines = new List<string>();

                for (int i = 0; i < lines.Count(); i++)
                {
                    var readVersion = VersionHelper.FindVersion(lines[i], regexPattern);

                    newLines.Add(VersionHelper.ReplaceVersionInText(lines[i], regexPattern, newVersion));

                    if (readVersion != null)
                    {
                        _writeLine($"{ToRelativePath(directory, file)} ({i}): {readVersion} -> {newVersion}");
                    }
                }

                _fileUtil.WriteLines(file, newLines);
            }
        }

        public void CreateConfigFile(DirectoryInfo directory)
        {
            _fileUtil.CreateConfig(directory);
        }

        public IEnumerable<BumpyConfiguration> ReadConfigFile(DirectoryInfo directory)
        {
            return _fileUtil.ReadConfig(directory);
        }

        public void PrintHelp()
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

        private string ToRelativePath(DirectoryInfo directory, FileInfo file)
        {
            return file.FullName.Substring(directory.FullName.Length);
        }
    }
}
