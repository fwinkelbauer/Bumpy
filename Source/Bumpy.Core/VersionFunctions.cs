using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bumpy.Core
{
    /// <summary>
    /// A set of functions which manipulate <see cref="BumpyVersion"/> objects.
    /// </summary>
    public static class VersionFunctions
    {
        private const string VersionGroupName = "version";
        private const string NumbersGroupName = "numbers";
        private const string LabelGroupName = "label";
        private const string MarkerGroupName = "marker";

        private static readonly Regex BumpyRegex = new Regex(@"^(?<numbers>\d+((\.|,)\d+)*)(?<label>[_\-\+\.0-9a-zA-Z]*)$", RegexOptions.Singleline);

        /// <summary>
        /// Creates a new version with incremented components.
        /// </summary>
        /// <param name="version">The initial version</param>
        /// <param name="position">A one-based index</param>
        /// <param name="cascade">True if following components should be updated, else false</param>
        /// <returns>A new version object with an incremented component.</returns>
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

            return new BumpyVersion(numbers, digits, version.Label, version.NumberDelimiter);
        }

        /// <summary>
        /// Creates a new version with an updated component.
        /// </summary>
        /// <param name="version">The initial version</param>
        /// <param name="position">A one-based index</param>
        /// <param name="formattedNumber">The new number for the given position</param>
        /// <returns>A new version object with an updated component</returns>
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

            return new BumpyVersion(numbers, digits, version.Label, version.NumberDelimiter);
        }

        /// <summary>
        /// Creates a new version with an updated postfix text.
        /// </summary>
        /// <param name="version">The initial version</param>
        /// <param name="versionLabel">The new postfix text</param>
        /// <returns>A new version with an updated postfix text.</returns>
        public static BumpyVersion Label(BumpyVersion version, string versionLabel)
        {
            return new BumpyVersion(version.Numbers, version.Digits, versionLabel, version.NumberDelimiter);
        }

        /// <summary>
        /// Parses a version text into an object of <see cref="BumpyVersion"/>.
        /// </summary>
        /// <param name="versionText">The version text to parse</param>
        /// <returns>A <see cref="BumpyVersion"/> object</returns>
        public static BumpyVersion ParseVersion(string versionText)
        {
            var match = BumpyRegex.Match(versionText);
            var numbersGroup = match.Groups[NumbersGroupName];
            var labelGroup = match.Groups[LabelGroupName];

            if (!numbersGroup.Success || !labelGroup.Success)
            {
                throw new ArgumentException($"The provided version string '{versionText}' is not supported");
            }

            string textNumbers = numbersGroup.Value;
            char numberDelimiter = '.';

            // C++ resource files contain versions in the format a,b,c,d (e.g. 1,0,0,0)
            // This is a rather simple approach, but it should work for now
            if (textNumbers.Contains(",") && !textNumbers.Contains("."))
            {
                numberDelimiter = ',';
            }

            var formattedNumbers = textNumbers.Split(numberDelimiter);
            var numbers = new List<int>(formattedNumbers.Length);
            var digits = new List<int>(formattedNumbers.Length);

            foreach (var formattedNum in formattedNumbers)
            {
                numbers.Add(Convert.ToInt32(formattedNum));
                digits.Add(formattedNum.Length);
            }

            return new BumpyVersion(numbers, digits, labelGroup.Value, numberDelimiter);
        }

        /// <summary>
        /// Finds a version string in a text and tries to create an object of <see cref="BumpyVersion"/>
        /// </summary>
        /// <param name="text">A text which may contain a version text</param>
        /// <param name="regexPattern">The pattern to search the version text</param>
        /// <param name="version">A instance of <see cref="BumpyVersion"/> if a version was found</param>
        /// <param name="marker">A marker which describes where the version was found</param>
        /// <returns>True if a version was found in the given text, else false</returns>
        public static bool TryParseVersionInText(string text, string regexPattern, out BumpyVersion version, out string marker)
        {
            version = null;

            var regex = new Regex(regexPattern, RegexOptions.Singleline);
            var match = regex.Match(text);
            var versionGroup = match.Groups[VersionGroupName];
            var markerGroup = match.Groups[MarkerGroupName];
            var success = versionGroup.Success;

            if (success)
            {
                version = ParseVersion(versionGroup.Value);
            }

            marker = markerGroup.Success ? markerGroup.Value : string.Empty;

            return success;
        }

        /// <summary>
        /// Ensures that a version is found in a given text. Throws an Exception if not.
        /// </summary>
        /// <param name="text">A text which may contain version text</param>
        /// <param name="regexPattern">The pattern to search the version text</param>
        /// <param name="expectedVersion">The version which should be found</param>
        /// <returns>The given text</returns>
        public static string EnsureExpectedVersion(string text, string regexPattern, BumpyVersion expectedVersion)
        {
            if (TryParseVersionInText(text, regexPattern, out var version, out _))
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
