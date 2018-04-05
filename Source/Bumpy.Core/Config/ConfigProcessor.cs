namespace Bumpy.Core.Config
{
    /// <summary>
    /// A postprocessor used to validate and enrich instances of
    /// <see cref="BumpyConfigEntry"/>
    /// </summary>
    public static class ConfigProcessor
    {
        /// <summary>
        /// Validates instances of <see cref="BumpyConfigEntry"/>. Will also enrich an instance
        /// with additional data.
        /// </summary>
        /// <param name="entry">The object to validate and enrich</param>
        /// <returns>The given object or a new enriched object</returns>
        public static BumpyConfigEntry Process(BumpyConfigEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Glob))
            {
                throw new ConfigException("Glob cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(entry.Regex))
            {
                return ApplyTemplate(entry);
            }

            return entry;
        }

        private static BumpyConfigEntry ApplyTemplate(BumpyConfigEntry entry)
        {
            if (Template.TryFindTemplate(entry.Glob, out var template))
            {
                return new BumpyConfigEntry
                {
                    Glob = entry.Glob,
                    Profile = entry.Profile,
                    Encoding = template.Encoding,
                    Regex = template.Regex
                };
            }

            throw new ConfigException($"Could not find a template for glob '{entry.Glob}'");
        }
    }
}
