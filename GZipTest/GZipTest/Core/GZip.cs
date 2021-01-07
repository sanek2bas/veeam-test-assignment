using System;
using System.Threading;
using GZipTest.Logs;
using GZipTest.DataTypes;
using GZipTest.DataStructure;
using GZipTest.IO;

namespace GZipTest.Classes.GZip.Core
{
    public abstract class GZip
    {
        private readonly Thread[] _zipThreads;
        protected const int BLOCK_SIZE = 1024 * 1024;
        protected readonly IZipController GZipController;
        protected readonly SafeThreadQueue<ByteBlock> QueueAfterRead;
        protected readonly SafeThreadQueue<ByteBlock> QueueBeforeWrite;
        protected readonly FileReader Reader;
        protected readonly FileWriter Writer;

        protected GZip(IZipController gZipController)
        {
            GZipController = gZipController;
            _zipThreads = new Thread[Environment.ProcessorCount];
            QueueAfterRead = new SafeThreadQueue<ByteBlock>();
            QueueBeforeWrite = new SafeThreadQueue<ByteBlock>();
            Reader = new FileReader(gZipController);
            Writer = new FileWriter(gZipController);
        }

        public bool Execute(string sourceFile, string destinationFile)
        {
            try
            {
                var reader = new Thread(() => Read(sourceFile));
                reader.Start();

                for (int i = 0; i < _zipThreads.Length; i++)
                {
                    _zipThreads[i] = new Thread(ZipMethod);
                    _zipThreads[i].Start();
                }

                var writer = new Thread(() => Write(destinationFile));
                writer.Start();

                reader.Join();
                QueueAfterRead.StopEnqueue();
                foreach (var thread in _zipThreads) thread.Join();
                QueueBeforeWrite.StopEnqueue();
                writer.Join();

                return !GZipController.IsCancelled;
            }
            catch (Exception ex)
            {
                LoggerProvider.Logger().Error(ex.Message);
                return false;
            }
        }

        protected abstract void Read(string sourceFile);

        protected abstract void ZipMethod();

        protected abstract void Write(string destinationFile);
    }
}
