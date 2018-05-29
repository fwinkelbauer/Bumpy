using System;
using Bumpy.Core;

namespace Bumpy
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var command = new Command(new FileUtil(), Console.WriteLine);
                var commandArguments = new CommandParser().ParseArguments(args);
                var runner = new CommandRunner(command, commandArguments);
                runner.Execute();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
                Environment.ExitCode = 1;
            }

#if DEBUG
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
#endif
        }
    }
}
