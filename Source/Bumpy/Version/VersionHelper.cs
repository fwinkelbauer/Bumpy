using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bumpy.Version
{
    public static class VersionHelper
    {
        private const string _versionGroupName = "version";

        private static readonly Regex _bumpyRegex = new Regex(@"^\d+(\.\d+)*$", RegexOptions.Singleline);

        public static string ReplaceVersionInText(string text, string regexPattern, BumpyVersion newVersion)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));
            newVersion.ThrowIfNull(nameof(newVersion));

            var newText = text;
            var version = FindVersion(text, regexPattern);

            if (version != null)
            {
                newText = text.Replace(version.ToString(), newVersion.ToString());

                if (FindVersion(newText, regexPattern) == null)
                {
                    throw new InvalidOperationException($"Your provided version '{newVersion}' cannot be captured using your current regex configuration. Aborting to prevent issues");
                }
            }

            return newText;
        }

        public static BumpyVersion FindVersion(string text, string regexPattern)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));

            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var group = regex.Match(text).Groups[_versionGroupName];

            if (group.Success)
            {
                return ParseVersionFromText(group.Value);
            }

            return null;
        }

        public static BumpyVersion ParseVersionFromText(string versionText)
        {
            versionText.ThrowIfNull(nameof(versionText));

            if (!_bumpyRegex.IsMatch(versionText))
            {
                throw new ArgumentException($"Illegal version string '{versionText}'");
            }

            var parts = versionText.Split('.').Select(p => Convert.ToInt32(p));

            return new BumpyVersion(parts);
        }
    }
}
