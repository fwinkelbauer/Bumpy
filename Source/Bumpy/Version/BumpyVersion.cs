using System;
using System.Collections.Generic;

namespace Bumpy.Version
{
    public class BumpyVersion
    {
        private List<int> _parts;

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
            position.ThrowIfCondition(n => n < 0 || n >= _parts.Count, nameof(position), $"Position must be between 0 and {_parts.Count - 1}");

            var newParts = new List<int>(_parts);
            newParts[position]++;

            for (int i = checked(position + 1); i < newParts.Count; i++)
            {
                newParts[i] = 0;
            }

            return new BumpyVersion(newParts);
        }

        public BumpyVersion Assign(int position, int number)
        {
            position.ThrowIfCondition(n => n < 0 || n >= _parts.Count, nameof(position), $"Position must be between 0 and {_parts.Count - 1}");
            number.ThrowIfCondition(n => n < 0, nameof(position), "Number cannot be negative");

            var newParts = new List<int>(_parts);

            newParts[position] = number;

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
