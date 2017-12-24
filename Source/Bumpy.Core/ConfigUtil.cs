using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Core
{
    class ConfigUtil
    {
        public IEnumerable<BumpyConfiguration> ReadConfigFile(FileInfo configFile)
        {
            configFile.ThrowIfNull(nameof(configFile));

            var lines = File.ReadLines(configFile.FullName);
            var profile = string.Empty;

            foreach (var line in lines)
            {
                var isComment = line.StartsWith("#", StringComparison.Ordinal) || string.IsNullOrWhiteSpace(line);
                var isProfile = line.StartsWith("[", StringComparison.Ordinal) && line.EndsWith("]", StringComparison.Ordinal);

                if (isComment)
                {
                    continue;
                }
                else if (isProfile)
                {
                    profile = line.Replace("[", string.Empty).Replace("]", string.Empty).Trim();

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
                var mainSplit = line.Split('=');
                var leftSplit = mainSplit[0].Split('|');

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
                var regularExpression = string.Join("=", mainSplit, 1, mainSplit.Length - 1).Trim();

                yield return new BumpyConfiguration(profile, searchPattern, regularExpression, encoding);
            }
        }
    }
}
