using System.Collections.Generic;
using System.IO;

namespace Bumpy.Util
{
    public interface IFileUtil
    {
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string searchPattern);

        FileContent ReadFile(FileInfo file);

        void WriteFiles(IEnumerable<FileContent> content);

        IEnumerable<BumpyConfiguration> ReadConfigLazy(DirectoryInfo directory);

        void CreateConfig(DirectoryInfo directory);
    }
}
