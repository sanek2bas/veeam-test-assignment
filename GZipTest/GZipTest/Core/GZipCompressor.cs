using System;
using System.IO;
using System.IO.Compression;
using GZipTest.Exceptions;
using GZipTest.Logs;
using GZipTest.DataTypes;

namespace GZipTest.Classes.GZip.Core
{
    public class GZipCompressor : GZip
    {
        public GZipCompressor(GZipController gZipController) : base(gZipController)
        {
        }

        protected override void Read(string sourceFile)
        {
            Reader.ReadOriginalFile(sourceFile, QueueAfterRead, BLOCK_SIZE);
        }

        protected override void ZipMethod()
        {
            try
            {
                while (QueueAfterRead.Count > 0 || !QueueAfterRead.IsEnqueueStoped)
                {
                    if (GZipController.IsCancelled)
                        throw new AbortedOperationException("Compress");

                    var block = QueueAfterRead.Dequeue();
                    if (block == null) continue;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                        {
                            gZipStream.Write(block.Bytes, 0, block.Bytes.Length);
                        }
                        var compressedData = memoryStream.ToArray();
                        var compressedBlock = new ByteBlock(block.SerialNumber, compressedData);
                        QueueBeforeWrite.Enqueue(compressedBlock);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Error in compressor thread. Error description: {ex.Message}";
                LoggerProvider.Logger().Error(message);
            }
        }

        protected override void Write(string destinationFile)
        {
            Writer.WriteCompressedFile(destinationFile, QueueBeforeWrite);
        }
    }
}
