using System;
using System.Collections.Generic;

namespace Bumpy.Version
{
    public class BumpyVersion
    {
        private readonly List<int> _numbers;
        private readonly string _label;

        public BumpyVersion(IEnumerable<int> numbers, string label)
        {
            _numbers = new List<int>(numbers.ThrowIfNull(nameof(numbers)));
            _label = label.ThrowIfNull(nameof(label));

            if (_numbers.Count == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(numbers));
            }

            foreach (var number in _numbers)
            {
                number.ThrowIfCondition(n => n < 0, nameof(numbers), "Elements cannot be negative");
            }
        }

        public BumpyVersion Increment(int position)
        {
            position.ThrowIfCondition(n => n < 1 || n > _numbers.Count, nameof(position), $"Position must be between 1 and {_numbers.Count}");

            var newNumbers = new List<int>(_numbers);
            newNumbers[checked(position - 1)]++;

            for (int i = position; i < newNumbers.Count; i++)
            {
                newNumbers[i] = 0;
            }

            return new BumpyVersion(newNumbers, _label);
        }

        public BumpyVersion Assign(int position, int number)
        {
            position.ThrowIfCondition(n => n < 1 || n > _numbers.Count, nameof(position), $"Position must be between 1 and {_numbers.Count}");
            number.ThrowIfCondition(n => n < 0, nameof(number), "Number cannot be negative");

            var newNumbers = new List<int>(_numbers);

            newNumbers[checked(position - 1)] = number;

            return new BumpyVersion(newNumbers, _label);
        }

        public override string ToString()
        {
            return string.Join(".", _numbers) + _label;
        }

        public override bool Equals(object obj)
        {
            var version = obj as BumpyVersion;

            if (version != null)
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
