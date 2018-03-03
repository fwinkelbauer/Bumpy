using System.Collections.Generic;
using System.IO;
using System.Text;
using Bumpy.Config;

namespace Bumpy
{
    public interface IFileUtil
    {
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, Glob glob);

        FileContent ReadFileContent(FileInfo file, Encoding encoding);

        void WriteFileContent(FileContent fileContent);

        bool CreateConfigFile(FileInfo configFile);

        IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile, string profile);
    }
}
