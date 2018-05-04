using System.IO;
using Bumpy.Core.Config;

namespace Bumpy.Core
{
    /// <summary>
    /// An arguments object which can be used to change the behavior
    /// of Bumpy's major commands.
    /// </summary>
    public sealed class BumpyArguments
    {
        /// <summary>
        /// Gets or sets the path to a configuration file (e.g.: ".bumpyconfig").
        /// </summary>
        public FileInfo ConfigFile { get; set; } = new FileInfo(BumpyConfigEntry.DefaultConfigFile);

        /// <summary>
        /// Gets or sets the working directory (e.g. ".").
        /// </summary>
        public DirectoryInfo WorkingDirectory { get; set; } = new DirectoryInfo(".");

        /// <summary>
        /// Gets or sets a value indicating whether the "no operation" (preview) mode
        /// should be used. Enabling this flag prevents Bumpy's core operations
        /// from changing files on disk.
        /// </summary>
        public bool NoOperation { get; set; }

        /// <summary>
        /// Gets or sets the profile named used in Bumpy's core operations.
        /// Profiles can be defined in Bumpy's configuration file.
        /// </summary>
        public string Profile { get; set; } = BumpyConfigEntry.DefaultProfile;
    }
}
