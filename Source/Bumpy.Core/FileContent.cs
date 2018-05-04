using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumpy.Core
{
    /// <summary>
    /// A class which encapsulates a file, its encoding and its content
    /// </summary>
    public sealed class FileContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileContent"/> class.
        /// </summary>
        /// <param name="file">The underlying file</param>
        /// <param name="lines">The file content</param>
        /// <param name="encoding">The encoding used to read the file content</param>
        public FileContent(FileInfo file, IEnumerable<string> lines, Encoding encoding)
        {
            File = file;
            Lines = lines;
            Encoding = encoding;
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> object.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Gets the file content.
        /// </summary>
        public IEnumerable<string> Lines { get; }

        /// <summary>
        /// Gets the file encoding.
        /// </summary>
        public Encoding Encoding { get; }
    }
}
