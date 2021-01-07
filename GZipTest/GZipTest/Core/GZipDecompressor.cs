using System;
using System.IO;
using System.IO.Compression;
using GZipTest.Exceptions;
using GZipTest.Logs;
using GZipTest.DataTypes;

namespace GZipTest.Classes.GZip.Core
{
    public class GZipDecompressor : GZip
    {
        public GZipDecompressor(IZipController gZipController) : base(gZipController)
        {
        }

        protected override void Read(string sourceFile)
        {
            Reader.ReadCompressedFile(sourceFile, QueueAfterRead);
        }

        protected override void ZipMethod()
        {
            try
            {
                while (QueueAfterRead.Count > 0 || !QueueAfterRead.IsEnqueueStoped)
                {
                    if (GZipController.IsCancelled)
                        throw new AbortedOperationException("Decompress");

                    if (!(QueueAfterRead.Dequeue() is CompressedByteBlock compressedBlock))
                        continue;

                    using (MemoryStream ms = new MemoryStream(compressedBlock.Bytes))
                    {
                        using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            var decompressedData = new byte[compressedBlock.DecompressedByteBlockSize];
                            gzipStream.Read(decompressedData, 0, decompressedData.Length);
                            var decompressedBlock = new ByteBlock(compressedBlock.SerialNumber, decompressedData);
                            QueueBeforeWrite.Enqueue(decompressedBlock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Error in decompress thread. Error description: {ex.Message}";
                LoggerProvider.Logger().Error(message);
            }
        }

        protected override void Write(string destinationFile)
        {
            Writer.WriteDecompressedFile(destinationFile, QueueBeforeWrite);
        }
    }
}
