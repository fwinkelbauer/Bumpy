using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bumpy.Tests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void New_CreatesFile()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            var command = CreateCommand(fileUtil);

            command.New();

            fileUtil.Received().CreateConfigFile(Arg.Any<FileInfo>());
        }

        [TestMethod]
        public void Help_WritesOutput()
        {
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(writeLine: writeLine);

            command.Help();

            writeLine.Received().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void List_NoVersion()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "no", "version", "here" });
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.List();

            writeLine.Received().Invoke(@"foo.txt: no version found");
        }

        [TestMethod]
        public void List_PrintVersions()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.List();

            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0");
        }

        [TestMethod]
        public void Ensure_Success()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "1.2.3", "1.2.3" });
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.Ensure();

            writeLine.Received().Invoke("1.2.3");
        }

        [TestMethod]
        public void Ensure_Error()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "1.2.3", "1.2.4" });
            var command = CreateCommand(fileUtil);

            Assert.ThrowsException<InvalidDataException>(() => command.Ensure());
        }

        [TestMethod]
        public void Increment_NoVersion()
        {
            var lines = new[] { "no", "version", "here" };
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, lines);
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.Increment(2);

            fileUtil.DidNotReceive().WriteFileContent(Arg.Any<FileContent>());
            writeLine.Received().Invoke(@"foo.txt: no version found");
        }

        [TestMethod]
        public void Increment_IncrementVersions()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.Increment(2);

            var newLines = new[] { "some", "version", "here", "1.3.0", "foobar 0.26.0" };
            fileUtil.Received().WriteFileContent(Arg.Is<FileContent>(f => f.Lines.SequenceEqual(newLines)));
            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3 -> 1.3.0");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0 -> 0.26.0");
        }

        [TestMethod]
        public void Increment_IncrementVersionsNoFileWrite()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.Increment(2, new BumpyArguments { NoOperation = true });

            fileUtil.DidNotReceive().WriteFileContent(Arg.Any<FileContent>());
            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3 -> 1.3.0");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0 -> 0.26.0");
        }

        [TestMethod]
        public void Assign_NoWrite()
        {
            var lines = new[] { "1.0.42" };
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, lines);
            var writeLine = Substitute.For<Action<string>>();
            var command = CreateCommand(fileUtil, writeLine);

            command.Assign(3, "42");

            fileUtil.DidNotReceive().WriteFileContent(Arg.Any<FileContent>());
            writeLine.Received().Invoke(@"foo.txt (1): 1.0.42 -> 1.0.42");
        }

        private void PrepareFileUtilSubstitute(IFileUtil fileUtil, IEnumerable<string> lines)
        {
            fileUtil.ReadConfigFile(Arg.Any<FileInfo>(), Arg.Any<string>()).Returns(new[]
            {
                new BumpyConfigEntry
                {
                    Glob = "*.txt",
                    Regex = @"(?<version>\d+\.\d+\.\d+)"
                }
            });

            fileUtil.GetFiles(Arg.Any<DirectoryInfo>(), Arg.Any<Glob>()).Returns(new[]
            {
                new FileInfo("foo.txt")
            });

            fileUtil.ReadFileContent(Arg.Any<FileInfo>(), Arg.Any<Encoding>()).Returns(
                new FileContent(new FileInfo("foo.txt"), lines, new UTF8Encoding(false)));
        }

        private Command CreateCommand(IFileUtil fileUtil = null, Action<string> writeLine = null)
        {
            fileUtil = fileUtil ?? Substitute.For<IFileUtil>();
            writeLine = writeLine ?? (l => { });

            return new Command(fileUtil, writeLine);
        }
    }
}
