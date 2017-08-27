using Bumpy.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.UnitTests.Util
{
    [TestClass]
    public class GlobUtilTests
    {
        [TestMethod]
        public void Glob_WildcardMatch()
        {
            var glob = new GlobUtil("**/Bumpy/**/AssemblyInfo.cs");

            Assert.IsTrue(glob.IsMatch("Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("Bumpy/Properties/AssemblyInfo.cs"));

            Assert.IsFalse(glob.IsMatch("Source/Bumpy.Test/Properties/AssemblyInfo.cs"));
            Assert.IsFalse(glob.IsMatch("Bumpy.Test/Properties/AssemblyInfo.cs"));
        }

        [TestMethod]
        public void Glob_GlobalMatch()
        {
            var glob = new GlobUtil("AssemblyInfo.cs");

            Assert.IsTrue(glob.IsMatch("Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("AssemblyInfo.cs"));
        }
    }
}
