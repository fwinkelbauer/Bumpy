using System;
using System.Collections.Generic;
using System.Text;

namespace Bumpy.Config
{
    internal static class LegacyConfigIO
    {
        private const string CommentPattern = "#";
        private const string ProfileStartPattern = "[";
        private const string ProfileEndPattern = "]";
        private const string EqualsPattern = "=";

        private const char MainSplit = '=';
        private const char EncodingSplit = '|';

        public static IEnumerable<BumpyConfigEntry> ReadConfigFile(IEnumerable<string> lines)
        {
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
                        throw new ConfigException("A profile name cannot be empty");
                    }

                    continue;
                }

                // Configuration line variants:
                //   <search pattern> | <code page> = <regex>
                //   <search pattern> | <encoding name> = <regex>
                //   <search pattern> = <regex>
                var mainSplit = line.Split(MainSplit);
                var leftSplit = mainSplit[0].Split(EncodingSplit);

                if (mainSplit.Length == 1)
                {
                    // Try to find template later
                    yield return new BumpyConfigEntry
                    {
                        Glob = mainSplit[0],
                        Profile = profile
                    };
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

                yield return new BumpyConfigEntry
                {
                    Glob = searchPattern,
                    Profile = profile,
                    Encoding = encoding,
                    Regex = regularExpression
                };
            }
        }
    }
}
