using System;
using GZipTest.Classes.GZip.Core;
using GZipTest.Core;

namespace GZipTest
{
    public class GZipController : IZipController
    {
        private readonly GZipFactory _gZipFactory;

        public GZipController()
        {
            _gZipFactory = new GZipFactory();
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            IsCancelled = false;
        }

        public bool IsCancelled { get; private set; }

        public bool Start(string sourceFile, string destinationFile, GZipModes gZipMode)
        {
            try
            {
                var gZiper = _gZipFactory.GetGZiper(this, gZipMode);
                return gZiper.Execute(sourceFile, destinationFile);
            }
            catch(Exception ex)
            {
                IsCancelled = true;
                return false;
            }
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            consoleCancelEventArgs.Cancel = true;
            IsCancelled = true;
        }
    }

    public interface IZipController
    {
        bool IsCancelled { get; }

        bool Start(string sourceFile, string destinationFile, GZipModes gZipMode);
    }
}
