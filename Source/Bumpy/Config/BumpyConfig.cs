using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Bumpy.Config
{
    public class BumpyConfig
    {
        [YamlMember(Alias = "queries")]
        public IEnumerable<BumpyConfigEntry> Queries { get; set; }
    }
}
