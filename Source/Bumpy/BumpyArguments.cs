using System.IO;
using Bumpy.Config;

namespace Bumpy
{
    public sealed class BumpyArguments
    {
        public FileInfo ConfigFile { get; set; } = new FileInfo(BumpyConfig.ConfigFile);

        public DirectoryInfo WorkingDirectory { get; set; } = new DirectoryInfo(".");

        public bool NoOperation { get; set; }

        public string Profile { get; set; } = BumpyConfigEntry.DefaultProfile;
    }
}
