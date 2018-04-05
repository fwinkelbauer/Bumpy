using System.IO;
using Xunit;

namespace Bumpy.Core.Tests
{
    public class FileInfoExtensionTests
    {
        [Fact]
        public void ToRelativePath_SimpleCheck()
        {
            var file = new FileInfo(@"C:\tmp\foo.txt");
            var relativePath = file.ToRelativePath(file.Directory);

            Assert.Equal("foo.txt", relativePath);
        }

        [Fact]
        public void ToRelativePath_AdvancedCheck()
        {
            var file = new FileInfo(@"C:\project\source\foo.txt");
            var directory = new DirectoryInfo("C:/");
            var relativePath = file.ToRelativePath(directory);

            Assert.Equal(@"project\source\foo.txt", relativePath);
        }
    }
}
