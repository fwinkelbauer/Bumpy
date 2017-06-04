using System.Collections.Generic;
using System.IO;

namespace Bumpy.Util
{
    public interface IFileUtil
    {
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string globPattern);

        IEnumerable<string> ReadLines(FileInfo file);

        void WriteLines(FileInfo file, IEnumerable<string> lines);
    }
}
