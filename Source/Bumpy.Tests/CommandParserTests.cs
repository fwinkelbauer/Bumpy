using System.IO;
using Bumpy.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests
{
    [TestClass]
    public class CommandParserTests
    {
        [TestMethod]
        public void ParseArguments_Help()
        {
            var arguments = Parse("help");

            Assert.AreEqual(CommandType.Help, arguments.CmdType);
        }

        [TestMethod]
        public void ParseArguments_New()
        {
            var arguments = Parse("new");

            Assert.AreEqual(CommandType.New, arguments.CmdType);
        }

        [TestMethod]
        public void ParseArguments_List()
        {
            var arguments = Parse("list");

            Assert.AreEqual(CommandType.List, arguments.CmdType);
        }

        [TestMethod]
        public void ParseArguments_Increment()
        {
            var arguments = Parse("increment 3");

            Assert.AreEqual(CommandType.Increment, arguments.CmdType);
            Assert.AreEqual(3, arguments.Position);
        }

        [TestMethod]
        public void ParseArguments_IncrementOnly()
        {
            var arguments = Parse("incrementonly 3");

            Assert.AreEqual(CommandType.IncrementOnly, arguments.CmdType);
            Assert.AreEqual(3, arguments.Position);
        }

        [TestMethod]
        public void ParseArguments_Assign()
        {
            var arguments = Parse("assign 2 007");

            Assert.AreEqual(CommandType.Assign, arguments.CmdType);
            Assert.AreEqual(2, arguments.Position);
            Assert.AreEqual("007", arguments.FormattedNumber);
        }

        [TestMethod]
        [DataRow("label -beta", "-beta")]
        [DataRow("label ", "")]
        public void ParseArguments_Label(string args, string expectedLabel)
        {
            var arguments = Parse(args);

            Assert.AreEqual(CommandType.Label, arguments.CmdType);
            Assert.AreEqual(expectedLabel, arguments.Text);
        }

        [TestMethod]
        public void ParseArguments_Write()
        {
            var arguments = Parse("write 1.0.0");

            Assert.AreEqual(CommandType.Write, arguments.CmdType);
            Assert.AreEqual("1.0.0", arguments.Text);
        }

        [TestMethod]
        public void ParseArguments_Ensure()
        {
            var arguments = Parse("ensure");

            Assert.AreEqual(CommandType.Ensure, arguments.CmdType);
        }

        [TestMethod]
        public void ParseArguments_DefaultOptions()
        {
            var arguments = Parse("list");

            Assert.AreEqual(string.Empty, arguments.Arguments.Profile);
            Assert.AreEqual(new FileInfo(BumpyConfigEntry.DefaultConfigFile).FullName, arguments.Arguments.ConfigFile.FullName);
            Assert.AreEqual(new DirectoryInfo(".").FullName, arguments.Arguments.WorkingDirectory.FullName);
        }

        [TestMethod]
        public void ParseArguments_CustomOptions()
        {
            var arguments = Parse("list -p bar -c foo.config -d foodir");

            Assert.AreEqual("bar", arguments.Arguments.Profile);
            Assert.AreEqual(new FileInfo("foo.config").FullName, arguments.Arguments.ConfigFile.FullName);
            Assert.AreEqual(new DirectoryInfo("foodir").FullName, arguments.Arguments.WorkingDirectory.FullName);
        }

        [TestMethod]
        public void ParseArguments_Errors()
        {
            Assert.ThrowsException<ParserException>(() => Parse("randomcommand"));
            Assert.ThrowsException<ParserException>(() => Parse("increment"));
            Assert.ThrowsException<ParserException>(() => Parse("increment notanumber"));
            Assert.ThrowsException<ParserException>(() => Parse("incrementonly"));
            Assert.ThrowsException<ParserException>(() => Parse("incrementonly notanumber"));
            Assert.ThrowsException<ParserException>(() => Parse("assign"));
            Assert.ThrowsException<ParserException>(() => Parse("assign notanumber"));
            Assert.ThrowsException<ParserException>(() => Parse("write"));
            Assert.ThrowsException<ParserException>(() => Parse("write -c -d"));
            Assert.ThrowsException<ParserException>(() => Parse("help -p someprofile"));
            Assert.ThrowsException<ParserException>(() => Parse("new -c foo.config -d foodir"));
        }

        private CommandArguments Parse(string args)
        {
            var parser = new CommandParser();

            return parser.ParseArguments(args.Split(' '));
        }
    }
}
