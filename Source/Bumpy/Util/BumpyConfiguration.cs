using System.Text;

namespace Bumpy.Util
{
    public class BumpyConfiguration
    {
        public BumpyConfiguration(string searchPattern, string regularExpression, Encoding encoding)
        {
            SearchPattern = searchPattern;
            RegularExpression = regularExpression;
            Encoding = encoding;
        }

        public string SearchPattern { get; }

        public string RegularExpression { get; }

        public Encoding Encoding { get; }
    }
}
