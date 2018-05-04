using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bumpy.Core;

namespace Bumpy
{
    public sealed class CommandParser
    {
        private CommandType _commandType;
        private int _position;
        private string _formattedNumber;
        private string _text;
        private BumpyArguments _arguments;

        public CommandParser()
        {
            _commandType = CommandType.Help;
            _position = -1;
            _formattedNumber = "-1";
            _text = string.Empty;
            _arguments = new BumpyArguments();
        }

        public CommandArguments ParseArguments(string[] args)
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

            return new CommandArguments(_commandType, _position, _formattedNumber, _text, _arguments);
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
            else if (_commandType == CommandType.Write || _commandType == CommandType.Label)
            {
                _text = args.Dequeue();
            }
            else if (_commandType == CommandType.Assign)
            {
                _position = Convert.ToInt32(args.Dequeue());
                _formattedNumber = args.Dequeue();
            }

            var shouldParseOptions = _commandType == CommandType.List
                || _commandType == CommandType.Increment
                || _commandType == CommandType.IncrementOnly
                || _commandType == CommandType.Write
                || _commandType == CommandType.Assign
                || _commandType == CommandType.Label
                || _commandType == CommandType.Ensure;

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
                _arguments.Profile = args.Dequeue();
            }
            else if (option.Equals("-d"))
            {
                _arguments.WorkingDirectory = new DirectoryInfo(args.Dequeue());
            }
            else if (option.Equals("-c"))
            {
                _arguments.ConfigFile = new FileInfo(args.Dequeue());
            }
            else if (option.Equals("-n"))
            {
                _arguments.NoOperation = true;
            }
            else
            {
                throw new ParserException($"Option '{option}' not recognized. See 'bumpy help'.");
            }
        }
    }
}
