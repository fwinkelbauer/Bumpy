using System;
using System.Collections.Generic;
using System.Linq;

namespace Bumpy
{
    public sealed class BumpyVersion
    {
        private readonly List<int> _numbers;
        private readonly List<int> _digits;

        public BumpyVersion(string[] formattedNumbers, string label, char numberDelimiter)
        {
            _numbers = formattedNumbers.Select(fn => Convert.ToInt32(fn)).ToList();
            _digits = new List<int>(_numbers.Count);
            Label = label;
            NumberDelimiter = numberDelimiter;

            if (_numbers.Count == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(formattedNumbers));
            }

            for (int i = 0; i < _numbers.Count; i++)
            {
                _numbers[i].ThrowIfOutOfRange(n => n < 0, nameof(formattedNumbers), "Elements cannot be negative");

                _digits.Add(formattedNumbers[i].Length);
            }
        }

        internal BumpyVersion(IEnumerable<int> numbers, IEnumerable<int> digits, string label, char numberDelimiter)
        {
            _numbers = numbers.ToList();
            _digits = digits.ToList();
            Label = label;
            NumberDelimiter = numberDelimiter;
        }

        public IList<int> Numbers => new List<int>(_numbers);

        public IList<int> Digits => new List<int>(_digits);

        public string Label
        {
            get;
        }

        public char NumberDelimiter
        {
            get;
        }

        public override string ToString()
        {
            var formattedNumbers = new string[_numbers.Count];

            for (int i = 0; i < _numbers.Count; i++)
            {
                formattedNumbers[i] = _numbers[i].ToString($"D{_digits[i]}");
            }

            return string.Join(string.Empty + NumberDelimiter, formattedNumbers) + Label;
        }

        public override bool Equals(object obj)
        {
            if (obj is BumpyVersion version)
            {
                return version.ToString().Equals(ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
