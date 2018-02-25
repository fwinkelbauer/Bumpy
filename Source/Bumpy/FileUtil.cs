using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bumpy
{
    public sealed class FileUtil : IFileUtil
    {
        private const string AllFilesPattern = "*";
        private const string CommentPattern = "#";
        private const string ProfileStartPattern = "[";
        private const string ProfileEndPattern = "]";
        private const string EqualsPattern = "=";

        private const char MainSplit = '=';
        private const char EncodingSplit = '|';

        public IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, Glob glob)
        {
            return directory.EnumerateFiles(AllFilesPattern, SearchOption.AllDirectories)
                .Where(f => glob.IsMatch(f.ToRelativePath(directory)));
        }

        public FileContent ReadFileContent(FileInfo file, Encoding encoding)
        {
            return new FileContent(file, File.ReadLines(file.FullName, encoding), encoding);
        }

        public void WriteFileContent(FileContent fileContent)
        {
            File.WriteAllLines(fileContent.File.FullName, fileContent.Lines, fileContent.Encoding);
        }

        public bool CreateConfigFile(FileInfo configFile)
        {
            if (configFile.Exists)
            {
                return false;
            }

            var builder = new StringBuilder();
            builder.AppendLine("# General usage:");
            builder.AppendLine("# <file glob pattern> = <regular expression>");
            builder.AppendLine("# <file glob pattern> | <encoding> = <regular expression>");
            builder.AppendLine("# <file glob pattern> | <code page> = <regular expression>");
            builder.AppendLine("# Note that the regular expression must contain a named capture group '<?version>' which contains the actual version information");
            builder.AppendLine();
            builder.AppendLine("# Example: Search for versions of the format a.b.c.d (e.g. 1.22.7.50) in all AssemblyInfo.cs files (with UTF-8 with BOM)");
            builder.AppendLine(@"# AssemblyInfo.cs | UTF-8 = (?<version>\d+\.\d+\.\d+\.\d+)");
            builder.AppendLine();
            builder.AppendLine("# Example: Search all .nuspec files (UTF-8 without BOM) in the folder 'NuSpec'");
            builder.AppendLine(@"# NuSpec\**\*.nuspec = <version>(?<version>\d+(\.\d+)+)");
            builder.AppendLine();
            builder.AppendLine("# Templates: Bumpy ships with a default configuration (regular expression + encoding) for some known file types.");
            builder.AppendLine();
            builder.AppendLine("# .NET Framework:");
            builder.AppendLine("[assembly]");
            builder.AppendLine("AssemblyInfo.cs");
            builder.AppendLine();
            builder.AppendLine("[nuspec]");
            builder.AppendLine("*.nuspec");
            builder.AppendLine();
            builder.AppendLine("# .NET Core:");
            builder.AppendLine("*.csproj");

            File.WriteAllText(configFile.FullName, builder.ToString());

            return true;
        }

        public IEnumerable<BumpyConfiguration> ReadConfigFile(FileInfo configFile, string profile)
        {
            var config = ReadConfigFile(configFile).ToArray();

            if (!profile.Equals(BumpyConfiguration.DefaultProfile))
            {
                config = config.Where(c => c.Profile == profile).ToArray();

                if (!config.Any())
                {
                    throw new InvalidOperationException($"Profile '{profile}' does not exist in '{configFile.Name}'");
                }
            }

            return config;
        }

        private IEnumerable<BumpyConfiguration> ReadConfigFile(FileInfo configFile)
        {
            var lines = File.ReadLines(configFile.FullName);
            var profile = string.Empty;

            foreach (var line in lines)
            {
                var isComment = line.StartsWith(CommentPattern, StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line);
                var isProfile = line.StartsWith(ProfileStartPattern, StringComparison.Ordinal) && line.EndsWith(ProfileEndPattern, StringComparison.Ordinal);

                if (isComment)
                {
                    continue;
                }
                else if (isProfile)
                {
                    profile = line.Replace(ProfileStartPattern, string.Empty).Replace(ProfileEndPattern, string.Empty).Trim();

                    if (profile.Length == 0)
                    {
                        throw new InvalidDataException("A profile name cannot be empty");
                    }

                    continue;
                }

                // Configuration line variants:
                //   <search pattern> | <code page> = <regex>
                //   <search pattern> | <encoding name> = <regex>
                //   <search pattern> = <regex>
                var mainSplit = line.Split(MainSplit);
                var leftSplit = mainSplit[0].Split(EncodingSplit);

                if (mainSplit.Length == 1 && Template.TryFindTemplate(mainSplit[0], out Template template))
                {
                    yield return new BumpyConfiguration(profile, mainSplit[0], template.RegularExpression, template.Encoding);
                    continue;
                }

                Encoding encoding = new UTF8Encoding(false);

                if (leftSplit.Length == 2)
                {
                    var possibleEncoding = leftSplit[1].Trim();
                    bool isCodingPage = int.TryParse(possibleEncoding, out var codePage);

                    if (isCodingPage)
                    {
                        encoding = Encoding.GetEncoding(codePage);
                    }
                    else
                    {
                        encoding = Encoding.GetEncoding(possibleEncoding);
                    }
                }

                var searchPattern = leftSplit[0].Trim();
                var regularExpression = string.Join(EqualsPattern, mainSplit, 1, mainSplit.Length - 1).Trim();

                yield return new BumpyConfiguration(profile, searchPattern, regularExpression, encoding);
            }
        }
    }
}
