using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Bumpy.Config
{
    public class BumpyConfig
    {
        public const string ConfigFile = "bumpy.yaml";
        public const string LegacyConfigFile = ".bumpyconfig";

        [YamlMember(Alias = "queries")]
        public IEnumerable<BumpyConfigEntry> Queries { get; set; }
    }
}
