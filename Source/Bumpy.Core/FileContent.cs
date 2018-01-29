using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Core
{
    internal class FileContent
    {
        public FileContent(FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            File = file;
            Lines = lines;
            Encoding = encoding;
        }

        public FileInfo File { get; }

        public IEnumerable<string> Lines { get; }

        public Encoding Encoding { get; }
    }
}
