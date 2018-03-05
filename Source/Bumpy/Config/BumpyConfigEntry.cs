using System.Text;
using YamlDotNet.Serialization;

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

        [YamlMember(Alias = "glob")]
        public string Glob { get; set; }

        [YamlMember(Alias = "profile")]
        public string Profile { get; set; }

        [YamlMember(Alias = "encoding")]
        public Encoding Encoding { get; set; }

        [YamlMember(Alias = "regex")]
        public string Regex { get; set; }
    }
}
