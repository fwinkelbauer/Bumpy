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

        private static readonly Regex _bumpyRegex = new Regex(@"^(?<numbers>\d+(\.\d+)*)(?<label>[\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        public static BumpyVersion Increment(BumpyVersion version, int position)
        {
            var numbers = version.ThrowIfNull(nameof(version)).Numbers;
            var numbersCount = numbers.Count;
            position.ThrowIfOutOfRange(n => n < 1 || n > numbersCount, nameof(position), $"Position must be between 1 and {numbersCount}");

            numbers[checked(position - 1)]++;

            for (int i = position; i < numbers.Count; i++)
            {
                numbers[i] = 0;
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
                throw new ArgumentException($"Invalid version string '{versionText}'");
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
    }
}
