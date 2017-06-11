using System.Collections.Generic;
using System.IO;

namespace Bumpy.Util
{
    public class FileContent
    {
        public FileContent(FileInfo file, IEnumerable<string> lines)
        {
            File = file;
            Lines = lines;
        }

        public FileInfo File { get; }

        public IEnumerable<string> Lines { get; }
    }
}
