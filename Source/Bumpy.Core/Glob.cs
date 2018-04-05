using System.Text.RegularExpressions;

namespace Bumpy.Core
{
    /// <summary>
    /// A glob pattern class.
    /// </summary>
    public sealed class Glob
    {
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Glob"/> class.
        /// </summary>
        /// <param name="searchPattern">The glob pattern</param>
        public Glob(string searchPattern)
        {
            var unifiedPattern = searchPattern.Replace("\\", "/");
            var escapedPattern = Regex.Escape(unifiedPattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".")
                .Replace("/.*.*/", "/(.*.*/)?")
                .Replace("/", @"[\\\/]");

            _regex = new Regex($"{escapedPattern}$");
        }

        /// <summary>
        /// Returns true if the input matches the given glob pattern.
        /// </summary>
        /// <param name="input">An input to check</param>
        /// <returns>True if the input matches the given glob pattern, else false</returns>
        public bool IsMatch(string input)
        {
            return _regex.IsMatch(input);
        }
    }
}
