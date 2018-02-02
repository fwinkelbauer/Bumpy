using System;
using System.Text.RegularExpressions;

namespace Bumpy
{
    public static class VersionFunctions
    {
        private const string _versionGroupName = "version";
        private const string _numbersGroupName = "numbers";
        private const string _labelGroupName = "label";

        private static readonly Regex _bumpyRegex = new Regex(@"^(?<numbers>\d+(\.\d+)*)(?<label>[_\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        public static BumpyVersion Increment(BumpyVersion version, int position, bool cascade)
        {
            var numbers = version.Numbers;
            var digits = version.Digits;

            position.ThrowIfOutOfRange(n => n < 1 || n > numbers.Length, nameof(position), $"Position must be between 1 and {numbers.Length}");

            var zeroBasedIndex = position - 1;
            numbers[zeroBasedIndex]++;
            var digitCount = numbers[zeroBasedIndex].ToString().Length;
            digits[zeroBasedIndex] = Math.Max(digits[zeroBasedIndex], digitCount);

            if (cascade)
            {
                for (int i = position; i < numbers.Length; i++)
                {
                    numbers[i] = 0;
                }
            }

            return new BumpyVersion(numbers, digits, version.Label);
        }

        public static BumpyVersion Assign(BumpyVersion version, int position, string formattedNumber)
        {
            int number;

            if (!int.TryParse(formattedNumber, out number))
            {
                throw new ArgumentException($"Expected '{formattedNumber}' to be a number");
            }

            var numbers = version.Numbers;
            var digits = version.Digits;

            position.ThrowIfOutOfRange(n => n < 1 || n > numbers.Length, nameof(position), $"Position must be between 1 and {numbers.Length}");
            number.ThrowIfOutOfRange(n => n < 0, nameof(formattedNumber), "Number cannot be negative");

            var zeroBasedIndex = position - 1;
            numbers[zeroBasedIndex] = number;
            digits[zeroBasedIndex] = formattedNumber.Length;

            return new BumpyVersion(numbers, digits, version.Label);
        }

        public static BumpyVersion ParseVersion(string versionText)
        {
            var match = _bumpyRegex.Match(versionText);
            var numbersGroup = match.Groups[_numbersGroupName];
            var labelGroup = match.Groups[_labelGroupName];

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
