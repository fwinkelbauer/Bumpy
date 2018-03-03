using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bumpy.Config
{
    public static class ConfigIO
    {
        public static string NewConfigFile()
        {
            // TODO fw
            return "# Bumpy configuration file";
        }

        // TODO fw Template Bestimmung und Profil Einschränkung könnte ich für beide ConfigIOs zusammenfassen
        public static IEnumerable<BumpyConfigEntry> ReadConfigFile(IEnumerable<string> lines, string profile)
        {
            return null;
        }
    }
}
