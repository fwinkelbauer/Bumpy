namespace Bumpy.Util
{
    public class BumpyConfiguration
    {
        public BumpyConfiguration(string glob, string regex)
        {
            GlobPattern = glob;
            RegularExpression = regex;
        }

        public string GlobPattern { get; }

        public string RegularExpression { get; }
    }
}
