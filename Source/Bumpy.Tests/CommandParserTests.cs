using System.IO;
using Bumpy.Core.Config;
using Xunit;

namespace Bumpy.Tests
{
    public class CommandParserTests
    {
        [Fact]
        public void ParseArguments_Help()
        {
            var arguments = Parse("help");

            Assert.Equal(CommandType.Help, arguments.CmdType);
        }

        [Fact]
        public void ParseArguments_New()
        {
            var arguments = Parse("new");

            Assert.Equal(CommandType.New, arguments.CmdType);
        }

        [Fact]
        public void ParseArguments_List()
        {
            var arguments = Parse("list");

            Assert.Equal(CommandType.List, arguments.CmdType);
        }

        [Fact]
        public void ParseArguments_Increment()
        {
            var arguments = Parse("increment 3");

            Assert.Equal(CommandType.Increment, arguments.CmdType);
            Assert.Equal(3, arguments.Position);
        }

        [Fact]
        public void ParseArguments_IncrementOnly()
        {
            var arguments = Parse("incrementonly 3");

            Assert.Equal(CommandType.IncrementOnly, arguments.CmdType);
            Assert.Equal(3, arguments.Position);
        }

        [Fact]
        public void ParseArguments_Assign()
        {
            var arguments = Parse("assign 2 007");

            Assert.Equal(CommandType.Assign, arguments.CmdType);
            Assert.Equal(2, arguments.Position);
            Assert.Equal("007", arguments.FormattedNumber);
        }

        [Theory]
        [InlineData("label -beta", "-beta")]
        [InlineData("label ", "")]
        public void ParseArguments_Label(string args, string expectedLabel)
        {
            var arguments = Parse(args);

            Assert.Equal(CommandType.Label, arguments.CmdType);
            Assert.Equal(expectedLabel, arguments.Text);
        }

        [Fact]
        public void ParseArguments_Write()
        {
            var arguments = Parse("write 1.0.0");

            Assert.Equal(CommandType.Write, arguments.CmdType);
            Assert.Equal("1.0.0", arguments.Text);
        }

        [Fact]
        public void ParseArguments_Ensure()
        {
            var arguments = Parse("ensure");

            Assert.Equal(CommandType.Ensure, arguments.CmdType);
        }

        [Fact]
        public void ParseArguments_DefaultOptions()
        {
            var arguments = Parse("list");

            Assert.Equal(string.Empty, arguments.Arguments.Profile);
            Assert.Equal(new FileInfo(BumpyConfigEntry.DefaultConfigFile).FullName, arguments.Arguments.ConfigFile.FullName);
            Assert.Equal(new DirectoryInfo(".").FullName, arguments.Arguments.WorkingDirectory.FullName);
        }

        [Fact]
        public void ParseArguments_CustomOptions()
        {
            var arguments = Parse("list -p bar -c foo.config -d foodir");

            Assert.Equal("bar", arguments.Arguments.Profile);
            Assert.Equal(new FileInfo("foo.config").FullName, arguments.Arguments.ConfigFile.FullName);
            Assert.Equal(new DirectoryInfo("foodir").FullName, arguments.Arguments.WorkingDirectory.FullName);
        }

        [Fact]
        public void ParseArguments_Errors()
        {
            Assert.Throws<ParserException>(() => Parse("randomcommand"));
            Assert.Throws<ParserException>(() => Parse("increment"));
            Assert.Throws<ParserException>(() => Parse("increment notanumber"));
            Assert.Throws<ParserException>(() => Parse("incrementonly"));
            Assert.Throws<ParserException>(() => Parse("incrementonly notanumber"));
            Assert.Throws<ParserException>(() => Parse("assign"));
            Assert.Throws<ParserException>(() => Parse("assign notanumber"));
            Assert.Throws<ParserException>(() => Parse("write"));
            Assert.Throws<ParserException>(() => Parse("write -c -d"));
            Assert.Throws<ParserException>(() => Parse("help -p someprofile"));
            Assert.Throws<ParserException>(() => Parse("new -c foo.config -d foodir"));
        }

        private CommandArguments Parse(string args)
        {
            var parser = new CommandParser();

            return parser.ParseArguments(args.Split(' '));
        }
    }
}
