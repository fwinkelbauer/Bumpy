using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Util
{
    public interface IFileUtil
    {
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, string searchPattern);

        FileContent ReadFile(FileInfo file, Encoding encoding);

        void WriteFiles(IEnumerable<FileContent> content);

        IEnumerable<BumpyConfiguration> ReadConfigLazy(DirectoryInfo directory);

        void CreateConfig(DirectoryInfo directory);
    }
}
