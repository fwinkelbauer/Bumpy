using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bumpy.Core
{
    internal class FileUtil
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

        public FileContent ReadFile(FileInfo file, Encoding encoding)
        {
            return new FileContent(file, File.ReadLines(file.FullName, encoding), encoding);
        }

        public void WriteFile(FileContent file)
        {
            File.WriteAllLines(file.File.FullName, file.Lines, file.Encoding);
        }

        public IEnumerable<BumpyConfiguration> ReadConfigFile(FileInfo configFile)
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
