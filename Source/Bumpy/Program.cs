using System;
using System.Diagnostics;
using Bumpy.Core;

namespace Bumpy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                PrintInfo();
                var command = new Command(new FileUtil(), Console.WriteLine);
                var commandArguments = new CommandParser().ParseArguments(args);
                var runner = new CommandRunner(command, commandArguments);
                runner.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Environment.ExitCode = 1;
            }

#if DEBUG
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
#endif
        }

        private static void PrintInfo()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Bumpy v{versionInfo.FileVersion}");
            Console.ResetColor();
        }
    }
}
