using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Bumpy.Config
{
    // TODO tests
    public static class ConfigIO
    {
        public static string NewConfigFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("# Bumpy configuration file");
            builder.AppendLine();
            builder.AppendLine("# Example configuration for .NET Framework projects:");
            builder.AppendLine("- glob: AssemblyInfo.cs");
            builder.AppendLine("  profile: assembly");
            builder.AppendLine();
            builder.AppendLine("- glob: \"*.nuspec\"");
            builder.AppendLine("  profile: nuspec");
            builder.AppendLine();
            builder.AppendLine("# Example configuration for .NET Standard/Core projects:");
            builder.AppendLine("- glob: \"*.csproj\"");
            builder.AppendLine("  profile: nuspec");

            return builder.ToString();
        }

        public static IEnumerable<BumpyConfigEntry> ReadConfigFile(TextReader reader)
        {
            try
            {
                var deserializer = new Deserializer();

                return deserializer.Deserialize<List<BumpyConfigEntry>>(reader);
            }
            catch (YamlException e)
            {
                throw new ConfigException(e.Message, e);
            }
        }
    }
}
