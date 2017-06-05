using System.Collections.Generic;
using System.IO;

namespace Bumpy.Util
{
    public interface IFileUtil
    {
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string globPattern);

        FileContent ReadFile(FileInfo file);

        void WriteFile(FileInfo file, FileContent content);

        IEnumerable<BumpyConfiguration> ReadConfig(DirectoryInfo directory);

        void CreateConfig(DirectoryInfo directory);
    }
}
