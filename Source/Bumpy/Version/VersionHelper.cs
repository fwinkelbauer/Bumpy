using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bumpy.Version
{
    public static class VersionHelper
    {
        private const string _versionGroupName = "version";

        private static readonly Regex _bumpyRegex = new Regex(@"^\d+(\.\d+)*$", RegexOptions.Singleline);

        public static string ReplaceVersionInText(string text, string regexPattern, BumpyVersion version)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));
            version.ThrowIfNull(nameof(version));

            var newText = text;
            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var group = regex.Match(text).Groups[_versionGroupName];

            if (group.Success)
            {
                ValidateVersionText(group.Value);
                newText = text.Replace(group.Value, version.ToString());
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

            ValidateVersionText(versionText);

            var parts = versionText.Split('.').Select(p => Convert.ToInt32(p));

            return new BumpyVersion(parts);
        }

        private static void ValidateVersionText(string versionText)
        {
            if (!_bumpyRegex.IsMatch(versionText))
            {
                throw new ArgumentException($"Illegal version string '{versionText}'");
            }
        }
    }
}
