using System.IO;

namespace Bumpy.Core
{
    internal static class FileInfoExtensions
    {
        public static string ToRelativePath(this FileInfo file)
        {
            return file.FullName.Substring(file.Directory.FullName.Length);
        }
    }
}
