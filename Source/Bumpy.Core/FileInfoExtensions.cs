using System;
using System.IO;

namespace Bumpy.Core
{
    /// <summary>
    /// A set of extension methods for <see cref="FileInfo"/> objects.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Creates a string containing the relative path of a file.
        /// </summary>
        /// <param name="file">The file for which we want a relative path</param>
        /// <param name="parent">The directory on which the relative path is based on</param>
        /// <returns>The relative path of a file</returns>
        public static string ToRelativePath(this FileInfo file, DirectoryInfo parent)
        {
            var fileName = file.FullName;
            var parentName = parent.FullName;

            if (!fileName.StartsWith(parentName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"'{parentName}' is not a parent directory of '{fileName}'");
            }

            int count = parentName.Length;

            if (!parentName.EndsWith("/", StringComparison.OrdinalIgnoreCase)
                && !parentName.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }

            return fileName.Substring(count);
        }
    }
}
