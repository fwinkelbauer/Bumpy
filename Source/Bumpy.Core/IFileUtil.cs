using System.Collections.Generic;
using System.IO;
using System.Text;
using Bumpy.Core.Config;

namespace Bumpy.Core
{
    /// <summary>
    /// An abstraction for all file system based operations.
    /// </summary>
    public interface IFileUtil
    {
        /// <summary>
        /// Returns all files in a directory which match a glob pattern.
        /// </summary>
        /// <param name="directory">The base directory</param>
        /// <param name="glob">The search glob pattern</param>
        /// <returns>A list of files which match the glob pattern</returns>
        IEnumerable<FileInfo> GetFiles(DirectoryInfo directory, Glob glob);

        /// <summary>
        /// Returns the content of a file.
        /// </summary>
        /// <param name="file">The file name</param>
        /// <param name="encoding">The encoding with which the file should be read</param>
        /// <returns>A representation of the file, it's encoding and the read content</returns>
        FileContent ReadFileContent(FileInfo file, Encoding encoding);

        /// <summary>
        /// Writes a file to disk.
        /// </summary>
        /// <param name="fileContent">The file, it's encoding and content which should be written</param>
        void WriteFileContent(FileContent fileContent);

        /// <summary>
        /// Creates a default config file for Bumpy.
        /// </summary>
        /// <param name="configFile">The location of the config file to create</param>
        /// <returns>True if the file was created, false if it did already exist</returns>
        bool CreateConfigFile(FileInfo configFile);

        /// <summary>
        /// Reads and returns Bumpy's configuration from a file.
        /// </summary>
        /// <param name="configFile">The config file to read</param>
        /// <param name="profile">A profile which should filter the return set</param>
        /// <returns>All read configuration of a given profile</returns>
        IEnumerable<BumpyConfigEntry> ReadConfigFile(FileInfo configFile, string profile);
    }
}
