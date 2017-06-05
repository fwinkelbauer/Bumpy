using System.Collections.Generic;
using System.Text;

namespace Bumpy.Util
{
    public class FileContent
    {
        public FileContent(IEnumerable<string> lines, Encoding encoding)
        {
            Lines = lines;
            Encoding = encoding;
        }

        public IEnumerable<string> Lines { get; }

        public Encoding Encoding { get; }
    }
}
