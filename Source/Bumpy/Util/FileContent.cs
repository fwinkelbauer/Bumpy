using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Util
{
    public class FileContent
    {
        public FileContent(FileInfo file, IList<string> lines, Encoding encoding)
        {
            File = file;
            Lines = lines;
            Encoding = encoding;
        }

        public FileInfo File { get; }

        public IList<string> Lines { get; }

        public Encoding Encoding { get; }
    }
}
