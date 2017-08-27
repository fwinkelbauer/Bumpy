namespace Bumpy.Util
{
    public class GlobUtil
    {
        private readonly Glob.Glob _glob;

        public GlobUtil(string searchPattern)
        {
            _glob = new Glob.Glob(searchPattern);
        }

        public bool IsMatch(string input)
        {
            return _glob.IsMatch(input);
        }
    }
}
