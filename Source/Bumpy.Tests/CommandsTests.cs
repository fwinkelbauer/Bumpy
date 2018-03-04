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
    public class CommandsTests
    {
        [TestMethod]
        public void CommandNew_CreatesFile()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            var commands = CreateCommands(fileUtil);

            commands.CommandNew();

            fileUtil.Received().CreateConfigFile(Arg.Any<FileInfo>());
        }

        [TestMethod]
        public void CommandHelp_WritesOutput()
        {
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(writeLine: writeLine);

            commands.CommandHelp();

            writeLine.Received().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandList_NoVersion()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "no", "version", "here" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandList(string.Empty);

            writeLine.Received().Invoke(@"foo.txt: no version found");
        }

        [TestMethod]
        public void CommandList_PrintVersions()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandList(string.Empty);

            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0");
        }

        [TestMethod]
        public void CommandEnsure_Success()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "1.2.3", "1.2.3" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandEnsure(string.Empty);

            writeLine.Received().Invoke("Success");
        }

        [TestMethod]
        public void CommandEnsure_Error()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "1.2.3", "1.2.4" });
            var commands = CreateCommands(fileUtil);

            Assert.ThrowsException<InvalidDataException>(() => commands.CommandEnsure(string.Empty));
        }

        [TestMethod]
        public void CommandIncrement_NoVersion()
        {
            var lines = new[] { "no", "version", "here" };
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, lines);
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandIncrement(string.Empty, 2);

            fileUtil.DidNotReceive().WriteFileContent(Arg.Any<FileContent>());
            writeLine.Received().Invoke(@"foo.txt: no version found");
        }

        [TestMethod]
        public void CommandIncrement_IncrementVersions()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandIncrement(string.Empty, 2);

            var newLines = new[] { "some", "version", "here", "1.3.0", "foobar 0.26.0" };
            fileUtil.Received().WriteFileContent(Arg.Is<FileContent>(f => f.Lines.SequenceEqual(newLines)));
            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3 -> 1.3.0");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0 -> 0.26.0");
        }

        [TestMethod]
        public void CommandIncrement_IncrementVersionsNoFileWrite()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "some", "version", "here", "1.2.3", "foobar 0.25.0" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine, true);

            commands.CommandIncrement(string.Empty, 2);

            fileUtil.DidNotReceive().WriteFileContent(Arg.Any<FileContent>());
            writeLine.Received().Invoke(@"foo.txt (4): 1.2.3 -> 1.3.0");
            writeLine.Received().Invoke(@"foo.txt (5): 0.25.0 -> 0.26.0");
        }

        [TestMethod]
        public void CommandAssign_NoWrite()
        {
            var lines = new[] { "1.0.42" };
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, lines);
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandAssign(string.Empty, 3, "42");

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

        private Commands CreateCommands(IFileUtil fileUtil = null, Action<string> writeLine = null, bool noOperation = false)
        {
            fileUtil = fileUtil ?? Substitute.For<IFileUtil>();
            writeLine = writeLine ?? (l => { });

            return new Commands(fileUtil, new FileInfo("bumpy.yaml"), new DirectoryInfo("."), noOperation, writeLine);
        }
    }
}
