using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bumpy.Util;

namespace Bumpy.Command
{
    // TODO fw write some tests
    public sealed class CommandsParser
    {
        private CommandType _commandType;

        private int _position;
        private int _number;
        private string _version;

        private DirectoryInfo _workingDirectory;
        private FileInfo _configFile;
        private string _profile;

        public CommandsParser()
        {
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

            var shouldParseOptions = _commandType == CommandType.List
                || _commandType == CommandType.Increment
                || _commandType == CommandType.Write
                || _commandType == CommandType.Assign;

            if (_commandType == CommandType.Increment)
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

            if (args.Any() && !shouldParseOptions)
            {
                throw new ParserException($"Command '{cmd}' does not accept additional parameters. See 'bumpy help'.");
            }

            while (args.Any() && shouldParseOptions)
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
            var fileUtil = new FileUtil();
            var commands = new Commands(_workingDirectory, fileUtil, Console.WriteLine);

            if (_commandType == CommandType.List)
            {
                commands.CommandList(LoadConfiguration(fileUtil));
            }
            else if (_commandType == CommandType.New)
            {
                commands.CommandNew();
            }
            else if (_commandType == CommandType.Increment)
            {
                commands.CommandIncrement(LoadConfiguration(fileUtil), _position);
            }
            else if (_commandType == CommandType.Write)
            {
                commands.CommandWrite(LoadConfiguration(fileUtil), _version);
            }
            else if (_commandType == CommandType.Assign)
            {
                commands.CommandAssign(LoadConfiguration(fileUtil), _position, _number);
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

        private IEnumerable<BumpyConfiguration> LoadConfiguration(IFileUtil fileUtil)
        {
            IEnumerable<BumpyConfiguration> config = fileUtil.ReadConfigLazy(_configFile).ToList();

            if (!_profile.Equals(BumpyConfiguration.DefaultProfile))
            {
                config = config.Where(c => c.Profile == _profile);

                if (!config.Any())
                {
                    throw new ParserException($"Profile '{_profile}' does not exist in '{_configFile}'");
                }
            }

            return config;
        }
    }
}
