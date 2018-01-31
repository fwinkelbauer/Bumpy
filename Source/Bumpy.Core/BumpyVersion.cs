using System;
using System.Collections.Generic;
using System.Linq;

namespace Bumpy.Core
{
    public sealed class BumpyVersion
    {
        private readonly int[] _numbers;

        public BumpyVersion(IEnumerable<int> numbers, string label)
        {
            _numbers = numbers.ThrowIfNull(nameof(numbers)).ToArray();
            Label = label.ThrowIfNull(nameof(label));

            if (_numbers.Length == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(numbers));
            }

            foreach (var number in _numbers)
            {
                number.ThrowIfOutOfRange(n => n < 0, nameof(numbers), "Elements cannot be negative");
            }
        }

        public IList<int> Numbers
        {
            get
            {
                return new List<int>(_numbers);
            }
        }

        public string Label
        {
            get;
        }

        public override string ToString()
        {
            return string.Join(".", _numbers) + Label;
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
