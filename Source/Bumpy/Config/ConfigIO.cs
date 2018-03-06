using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bumpy.Config
{
    public static class ConfigIO
    {
        private const string CommentPattern = "#";
        private const string SectionStartPattern = "[";
        private const string SectionEndPattern = "]";
        private const string EqualsPattern = "=";
        private const string ProfileSplit = "|";

        public static string NewConfigFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("# Bumpy configuration file");
            builder.AppendLine("[AssemblyInfo.cs | assembly]");
            builder.AppendLine();
            builder.AppendLine("[*.nuspec | nuspec]");
            builder.AppendLine();
            builder.AppendLine("[*.csproj | nuspec]");

            return builder.ToString();
        }

        public static BumpyConfig ReadConfigFile(IEnumerable<string> lines)
        {
            var iniContent = ReadIniContent(lines.ToList());
            var configEntries = new List<BumpyConfigEntry>();

            foreach (KeyValuePair<string, Dictionary<string, string>> section in iniContent)
            {
                if (string.IsNullOrWhiteSpace(section.Key))
                {
                    continue;
                }

                var sectionSplit = section.Key.Split(ProfileSplit.ToCharArray(), 2);
                var configEntry = new BumpyConfigEntry();
                configEntry.Glob = sectionSplit[0].Trim();

                if (sectionSplit.Length == 2)
                {
                    configEntry.Profile = sectionSplit[1].Trim();
                }

                foreach (KeyValuePair<string, string> iniKeyValue in section.Value)
                {
                    if (iniKeyValue.Key.Equals("encoding", StringComparison.OrdinalIgnoreCase))
                    {
                        configEntry.Encoding = GetEncoding(iniKeyValue.Value);
                    }
                    else if (iniKeyValue.Key.Equals("regex", StringComparison.OrdinalIgnoreCase))
                    {
                        configEntry.Regex = iniKeyValue.Value;
                    }
                    else
                    {
                        throw new ConfigException($"Unrecognized element: '{iniKeyValue.Key}'");
                    }
                }

                configEntries.Add(configEntry);
            }

            return new BumpyConfig { Queries = configEntries };
        }

        private static Dictionary<string, Dictionary<string, string>> ReadIniContent(IList<string> lines)
        {
            var iniContent = new Dictionary<string, Dictionary<string, string>>();
            var currentSection = string.Empty;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var isComment = line.StartsWith(CommentPattern, StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line);
                var isSection = line.StartsWith(SectionStartPattern, StringComparison.Ordinal) && line.EndsWith(SectionEndPattern, StringComparison.Ordinal);

                if (isComment)
                {
                    continue;
                }
                else if (isSection)
                {
                    currentSection = line.Substring(1, line.Length - 2).Trim();

                    if (!iniContent.ContainsKey(currentSection))
                    {
                        iniContent.Add(currentSection, new Dictionary<string, string>());
                    }
                }
                else
                {
                    var lineSplit = line.Split(EqualsPattern.ToCharArray(), 2);

                    if (lineSplit.Length != 2)
                    {
                        throw new ConfigException($"Invalid syntax in line {i + 1}. Expected Format: 'key = value'");
                    }

                    var key = lineSplit[0].Trim();
                    var value = lineSplit[1].Trim();

                    if (!iniContent[currentSection].ContainsKey(key))
                    {
                        iniContent[currentSection].Add(key, value);
                    }
                    else
                    {
                        iniContent[currentSection][key] = value;
                    }
                }
            }

            return iniContent;
        }

        private static Encoding GetEncoding(string encodingText)
        {
            bool isCodingPage = int.TryParse(encodingText, out var codePage);

            if (isCodingPage)
            {
                return Encoding.GetEncoding(codePage);
            }
            else
            {
                return Encoding.GetEncoding(encodingText);
            }
        }
    }
}
