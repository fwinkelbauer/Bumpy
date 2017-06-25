using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bumpy.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bumpy.UnitTests
{
    [TestClass]
    public class CommandsTests
    {
        [TestMethod]
        public void CommandCreateConfig_CreatesFile()
        {
            var directory = new DirectoryInfo(".");
            var fileUtil = Substitute.For<IFileUtil>();
            var commands = CreateCommands(fileUtil: fileUtil, directory: directory);

            commands.CommandCreateConfig();

            fileUtil.Received().CreateConfig(directory);
        }

        [TestMethod]
        public void CommandPrintHelp_WritesOutput()
        {
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(writeAction: writeAction);

            commands.CommandPrintHelp();

            writeAction.Received().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandList_NoOutput()
        {
            var lines = new List<string>() { "some", "text" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandList(CreateConfiguration());

            writeAction.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandList_Output()
        {
            var lines = new List<string>() { "aaaaa", "1.2.3", "1.2.a", "1.22.333", "3.2a" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandList(CreateConfiguration());

            writeAction.Received().Invoke(@"\file (1): 1.2.3");
            writeAction.Received().Invoke(@"\file (3): 1.22.333");
        }

        [TestMethod]
        public void CommandIncrement_NoChange()
        {
            var lines = new List<string>() { "some", "text" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandIncrement(CreateConfiguration(), 1);

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(lines, f)));
            writeAction.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandIncrement_Change()
        {
            var lines = new List<string>() { "aaaaa", "1.2.3", "1.2.a", "1.22.333", "3.2a" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandIncrement(CreateConfiguration(), 1);

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(new List<string>() { "aaaaa", "2.0.0", "1.2.a", "2.0.0", "3.2a" }, f)));
            writeAction.Received().Invoke(@"\file (1): 1.2.3 -> 2.0.0");
            writeAction.Received().Invoke(@"\file (3): 1.22.333 -> 2.0.0");
        }

        [TestMethod]
        public void CommandAssign_NoChange()
        {
            var lines = new List<string>() { "some", "text" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandAssign(CreateConfiguration(), 3, 42);

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(lines, f)));
            writeAction.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandAssign_Change()
        {
            var lines = new List<string>() { "aaaaa", "1.2.3", "1.2.a", "1.22.333", "3.2a" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandAssign(CreateConfiguration(), 3, 42);

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(new List<string>() { "aaaaa", "1.2.42", "1.2.a", "1.22.42", "3.2a" }, f)));
            writeAction.Received().Invoke(@"\file (1): 1.2.3 -> 1.2.42");
            writeAction.Received().Invoke(@"\file (3): 1.22.333 -> 1.22.42");
        }

        [TestMethod]
        public void CommandWrite_NoChange()
        {
            var lines = new List<string>() { "some", "text" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandWrite(CreateConfiguration(), "4.2.4");

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(lines, f)));
            writeAction.DidNotReceive().Invoke(Arg.Any<string>());
        }

        [TestMethod]
        public void CommandWrite_Change()
        {
            var lines = new List<string>() { "aaaaa", "1.2.3", "1.2.a", "1.22.333", "3.2a" };
            var fileUtil = CreateFileUtil(lines);
            var writeAction = Substitute.For<Action<string>>();
            var commands = CreateCommands(fileUtil: fileUtil, writeAction: writeAction);

            commands.CommandWrite(CreateConfiguration(), "4.2.4");

            fileUtil.Received().WriteFiles(Arg.Is<IEnumerable<FileContent>>(f => VerifyFileContents(new List<string>() { "aaaaa", "4.2.4", "1.2.a", "4.2.4", "3.2a" }, f)));
            writeAction.Received().Invoke(@"\file (1): 1.2.3 -> 4.2.4");
            writeAction.Received().Invoke(@"\file (3): 1.22.333 -> 4.2.4");
        }

        private static IFileUtil CreateFileUtil(List<string> lines)
        {
            var fileUtil = Substitute.For<IFileUtil>();
            var file = new FileInfo("file");

            fileUtil.GetFiles(Arg.Any<DirectoryInfo>(), Arg.Any<string>()).Returns(new[] { file });
            fileUtil.ReadFile(Arg.Any<FileInfo>(), Arg.Any<Encoding>()).Returns(new FileContent(file, lines, new UTF8Encoding(false)));

            return fileUtil;
        }

        private static bool VerifyFileContents(List<string> expectedLines, IEnumerable<FileContent> actualContent)
        {
            foreach (var content in actualContent)
            {
                CollectionAssert.AreEqual(expectedLines, content.Lines.ToList());
            }

            return true;
        }

        private static IEnumerable<BumpyConfiguration> CreateConfiguration()
        {
            return new List<BumpyConfiguration>()
            {
                new BumpyConfiguration(BumpyConfiguration.DefaultProfile, "search", @"(?<version>\d+\.\d+\.\d+)", new UTF8Encoding(false))
            };
        }

        private static Commands CreateCommands(DirectoryInfo directory = null, IFileUtil fileUtil = null, Action<string> writeAction = null)
        {
            directory = directory ?? new DirectoryInfo(".");
            fileUtil = fileUtil ?? Substitute.For<IFileUtil>();
            writeAction = writeAction ?? (s => { });

            return new Commands(directory, fileUtil, writeAction);
        }
    }
}
