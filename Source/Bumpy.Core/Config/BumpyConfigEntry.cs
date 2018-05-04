using System.Text;

namespace Bumpy.Core.Config
{
    /// <summary>
    /// A simple configuration class.
    /// </summary>
    public class BumpyConfigEntry
    {
        /// <summary>
        /// The name of a default Bumpy configuration file.
        /// </summary>
        public const string DefaultConfigFile = ".bumpyconfig";

        /// <summary>
        /// The default profile in a Bumpy configuration file if the user does
        /// not specify a profile.
        /// </summary>
        public const string DefaultProfile = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="BumpyConfigEntry"/> class.
        /// </summary>
        public BumpyConfigEntry()
        {
            Glob = string.Empty;
            Profile = DefaultProfile;
            Encoding = new UTF8Encoding(false);
            Regex = string.Empty;
        }

        /// <summary>
        /// Gets or sets the glob pattern.
        /// </summary>
        public string Glob { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the regular expression.
        /// </summary>
        public string Regex { get; set; }
    }
}
