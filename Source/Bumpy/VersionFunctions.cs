using System;
using System.Text.RegularExpressions;

namespace Bumpy
{
    public static class VersionFunctions
    {
        private const string VersionGroupName = "version";
        private const string NumbersGroupName = "numbers";
        private const string LabelGroupName = "label";

        private static readonly Regex BumpyRegex = new Regex(@"^(?<numbers>\d+(\.\d+)*)(?<label>[_\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        public static BumpyVersion Increment(BumpyVersion version, int position, bool cascade)
        {
            var numbers = version.Numbers;
            var digits = version.Digits;

            position.ThrowIfOutOfRange(n => n < 1 || n > numbers.Count, nameof(position), $"Position must be between 1 and {numbers.Count}");

            var zeroBasedIndex = checked(position - 1);
            numbers[zeroBasedIndex]++;
            var digitCount = numbers[zeroBasedIndex].ToString().Length;
            digits[zeroBasedIndex] = Math.Max(digits[zeroBasedIndex], digitCount);

            if (cascade)
            {
                for (int i = position; i < numbers.Count; i++)
                {
                    numbers[i] = 0;
                    digits[i] = 1;
                }
            }

            return new BumpyVersion(numbers, digits, version.Label);
        }

        public static BumpyVersion Assign(BumpyVersion version, int position, string formattedNumber)
        {
            if (!int.TryParse(formattedNumber, out int number))
            {
                throw new ArgumentException($"Expected '{formattedNumber}' to be a number");
            }

            var numbers = version.Numbers;
            var digits = version.Digits;

            position.ThrowIfOutOfRange(n => n < 1 || n > numbers.Count, nameof(position), $"Position must be between 1 and {numbers.Count}");
            number.ThrowIfOutOfRange(n => n < 0, nameof(formattedNumber), "Number cannot be negative");

            var zeroBasedIndex = checked(position - 1);
            numbers[zeroBasedIndex] = number;
            digits[zeroBasedIndex] = formattedNumber.Length;

            return new BumpyVersion(numbers, digits, version.Label);
        }

        public static BumpyVersion ParseVersion(string versionText)
        {
            var match = BumpyRegex.Match(versionText);
            var numbersGroup = match.Groups[NumbersGroupName];
            var labelGroup = match.Groups[LabelGroupName];

            if (!numbersGroup.Success || !labelGroup.Success)
            {
                throw new ArgumentException($"The provided version string '{versionText}' is not supported");
            }

            var formattedNumbers = numbersGroup.Value.Split('.');

            return new BumpyVersion(formattedNumbers, labelGroup.Value);
        }

        public static bool TryParseVersionInText(string text, string regexPattern, out BumpyVersion version)
        {
            version = null;

            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var group = regex.Match(text).Groups[VersionGroupName];
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
