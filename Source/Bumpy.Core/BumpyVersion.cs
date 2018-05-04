using System;
using System.Collections.Generic;
using System.Linq;

namespace Bumpy.Core
{
    /// <summary>
    /// A class which represents a version string.
    ///
    /// Examples:
    /// - 1.0.0.0
    /// - 1.2.3-beta
    /// - 1.2.3-beta+foo
    /// - 01.01.2018
    /// - 8,3,4,0
    /// </summary>
    public sealed class BumpyVersion
    {
        private readonly List<int> _numbers;
        private readonly List<int> _digits;

        /// <summary>
        /// Initializes a new instance of the <see cref="BumpyVersion"/> class.
        /// </summary>
        /// <param name="numbers">A collection of version number parts, e.g. { 1, 23, 8025 }</param>
        /// <param name="digits">A collection of numbers describing the length of all members in the "numbers" collection, e.g. { 1, 2, 4 }</param>
        /// <param name="label">The label suffix of a version, e.g. "-beta"</param>
        /// <param name="numberDelimiter">The delimiter used to display a version string, e.g. "." -> "1.23.8025</param>
        public BumpyVersion(IEnumerable<int> numbers, IEnumerable<int> digits, string label, char numberDelimiter)
        {
            _numbers = numbers.ToList();
            _digits = digits.ToList();
            Label = label;
            NumberDelimiter = numberDelimiter;

            if (_numbers.Count == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(numbers));
            }

            if (_numbers.Count != _digits.Count)
            {
                throw new ArgumentException($"Length mismatch found in collections {nameof(numbers)} and {nameof(digits)}");
            }

            for (int i = 0; i < _numbers.Count; i++)
            {
                _numbers[i].ThrowIfOutOfRange(n => n < 0, nameof(numbers), "Elements cannot be negative");
                _digits[i].ThrowIfOutOfRange(n => n < 0, nameof(digits), "Elements cannot be negative");
            }
        }

        /// <summary>
        /// Gets the numbers of a version, e.g. { 1, 24, 8025 }.
        /// </summary>
        public IList<int> Numbers => new List<int>(_numbers);

        /// <summary>
        /// Gets the digit count for each member of the Numbers collection.
        ///
        /// Example #1:
        /// - Numbers: { 1, 24, 8025 }
        /// - Digits: { 1, 2, 4 }
        /// - Results in version "1.24.8025"
        ///
        /// Example #2:
        /// - Numbers: { 1, 1, 2018 }
        /// - Digits: { 2, 2, 4 }
        /// - Results in version "01.01.2018"
        /// </summary>
        public IList<int> Digits => new List<int>(_digits);

        /// <summary>
        /// Gets the suffix version label, e.g. "-beta" or "".
        /// </summary>
        public string Label
        {
            get;
        }

        /// <summary>
        /// Gets the delimiter used to display the numbers of a version, e.g. "." -> "1.2.3".
        /// </summary>
        public char NumberDelimiter
        {
            get;
        }

        /// <summary>
        /// Creates a readable representation of a version.
        ///
        /// Examples:
        /// - 14.31.6532.4
        /// - 1.2.3-beta
        /// - 01.01.2018
        /// - 1,0,0,0
        /// </summary>
        /// <returns>A readable version string</returns>
        public override string ToString()
        {
            var formattedNumbers = new string[_numbers.Count];

            for (int i = 0; i < _numbers.Count; i++)
            {
                formattedNumbers[i] = _numbers[i].ToString($"D{_digits[i]}");
            }

            return string.Join(string.Empty + NumberDelimiter, formattedNumbers) + Label;
        }

        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True if all members are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is BumpyVersion version)
            {
                return version.ToString().Equals(ToString());
            }

            return false;
        }

        /// <summary>
        /// Creates a hash code based on the object's state.
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
