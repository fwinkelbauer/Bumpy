using System.Collections.Generic;

namespace Bumpy.Config
{
    public class BumpyConfig
    {
        public const string ConfigFile = ".bumpyconfig";

        public IEnumerable<BumpyConfigEntry> Queries { get; set; }
    }
}
