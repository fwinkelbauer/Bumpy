using System.Text;

namespace Bumpy.Config
{
    public class BumpyConfigEntry
    {
        public const string ConfigFile = ".bumpyconfig";
        public const string DefaultProfile = "";

        public BumpyConfigEntry(string glob, string profile, Encoding encoding, string regex)
        {
            Glob = glob;
            Profile = profile;
            Encoding = encoding;
            Regex = regex;
        }

        public string Glob { get; }

        public string Profile { get; }

        public Encoding Encoding { get; }

        public string Regex { get; }
    }
}
