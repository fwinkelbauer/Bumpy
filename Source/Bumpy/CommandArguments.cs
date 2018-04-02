namespace Bumpy
{
    public sealed class CommandArguments
    {
        public CommandArguments(CommandType cmdType, int position, string formattedNumber, string text, BumpyArguments arguments)
        {
            CmdType = cmdType;
            Position = position;
            FormattedNumber = formattedNumber;
            Text = text;
            Arguments = arguments;
        }

        public CommandType CmdType { get; }

        public int Position { get; }

        public string FormattedNumber { get; }

        public string Text { get; }

        public BumpyArguments Arguments { get; }
    }
}
