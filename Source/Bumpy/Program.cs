using System;
using System.Diagnostics;
using Bumpy.Core;

namespace Bumpy
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                PrintInfo();
                new CommandsParser(new FileUtil(), Console.WriteLine).Execute(args);
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

        // TODO change .bumpyconfig to do this
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
