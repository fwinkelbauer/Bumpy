namespace Bumpy
{
    public sealed class CommandRunner
    {
        private readonly Command _command;
        private readonly CommandArguments _arguments;

        public CommandRunner(Command command, CommandArguments arguments)
        {
            _command = command;
            _arguments = arguments;
        }

        public void Execute()
        {
            if (_arguments.CmdType == CommandType.List)
            {
                _command.List(_arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.New)
            {
                _command.New();
            }
            else if (_arguments.CmdType == CommandType.Increment)
            {
                _command.Increment(_arguments.Position, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.IncrementOnly)
            {
                _command.IncrementOnly(_arguments.Position, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Write)
            {
                _command.Write(_arguments.Text, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Assign)
            {
                _command.Assign(_arguments.Position, _arguments.FormattedNumber, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Label)
            {
                _command.Label(_arguments.Text, _arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Ensure)
            {
                _command.Ensure(_arguments.Arguments);
            }
            else if (_arguments.CmdType == CommandType.Help)
            {
                _command.Help();
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
