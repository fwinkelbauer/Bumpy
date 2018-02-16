using System.IO;

namespace Bumpy
{
    public sealed class CommandArguments
    {
        public CommandArguments(CommandType cmdType, int position, string formattedNumber, string text, DirectoryInfo workingDirectory, FileInfo configFile, bool noOperation, string profile)
        {
            CmdType = cmdType;
            Position = position;
            FormattedNumber = formattedNumber;
            Text = text;
            WorkingDirectory = workingDirectory;
            ConfigFile = configFile;
            NoOperation = noOperation;
            Profile = profile;
        }

        public CommandType CmdType { get; }

        public int Position { get; }

        public string FormattedNumber { get; }

        public string Text { get; }

        public DirectoryInfo WorkingDirectory { get; }

        public FileInfo ConfigFile { get; }

        public bool NoOperation { get; }

        public string Profile { get; }
    }
}
