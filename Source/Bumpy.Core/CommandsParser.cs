using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bumpy.Core
{
    // TODO fw write some tests
    public sealed class CommandsParser
    {
        private readonly IFileUtil _fileUtil;
        private readonly Action<string> _writeLine;

        private CommandType _commandType;

        private int _position;
        private int _number;
        private string _version;

        private DirectoryInfo _workingDirectory;
        private FileInfo _configFile;
        private string _profile;

        public CommandsParser(IFileUtil fileUtil, Action<string> writeLine)
        {
            _fileUtil = fileUtil;
            _writeLine = writeLine;
            _commandType = CommandType.Help;

            _workingDirectory = new DirectoryInfo(".");
            _configFile = new FileInfo(BumpyConfiguration.ConfigFile);
            _profile = BumpyConfiguration.DefaultProfile;
        }

        private enum CommandType
        {
            Help,
            List,
            New,
            Increment,
            IncrementOnly,
            Write,
            Assign
        }

        public void Execute(string[] args)
        {
            try
            {
                ParseCommand(new Queue<string>(args));
            }
            catch (ParserException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ParserException("Invalid arguments. See 'bumpy help'.", e);
            }

            RunCommand();
        }

        private void ParseCommand(Queue<string> args)
        {
            if (!args.Any())
            {
                return;
            }

            var cmd = args.Dequeue();

            if (!Enum.TryParse(cmd, true, out _commandType))
            {
                throw new ParserException($"Command '{cmd}' not recognized. See 'bumpy help'.");
            }

            if (_commandType == CommandType.Increment || _commandType == CommandType.IncrementOnly)
            {
                _position = Convert.ToInt32(args.Dequeue());
            }
            else if (_commandType == CommandType.Write)
            {
                _version = args.Dequeue();
            }
            else if (_commandType == CommandType.Assign)
            {
                _position = Convert.ToInt32(args.Dequeue());
                _number = Convert.ToInt32(args.Dequeue());
            }

            var shouldParseOptions = _commandType == CommandType.List
                || _commandType == CommandType.Increment
                || _commandType == CommandType.IncrementOnly
                || _commandType == CommandType.Write
                || _commandType == CommandType.Assign;

            if (!shouldParseOptions)
            {
                if (args.Any())
                {
                    throw new ParserException($"Command '{cmd}' does not accept additional arguments. See 'bumpy help'.");
                }

                return;
            }

            while (args.Any())
            {
                ParseOptions(args);
            }
        }

        private void ParseOptions(Queue<string> args)
        {
            string option = args.Dequeue();

            if (option.Equals("-p"))
            {
                _profile = args.Dequeue();
            }
            else if (option.Equals("-d"))
            {
                _workingDirectory = new DirectoryInfo(args.Dequeue());
            }
            else if (option.Equals("-c"))
            {
                _configFile = new FileInfo(args.Dequeue());
            }
            else
            {
                throw new ParserException($"Option '{option}' not recognized. See 'bumpy help'.");
            }
        }

        private void RunCommand()
        {
            var commands = new Commands(_fileUtil, _configFile, _workingDirectory, _writeLine);

            if (_commandType == CommandType.List)
            {
                commands.CommandList(_profile);
            }
            else if (_commandType == CommandType.New)
            {
                commands.CommandNew();
            }
            else if (_commandType == CommandType.Increment)
            {
                commands.CommandIncrement(_profile, _position);
            }
            else if (_commandType == CommandType.IncrementOnly)
            {
                commands.CommandIncrementOnly(_profile, _position);
            }
            else if (_commandType == CommandType.Write)
            {
                commands.CommandWrite(_profile, _version);
            }
            else if (_commandType == CommandType.Assign)
            {
                commands.CommandAssign(_profile, _position, _number);
            }
            else if (_commandType == CommandType.Help)
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
