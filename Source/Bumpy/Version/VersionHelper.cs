using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bumpy.Version
{
    public static class VersionHelper
    {
        private const string _versionGroupName = "version";
        private const string _numbersGroupName = "numbers";
        private const string _labelGroupName = "label";

        private static readonly Regex _bumpyRegex = new Regex(@"^(?<numbers>\d+(\.\d+)*)(?<label>[\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        public static string ReplaceVersionInText(string text, string regexPattern, BumpyVersion newVersion)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));
            newVersion.ThrowIfNull(nameof(newVersion));

            var newText = text;
            var version = FindVersionString(text, regexPattern);

            if (version != null)
            {
                newText = text.Replace(version, newVersion.ToString());
                var tmp = FindVersion(newText, regexPattern);

                if (tmp == null || !tmp.Equals(newVersion))
                {
                    throw new InvalidOperationException($"Your provided version '{newVersion}' cannot be captured using your current regex configuration. Aborting to prevent issues");
                }
            }

            return newText;
        }

        public static string FindVersionString(string text, string regexPattern)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));

            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var group = regex.Match(text).Groups[_versionGroupName];

            if (group.Success)
            {
                return group.Value;
            }

            return null;
        }

        public static BumpyVersion FindVersion(string text, string regexPattern)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));

            var version = FindVersionString(text, regexPattern);

            if (version != null)
            {
                return ParseVersionFromText(version);
            }

            return null;
        }

        public static BumpyVersion ParseVersionFromText(string versionText)
        {
            versionText.ThrowIfNull(nameof(versionText));
            var match = _bumpyRegex.Match(versionText);
            var numbersGroup = match.Groups[_numbersGroupName];
            var labelGroup = match.Groups[_labelGroupName];

            if (!numbersGroup.Success || !labelGroup.Success)
            {
                throw new ArgumentException($"Illegal version string '{versionText}'");
            }

            var numbers = numbersGroup.Value.Split('.').Select(p => Convert.ToInt32(p));
            var label = labelGroup.Value;

            return new BumpyVersion(numbers, label);
        }
    }
}
