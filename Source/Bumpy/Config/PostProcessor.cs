using System;

namespace Bumpy.Config
{
    // TODO test
    internal static class PostProcessor
    {
        public static BumpyConfigEntry Process(BumpyConfigEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Glob))
            {
                // TODO
                throw new Exception("Glob");
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
                return new BumpyConfigEntry(entry.Glob, entry.Profile, template.Encoding, template.Regex);
            }

            // TODO
            throw new Exception();
        }
    }
}
