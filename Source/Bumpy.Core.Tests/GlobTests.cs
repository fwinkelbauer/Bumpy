using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Core.Tests
{
    [TestClass]
    public class GlobTests
    {
        [TestMethod]
        public void Glob_WildcardMatch()
        {
            var glob = new Glob("**/Bumpy/**/AssemblyInfo.cs");

            Assert.IsTrue(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("./Bumpy/Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("./Bumpy/AssemblyInfo.cs"));

            Assert.IsFalse(glob.IsMatch("./Source/Bumpy.Test/Properties/AssemblyInfo.cs"));
            Assert.IsFalse(glob.IsMatch("./Bumpy.Test/Properties/AssemblyInfo.cs"));
        }

        [TestMethod]
        public void Glob_AnotherWildcardMatch()
        {
            var glob = new Glob("**/AssemblyInfo.cs");

            Assert.IsTrue(glob.IsMatch("./AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));

            Assert.IsFalse(glob.IsMatch("./FooAssemblyInfo.cs"));
            Assert.IsFalse(glob.IsMatch("./AssemblyInfo.cs.backup"));
        }

        [TestMethod]
        public void Glob_PreciseMatch()
        {
            var glob = new Glob("AssemblyInfo.cs");

            Assert.IsTrue(glob.IsMatch("AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch("./FooAssemblyInfo.cs"));

            Assert.IsFalse(glob.IsMatch("./AssemblyInfo.cs.backup"));
        }

        [TestMethod]
        public void Glob_FileSeparatorMatch()
        {
            FileSeparatorTest(new Glob("Properties/AssemblyInfo.cs"));
            FileSeparatorTest(new Glob("Properties\\AssemblyInfo.cs"));
        }

        private void FileSeparatorTest(Glob glob)
        {
            Assert.IsTrue(glob.IsMatch("./Properties/AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch(".\\Properties\\AssemblyInfo.cs"));
            Assert.IsTrue(glob.IsMatch(@".\Properties\AssemblyInfo.cs"));
        }
    }
}
