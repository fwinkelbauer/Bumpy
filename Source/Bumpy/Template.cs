using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bumpy
{
    internal sealed class Template
    {
        private static readonly Dictionary<string, Template> Templates = new Dictionary<string, Template>()
        {
            { ".csproj", new Template(@"<(?<marker>[Vv]ersion)>(?<version>\d+\.\d+\.\d+.*)<\/[Vv]ersion>", new UTF8Encoding(false)) },
            { ".nuspec", new Template(@"<(?<marker>[Vv]ersion)>(?<version>\d+\.\d+\.\d+.*)<\/[Vv]ersion>", new UTF8Encoding(false)) },
            { "AssemblyInfo.cs", new Template(@"(?<marker>Assembly(File)?Version).*(?<version>\d+\.\d+\.\d+\.\d+)", new UTF8Encoding(true)) }
        };

        private Template(string regularExpression, Encoding encoding)
        {
            Regex = regularExpression;
            Encoding = encoding;
        }

        public string Regex { get; }

        public Encoding Encoding { get; }

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
