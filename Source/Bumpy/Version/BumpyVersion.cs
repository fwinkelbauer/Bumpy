using System;
using System.Collections.Generic;

namespace Bumpy.Version
{
    public class BumpyVersion
    {
        private readonly List<int> _parts;

        public BumpyVersion(IEnumerable<int> parts)
        {
            _parts = new List<int>(parts.ThrowIfNull(nameof(parts)));

            if (_parts.Count == 0)
            {
                throw new ArgumentException("Parameter must at least contain one element", nameof(parts));
            }

            foreach (var part in _parts)
            {
                part.ThrowIfCondition(n => n < 0, nameof(parts), "Elements cannot be negative");
            }
        }

        public BumpyVersion Increment(int position)
        {
            position.ThrowIfCondition(n => n < 1 || n > _parts.Count, nameof(position), $"Position must be between 1 and {_parts.Count}");

            var newParts = new List<int>(_parts);
            newParts[checked(position - 1)]++;

            for (int i = position; i < newParts.Count; i++)
            {
                newParts[i] = 0;
            }

            return new BumpyVersion(newParts);
        }

        public BumpyVersion Assign(int position, int number)
        {
            position.ThrowIfCondition(n => n < 1 || n > _parts.Count, nameof(position), $"Position must be between 1 and {_parts.Count}");
            number.ThrowIfCondition(n => n < 0, nameof(number), "Number cannot be negative");

            var newParts = new List<int>(_parts);

            newParts[checked(position - 1)] = number;

            return new BumpyVersion(newParts);
        }

        public override string ToString()
        {
            return string.Join(".", _parts);
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
