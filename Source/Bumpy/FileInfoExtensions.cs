using System;
using System.IO;

namespace Bumpy
{
    public static class FileInfoExtensions
    {
        public static string ToRelativePath(this FileInfo file, DirectoryInfo parent)
        {
            var fileName = file.FullName;
            var parentName = parent.FullName;

            if (!fileName.StartsWith(parentName))
            {
                throw new ArgumentException($"'{parentName}' is not a parent directory of '{fileName}'");
            }

            int count = parentName.Length;

            if (!parentName.EndsWith("/") && !parentName.EndsWith("\\"))
            {
                count++;
            }

            return fileName.Substring(count);
        }
    }
}
