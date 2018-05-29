using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Bumpy.Core;
using Bumpy.Core.Config;

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
                Help();
            }
            else
            {
                // This Exception is only thrown if we forget to extend
                // the 'Execute' method after introducing new commands.
                throw new NotImplementedException("Could not execute command");
            }
        }

        private void Help()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            var builder = new StringBuilder();
            builder.AppendLine($"Bumpy v{versionInfo.FileVersion}");
            builder.AppendLine("A tool to maintain version information accross multiple files found in the current working directory");
            builder.AppendLine();
            builder.AppendLine("Commands:");
            builder.AppendLine("  help");
            builder.AppendLine("    View all commands and options");
            builder.AppendLine("  list");
            builder.AppendLine("    Lists all versions");
            builder.AppendLine("  new");
            builder.AppendLine($"    Creates a '{BumpyConfigEntry.DefaultConfigFile}' file if it does not exist");
            builder.AppendLine("  increment <one-based index number> (e.g. 'bumpy increment 3')");
            builder.AppendLine("    Increments the specified component of each version");
            builder.AppendLine("  incrementonly <one-based index number> (e.g. 'bumpy incrementonly 3')");
            builder.AppendLine("    Increments the specified component of each version, without updating following components");
            builder.AppendLine("  write <version string>");
            builder.AppendLine("    Overwrites a version with another version (e.g. 'bumpy write 1.0.0.0')");
            builder.AppendLine("  assign <one-based index number> <version number> (e.g. 'bumpy assign 3 99')");
            builder.AppendLine("    Replaces the specified component of a version with a new number");
            builder.AppendLine("  label <suffix version text>");
            builder.AppendLine("    Replaces the suffix text of a version (e.g. 'bumpy label \"-beta\"')");
            builder.AppendLine("  ensure");
            builder.AppendLine("    Checks that all versions in a profile are equal");
            builder.AppendLine();
            builder.AppendLine("Options: (only available for 'list', 'increment', 'incrementonly', 'write', 'assign', 'label' and 'ensure')");
            builder.AppendLine("  -p <profile name>");
            builder.AppendLine("    Limit a command to a profile");
            builder.AppendLine("  -d <directory>");
            builder.AppendLine("    Run a command in a specific folder (the working directory is used by default)");
            builder.AppendLine("  -c <config file path>");
            builder.AppendLine($"    Alternative name/path of a configuration file (default: '{BumpyConfigEntry.DefaultConfigFile}')");
            builder.AppendLine("  -n");
            builder.AppendLine("    No operation: The specified command (e.g. increment) will not perform file changes");

            Console.WriteLine(builder.ToString());
        }
    }
}
