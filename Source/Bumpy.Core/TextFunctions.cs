using System;

namespace Bumpy.Core
{
    public static class TextFunctions
    {
        public static string EnsureExpectedVersion(string text, string regexPattern, BumpyVersion expectedVersion)
        {
            if (VersionFunctions.TryParseVersionInText(text, regexPattern, out var version))
            {
                if (version.Equals(expectedVersion))
                {
                    return text;
                }
            }

            throw new InvalidOperationException($"The provided version '{expectedVersion}' cannot be captured in the text '{text}' using the regex '{regexPattern}'. Please correct either the version or the regex");
        }
    }
}
