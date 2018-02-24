using System;

namespace Bumpy
{
    public sealed class CommandRunner
    {
        private readonly IFileUtil _fileUtil;
        private readonly Action<string> _writeLine;
        private readonly CommandArguments _arguments;

        public CommandRunner(IFileUtil fileUtil, Action<string> writeLine, CommandArguments arguments)
        {
            _fileUtil = fileUtil;
            _writeLine = writeLine;
            _arguments = arguments;
        }

        public void Execute()
        {
            var commands = new Commands(_fileUtil, _arguments.ConfigFile, _arguments.WorkingDirectory, _arguments.NoOperation, _writeLine);

            if (_arguments.CmdType == CommandType.List)
            {
                commands.CommandList(_arguments.Profile);
            }
            else if (_arguments.CmdType == CommandType.New)
            {
                commands.CommandNew();
            }
            else if (_arguments.CmdType == CommandType.Increment)
            {
                commands.CommandIncrement(_arguments.Profile, _arguments.Position);
            }
            else if (_arguments.CmdType == CommandType.IncrementOnly)
            {
                commands.CommandIncrementOnly(_arguments.Profile, _arguments.Position);
            }
            else if (_arguments.CmdType == CommandType.Write)
            {
                commands.CommandWrite(_arguments.Profile, _arguments.Text);
            }
            else if (_arguments.CmdType == CommandType.Assign)
            {
                commands.CommandAssign(_arguments.Profile, _arguments.Position, _arguments.FormattedNumber);
            }
            else if (_arguments.CmdType == CommandType.Label)
            {
                commands.CommandLabel(_arguments.Profile, _arguments.Text);
            }
            else if (_arguments.CmdType == CommandType.Ensure)
            {
                commands.CommandEnsure(_arguments.Profile);
            }
            else if (_arguments.CmdType == CommandType.Help)
            {
                commands.CommandHelp();
            }
            else
            {
                // This Exception is only thrown if we forget to extend
                // the 'Execute' method after introducing new commands.
                throw new ParserException("Could not execute command");
            }
        }
    }
}
