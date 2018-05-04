using Xunit;

namespace Bumpy.Core.Tests
{
    public class GlobTests
    {
        [Fact]
        public void Glob_WildcardMatch()
        {
            var glob = new Glob("**/Bumpy/**/AssemblyInfo.cs");

            Assert.True(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.True(glob.IsMatch("./Bumpy/Properties/AssemblyInfo.cs"));
            Assert.True(glob.IsMatch("./Bumpy/AssemblyInfo.cs"));

            Assert.False(glob.IsMatch("./Source/Bumpy.Test/Properties/AssemblyInfo.cs"));
            Assert.False(glob.IsMatch("./Bumpy.Test/Properties/AssemblyInfo.cs"));
        }

        [Fact]
        public void Glob_AnotherWildcardMatch()
        {
            var glob = new Glob("**/AssemblyInfo.cs");

            Assert.True(glob.IsMatch("./AssemblyInfo.cs"));
            Assert.True(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));

            Assert.False(glob.IsMatch("./FooAssemblyInfo.cs"));
            Assert.False(glob.IsMatch("./AssemblyInfo.cs.backup"));
        }

        [Fact]
        public void Glob_PreciseMatch()
        {
            var glob = new Glob("AssemblyInfo.cs");

            Assert.True(glob.IsMatch("AssemblyInfo.cs"));
            Assert.True(glob.IsMatch("./Source/Bumpy/Properties/AssemblyInfo.cs"));
            Assert.True(glob.IsMatch("./FooAssemblyInfo.cs"));

            Assert.False(glob.IsMatch("./AssemblyInfo.cs.backup"));
        }

        [Fact]
        public void Glob_FileSeparatorMatch()
        {
            FileSeparatorTest(new Glob("Properties/AssemblyInfo.cs"));
            FileSeparatorTest(new Glob("Properties\\AssemblyInfo.cs"));
        }

        private void FileSeparatorTest(Glob glob)
        {
            Assert.True(glob.IsMatch("./Properties/AssemblyInfo.cs"));
            Assert.True(glob.IsMatch(".\\Properties\\AssemblyInfo.cs"));
            Assert.True(glob.IsMatch(@".\Properties\AssemblyInfo.cs"));
        }
    }
}
