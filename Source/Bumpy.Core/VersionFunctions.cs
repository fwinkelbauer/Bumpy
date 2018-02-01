using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bumpy.Core
{
    public static class VersionFunctions
    {
        private const string _versionGroupName = "version";
        private const string _numbersGroupName = "numbers";
        private const string _labelGroupName = "label";

        private static readonly Regex _bumpyRegex = new Regex(@"^(?<numbers>\d+(\.\d+)*)(?<label>[_\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        public static BumpyVersion Increment(BumpyVersion version, int position, bool cascade)
        {
            var numbers = version.ThrowIfNull(nameof(version)).Numbers;
            var numbersCount = numbers.Count;
            position.ThrowIfOutOfRange(n => n < 1 || n > numbersCount, nameof(position), $"Position must be between 1 and {numbersCount}");

            numbers[checked(position - 1)]++;

            if (cascade)
            {
                for (int i = position; i < numbersCount; i++)
                {
                    numbers[i] = 0;
                }
            }

            return new BumpyVersion(numbers, version.Label);
        }

        public static BumpyVersion Assign(BumpyVersion version, int position, int number)
        {
            var numbers = version.ThrowIfNull(nameof(version)).Numbers;
            var numbersCount = numbers.Count;

            position.ThrowIfOutOfRange(n => n < 1 || n > numbersCount, nameof(position), $"Position must be between 1 and {numbersCount}");
            number.ThrowIfOutOfRange(n => n < 0, nameof(number), "Number cannot be negative");

            numbers[checked(position - 1)] = number;

            return new BumpyVersion(numbers, version.Label);
        }

        public static BumpyVersion ParseVersion(string versionText)
        {
            var match = _bumpyRegex.Match(versionText.ThrowIfNull(nameof(versionText)));
            var numbersGroup = match.Groups[_numbersGroupName];
            var labelGroup = match.Groups[_labelGroupName];

            if (!numbersGroup.Success || !labelGroup.Success)
            {
                throw new ArgumentException($"The provided version string '{versionText}' is not supported");
            }

            var numbers = numbersGroup.Value.Split('.').Select(p => Convert.ToInt32(p));

            return new BumpyVersion(numbers, labelGroup.Value);
        }

        public static bool TryParseVersionInText(string text, string regexPattern, out BumpyVersion version)
        {
            text.ThrowIfNull(nameof(text));
            regexPattern.ThrowIfNull(nameof(regexPattern));

            version = null;

            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var group = regex.Match(text).Groups[_versionGroupName];
            var success = group.Success;

            if (success)
            {
                version = ParseVersion(group.Value);
            }

            return success;
        }

        public static string EnsureExpectedVersion(string text, string regexPattern, BumpyVersion expectedVersion)
        {
            if (TryParseVersionInText(text, regexPattern, out var version))
            {
                if (version.Equals(expectedVersion))
                {
                    return text;
                }
            }

            throw new InvalidOperationException($"The provided version '{expectedVersion}' cannot be captured in the text '{text.Trim()}' using the regex '{regexPattern}'. Please correct either the version or the regex");
        }
    }
}
