using System;
using GZipTest.Logs;

namespace GZipTest
{
    static class Program
    {
        static int Main(string[] args)
        {
            LoggerProvider.Logger().Info("Start application");

            int retValue = 0;

            if (args.Length < 3)
            {
                var errMessage = "Insufficient amount of parameters of the program startup";
                ShowError(errMessage);
                goto End;
            }

            if (!Checker.TryGetGZipMode(args[0], out var gZipMode))
            {
                var errMessage = "Not correctly zip mode in parameters of the program startup";
                ShowError(errMessage);
                goto End;
            }

            if (!Checker.Check(args[1], args[2]))
            {
                var errMessage = "Source file not founded";
                ShowError(errMessage);
                goto End;
            }

            var sourceFilePath = args[1];
            var destinationFilePath = args[2];
            IZipController zipController = new GZipController();

            LoggerProvider.Logger().Info($"Start procces of {args[0]}");
            Console.WriteLine("Start...");

            var success = zipController.Start(sourceFilePath, destinationFilePath, gZipMode);
            if (success)
            {
                retValue = 1;
                var message = $"Operation of {gZipMode} completed successfully";
                LoggerProvider.Logger().Info(message);
                Console.WriteLine(message);
            }
            else
            {
                var message = $"Operation of {gZipMode} failed";
                ShowError(message);
            }

        End:
            Console.WriteLine(retValue);
            Console.ReadKey();
            return retValue;
        }

        private static void ShowError(string message)
        {
            LoggerProvider.Logger().Error(message);
            Console.WriteLine($"{message}. See log file");
        }
    }
}
