using System;
using System.IO;

namespace Bumpy
{
    public sealed class CommandRunner
    {
        private readonly IFileUtil _fileUtil;
        private readonly Action<string> _writeLine;

        public CommandRunner(IFileUtil fileUtil, Action<string> writeLine, CommandType type, int position, string formattedNumber, string version, DirectoryInfo workingDirectory, FileInfo configFile, string profile)
        {
            _fileUtil = fileUtil;
            _writeLine = writeLine;

            Type = type;
            Position = position;
            FormattedNumber = formattedNumber;
            Version = version;
            WorkingDirectory = workingDirectory;
            ConfigFile = configFile;
            Profile = profile;
        }

        public CommandType Type { get; }

        public int Position { get; }

        public string FormattedNumber { get; }

        public string Version { get; }

        public DirectoryInfo WorkingDirectory { get; }

        public FileInfo ConfigFile { get; }

        public string Profile { get; }

        public void Execute()
        {
            var commands = new Commands(_fileUtil, ConfigFile, WorkingDirectory, _writeLine);

            if (Type == CommandType.List)
            {
                commands.CommandList(Profile);
            }
            else if (Type == CommandType.New)
            {
                commands.CommandNew();
            }
            else if (Type == CommandType.Increment)
            {
                commands.CommandIncrement(Profile, Position);
            }
            else if (Type == CommandType.IncrementOnly)
            {
                commands.CommandIncrementOnly(Profile, Position);
            }
            else if (Type == CommandType.Write)
            {
                commands.CommandWrite(Profile, Version);
            }
            else if (Type == CommandType.Assign)
            {
                commands.CommandAssign(Profile, Position, FormattedNumber);
            }
            else if (Type == CommandType.Help)
            {
                commands.CommandHelp();
            }
            else
            {
                // This Exception is only thrown if we forget to extend this method after introducing new commands.
                throw new ParserException("Could not execute command");
            }
        }
    }
}
