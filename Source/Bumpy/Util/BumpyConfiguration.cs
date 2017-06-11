namespace Bumpy.Util
{
    public class BumpyConfiguration
    {
        public BumpyConfiguration(string searchPattern, string regularExpression)
        {
            SearchPattern = searchPattern;
            RegularExpression = regularExpression;
        }

        public string SearchPattern { get; }

        public string RegularExpression { get; }
    }
}
