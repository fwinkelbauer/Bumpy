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
            var commands = new Commands(_fileUtil, _writeLine);

            if (_arguments.CmdType == CommandType.List)
            {
                commands.List(_arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.New)
            {
                commands.New();
            }
            else if (_arguments.CmdType == CommandType.Increment)
            {
                commands.Increment(_arguments.Position, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.IncrementOnly)
            {
                commands.IncrementOnly(_arguments.Position, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Write)
            {
                commands.Write(_arguments.Text, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Assign)
            {
                commands.Assign(_arguments.Position, _arguments.FormattedNumber, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Label)
            {
                commands.Label(_arguments.Text, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Ensure)
            {
                commands.Ensure(_arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Help)
            {
                commands.Help();
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
