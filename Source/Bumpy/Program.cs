using System;
using System.Diagnostics;
using Bumpy.Command;

namespace Bumpy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                PrintBumpy();
                new CommandsParser().Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine();
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
    }
}
