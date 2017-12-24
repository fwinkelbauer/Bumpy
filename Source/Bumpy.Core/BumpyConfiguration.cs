using System.Text;

namespace Bumpy.Core
{
    public class BumpyConfiguration
    {
        public BumpyConfiguration(string profile, string searchPattern, string regularExpression, Encoding encoding)
        {
            Profile = profile;
            SearchPattern = searchPattern;
            RegularExpression = regularExpression;
            Encoding = encoding;
        }

        public string Profile { get; }

        public string SearchPattern { get; }

        public string RegularExpression { get; }

        public Encoding Encoding { get; }
    }
}
