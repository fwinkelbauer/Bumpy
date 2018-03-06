using System.Text;

namespace Bumpy.Config
{
    public class BumpyConfigEntry
    {
        public const string DefaultProfile = "";

        public BumpyConfigEntry()
        {
            Glob = string.Empty;
            Profile = DefaultProfile;
            Encoding = new UTF8Encoding(false);
            Regex = string.Empty;
        }

        public string Glob { get; set; }

        public string Profile { get; set; }

        public Encoding Encoding { get; set; }

        public string Regex { get; set; }
    }
}
