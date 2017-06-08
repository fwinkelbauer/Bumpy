using System;
using System.IO;
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
                var config = fileUtil.ReadConfigLazy(directory);
                commands = new Commands(config, directory, fileUtil, (s) => Console.WriteLine(s));
                Execute(commands, args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine();
                commands?.CommandPrintHelp();
                Environment.ExitCode = 1;
            }

            // This is only used for convenience when working in Visual Studio
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }
        }

        private static void Execute(Commands commands, string[] args)
        {
            int position = -1;
            int number = -1;

            if (args.Length == 1 && args[0].Equals("-l"))
            {
                commands.CommandList();
            }
            else if (args.Length == 1 && args[0].Equals("-c"))
            {
                commands.CommandCreateConfig();
            }
            else if (args.Length == 2 && args[0].Equals("-i") && int.TryParse(args[1], out position))
            {
                commands.CommandIncrement(position);
            }
            else if (args.Length == 2 && args[0].Equals("-w"))
            {
                commands.CommandWrite(args[1]);
            }
            else if (args.Length == 3 && args[0].Equals("-a") && int.TryParse(args[1], out position) && int.TryParse(args[2], out number))
            {
                commands.CommandAssign(position, number);
            }
            else
            {
                commands.CommandPrintHelp();
            }
        }
    }
}
