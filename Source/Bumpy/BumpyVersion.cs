using System;
using System.Linq;

namespace Bumpy
{
    public sealed class BumpyVersion
    {
        private readonly int[] _numbers;
        private readonly int[] _digits;

        public BumpyVersion(string[] formattedNumbers, string label)
        {
            _numbers = formattedNumbers.Select(fn => Convert.ToInt32(fn)).ToArray();
            _digits = new int[_numbers.Length];
            Label = label;

            if (_numbers.Length == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(formattedNumbers));
            }

            for (int i = 0; i < _numbers.Length; i++)
            {
                _numbers[i].ThrowIfOutOfRange(n => n < 0, nameof(formattedNumbers), "Elements cannot be negative");

                _digits[i] = formattedNumbers[i].Length;
            }
        }

        internal BumpyVersion(int[] numbers, int[] digits, string label)
        {
            _numbers = numbers;
            _digits = digits;
            Label = label;
        }

        public int[] Numbers => _numbers.ToArray();

        public int[] Digits => _digits.ToArray();

        public string Label
        {
            get;
        }

        public override string ToString()
        {
            var formattedNumbers = new string[_numbers.Length];

            for (int i = 0; i < _numbers.Length; i++)
            {
                formattedNumbers[i] = _numbers[i].ToString($"D{_digits[i]}");
            }

            return string.Join(".", formattedNumbers) + Label;
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
