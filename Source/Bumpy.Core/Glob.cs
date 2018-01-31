using System.Text.RegularExpressions;

namespace Bumpy.Core
{
    public sealed class Glob
    {
        private readonly Regex _regex;

        public Glob(string searchPattern)
        {
            var unifiedPattern = searchPattern.ThrowIfNull(nameof(searchPattern))
                .Replace("\\", "/");
            var escapedPattern = Regex.Escape(unifiedPattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".")
                .Replace("/", @"[\\\/]");

            _regex = new Regex($"{escapedPattern}$");
        }

        public bool IsMatch(string input)
        {
            return _regex.IsMatch(input);
        }
    }
}
