using System.IO;

namespace Bumpy
{
    internal static class FileInfoExtensions
    {
        public static string ToRelativePath(this FileInfo file, DirectoryInfo parent)
        {
            return file.FullName.Substring(parent.FullName.Length);
        }
    }
}
