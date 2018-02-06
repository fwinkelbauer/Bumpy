using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public void CommandList_NoOutput()
        {
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, new[] { "no", "version", "here" });
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandList(string.Empty);

            writeLine.DidNotReceive().Invoke(Arg.Any<string>());
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
        public void CommandIncrement_NoChange()
        {
            var lines = new[] { "no", "version", "here" };
            var fileUtil = Substitute.For<IFileUtil>();
            PrepareFileUtilSubstitute(fileUtil, lines);
            var writeLine = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil, writeLine);

            commands.CommandIncrement(string.Empty, 2);

            fileUtil.DidNotReceive().WriteFileContent(Arg.Is<FileContent>(f => f.Lines.SequenceEqual(lines)));
            writeLine.DidNotReceive().Invoke(Arg.Any<string>());
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

        private void PrepareFileUtilSubstitute(IFileUtil fileUtil, IEnumerable<string> lines)
        {
            fileUtil.ReadConfigFile(Arg.Any<FileInfo>(), Arg.Any<string>()).Returns(new[]
            {
                new BumpyConfiguration(
                    string.Empty,
                    "*.txt",
                    @"(?<version>\d+\.\d+\.\d+)",
                    new UTF8Encoding(false))
            });

            fileUtil.GetFiles(Arg.Any<DirectoryInfo>(), Arg.Any<Glob>()).Returns(new[]
            {
                new FileInfo("foo.txt")
            });

            fileUtil.ReadFileContent(Arg.Any<FileInfo>(), Arg.Any<Encoding>()).Returns(
                new FileContent(new FileInfo("foo.txt"), lines, new UTF8Encoding(false)));
        }

        private Commands CreateCommands(IFileUtil fileUtil = null, Action<string> writeLine = null)
        {
            fileUtil = fileUtil ?? Substitute.For<IFileUtil>();
            writeLine = writeLine ?? (l => { });

            return new Commands(fileUtil, new FileInfo(".bumpyconfig"), new DirectoryInfo("."), writeLine);
        }
    }
}
