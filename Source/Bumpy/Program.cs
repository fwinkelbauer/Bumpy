using System;
using System.Collections.Generic;
using System.IO;
using Bumpy.Util;

namespace Bumpy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileUtil = new FileUtil();
            var commands = new Commands(fileUtil, (s) => Console.WriteLine(s));
            var directory = new DirectoryInfo(@".");

            try
            {
                Execute(commands, directory, args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine();
                commands.PrintHelp();
                Environment.ExitCode = 1;
            }

            // This is only used for convenience when working in Visual Studio
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        private static void Execute(Commands commands, DirectoryInfo directory, string[] args)
        {
            int position = -1;
            int number = -1;

            if (args.Length == 1 && args[0].Equals("-l"))
            {
                var config = commands.ReadConfigFile(directory);
                ForEachConfig(config, (glob, regex) => { commands.List(directory, glob, regex); });
            }
            else if (args.Length == 1 && args[0].Equals("-c"))
            {
                commands.CreateConfigFile(directory);
            }
            else if (args.Length == 2 && args[0].Equals("-i") && int.TryParse(args[1], out position))
            {
                var config = commands.ReadConfigFile(directory);
                ForEachConfig(config, (glob, regex) => { commands.Increment(directory, glob, regex, position); });
            }
            else if (args.Length == 2 && args[0].Equals("-w"))
            {
                var config = commands.ReadConfigFile(directory);
                ForEachConfig(config, (glob, regex) => { commands.Write(directory, glob, regex, args[1]); });
            }
            else if (args.Length == 3 && args[0].Equals("-a") && int.TryParse(args[1], out position) && int.TryParse(args[2], out number))
            {
                var config = commands.ReadConfigFile(directory);
                ForEachConfig(config, (glob, regex) => { commands.Assign(directory, glob, regex, position, number); });
            }
            else
            {
                commands.PrintHelp();
            }
        }

        private static void ForEachConfig(IEnumerable<BumpyConfiguration> config, Action<string, string> action)
        {
            foreach (var entry in config)
            {
                action(entry.GlobPattern, entry.RegularExpression);
            }
        }
    }
}
