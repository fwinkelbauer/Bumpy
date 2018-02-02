using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests
{
    [TestClass]
    public class FileInfoExtensionTests
    {
        [TestMethod]
        public void ToRelativePath_SimpleCheck()
        {
            var file = new FileInfo(@"C:\tmp\foo.txt");
            var relativePath = file.ToRelativePath(file.Directory);

            Assert.AreEqual("foo.txt", relativePath);
        }

        [TestMethod]
        public void ToRelativePath_AdvancedCheck()
        {
            var file = new FileInfo(@"C:\project\source\foo.txt");
            var directory = new DirectoryInfo("C:/");
            var relativePath = file.ToRelativePath(directory);

            Assert.AreEqual(@"project\source\foo.txt", relativePath);
        }
    }
}
