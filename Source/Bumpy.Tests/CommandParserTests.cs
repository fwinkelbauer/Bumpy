using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests
{
    [TestClass]
    public class CommandParserTests
    {
        [TestMethod]
        public void Parse_Help()
        {
            var runner = Parse("help");

            Assert.AreEqual(CommandType.Help, runner.CmdType);
        }

        [TestMethod]
        public void Parse_New()
        {
            var runner = Parse("new");

            Assert.AreEqual(CommandType.New, runner.CmdType);
        }

        [TestMethod]
        public void Parse_List()
        {
            var runner = Parse("list");

            Assert.AreEqual(CommandType.List, runner.CmdType);
        }

        [TestMethod]
        public void Parse_Increment()
        {
            var runner = Parse("increment 3");

            Assert.AreEqual(CommandType.Increment, runner.CmdType);
            Assert.AreEqual(3, runner.Position);
        }

        [TestMethod]
        public void Parse_IncrementOnly()
        {
            var runner = Parse("incrementonly 3");

            Assert.AreEqual(CommandType.IncrementOnly, runner.CmdType);
            Assert.AreEqual(3, runner.Position);
        }

        [TestMethod]
        public void Parse_Assign()
        {
            var runner = Parse("assign 2 007");

            Assert.AreEqual(CommandType.Assign, runner.CmdType);
            Assert.AreEqual(2, runner.Position);
            Assert.AreEqual("007", runner.FormattedNumber);
        }

        [TestMethod]
        public void Parse_Write()
        {
            var runner = Parse("write 1.0.0");

            Assert.AreEqual(CommandType.Write, runner.CmdType);
            Assert.AreEqual("1.0.0", runner.Version);
        }

        [TestMethod]
        public void Parse_DefaultOptions()
        {
            var runner = Parse("list");

            Assert.AreEqual(string.Empty, runner.Profile);
            Assert.AreEqual(new FileInfo(".bumpyconfig").FullName, runner.ConfigFile.FullName);
            Assert.AreEqual(new DirectoryInfo(".").FullName, runner.WorkingDirectory.FullName);
        }

        [TestMethod]
        public void Parse_CustomOptions()
        {
            var runner = Parse("list -p bar -c foo.config -d foodir");

            Assert.AreEqual("bar", runner.Profile);
            Assert.AreEqual(new FileInfo("foo.config").FullName, runner.ConfigFile.FullName);
            Assert.AreEqual(new DirectoryInfo("foodir").FullName, runner.WorkingDirectory.FullName);
        }

        [TestMethod]
        public void Parse_Errors()
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

        private CommandRunner Parse(string args)
        {
            var parser = new CommandParser(null, null);

            return parser.Parse(args.Split(' '));
        }
    }
}
