using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bumpy.Core.Config
{
    /// <summary>
    /// A template is used to provide default parameters for a
    /// <see cref="BumpyConfigEntry"/> instance for known file types.
    /// </summary>
    internal sealed class Template
    {
        private static readonly Dictionary<string, Template> Templates = new Dictionary<string, Template>()
        {
            { ".csproj", new Template(@"<(?<marker>[Vv]ersion)>(?<version>\d+\.\d+\.\d+.*)<\/[Vv]ersion>", new UTF8Encoding(false)) },
            { ".nuspec", new Template(@"<(?<marker>[Vv]ersion)>(?<version>\d+\.\d+\.\d+.*)<\/[Vv]ersion>", new UTF8Encoding(false)) },
            { "AssemblyInfo.cs", new Template(@"(?<marker>Assembly(File)?Version).*(?<version>\d+\.\d+\.\d+\.\d+)", new UTF8Encoding(true)) },
            { ".cs", new Template("(?<tag>(FILEVERSION|PRODUCTVERSION|FileVersion|ProductVersion))[\", ]*(?<version>\\d+[\\.,]\\d+[\\.,]\\d+[\\.,]\\d+)", Encoding.GetEncoding(1200)) }
        };

        private Template(string regularExpression, Encoding encoding)
        {
            Regex = regularExpression;
            Encoding = encoding;
        }

        /// <summary>
        /// Gets the regular expression.
        /// </summary>
        public string Regex { get; }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Tries to find a template based on the input text.
        /// </summary>
        /// <param name="text">The given text</param>
        /// <param name="template">A template if one is registered for the given text</param>
        /// <returns>True if a template was found, else false</returns>
        public static bool TryFindTemplate(string text, out Template template)
        {
            var matches = Templates.Where(t => text.EndsWith(t.Key, StringComparison.OrdinalIgnoreCase)).ToList();
            template = null;

            if (matches.Any())
            {
                template = matches[0].Value;
                return true;
            }

            return false;
        }
    }
}
