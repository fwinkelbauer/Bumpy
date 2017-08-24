using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bumpy.Util;

namespace Bumpy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var fileUtil = new FileUtil();
            var directory = new DirectoryInfo(@".");
            Commands commands = null;

            try
            {
                commands = new Commands(directory, fileUtil, (s) => Console.WriteLine(s));
                var config = fileUtil.ReadConfigLazy(directory);
                PrintBumpy();
                Execute(config, commands, args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine();
                commands?.CommandPrintHelp();
                Environment.ExitCode = 1;
            }

            // This is only used for convenience when working in Visual Studio
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        private static void PrintBumpy()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bumpy v{versionInfo.FileVersion}");
            Console.ResetColor();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "This is currently sufficient")]
        private static void Execute(IEnumerable<BumpyConfiguration> config, Commands commands, string[] args)
        {
            int position = -1;
            int number = -1;

            if (args.Length == 1 && args[0].Equals("-l"))
            {
                commands.CommandList(config);
            }
            else if (args.Length == 2 && args[1].Equals("-l"))
            {
                commands.CommandList(config.Where(c => c.Profile == args[0]));
            }
            else if (args.Length == 1 && args[0].Equals("-c"))
            {
                commands.CommandCreateConfig();
            }
            else if (args.Length == 1 && args[0].Equals("-p"))
            {
                commands.CommandPrintProfiles(config);
            }
            else if (args.Length == 2 && args[0].Equals("-i") && int.TryParse(args[1], out position))
            {
                commands.CommandIncrement(config, position);
            }
            else if (args.Length == 3 && args[1].Equals("-i") && int.TryParse(args[2], out position))
            {
                commands.CommandIncrement(config.Where(c => c.Profile == args[0]), position);
            }
            else if (args.Length == 2 && args[0].Equals("-w"))
            {
                commands.CommandWrite(config, args[1]);
            }
            else if (args.Length == 3 && args[1].Equals("-w"))
            {
                commands.CommandWrite(config.Where(c => c.Profile == args[0]), args[2]);
            }
            else if (args.Length == 3 && args[0].Equals("-a") && int.TryParse(args[1], out position) && int.TryParse(args[2], out number))
            {
                commands.CommandAssign(config, position, number);
            }
            else if (args.Length == 4 && args[1].Equals("-a") && int.TryParse(args[2], out position) && int.TryParse(args[3], out number))
            {
                commands.CommandAssign(config.Where(c => c.Profile == args[0]), position, number);
            }
            else
            {
                commands.CommandPrintHelp();
            }
        }
    }
}
