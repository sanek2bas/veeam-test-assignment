using GZipTest.DataStructure;
using GZipTest.DataTypes;
using GZipTest.Exceptions;
using GZipTest.Logs;
using System;
using System.IO;

namespace GZipTest.IO
{
    public class FileWriter
    {
        private readonly IZipController _zipController;

        public FileWriter(IZipController zipController)
        {
            _zipController = zipController;
        }

        public void WriteCompressedFile(string destinationFile, IReadQueue<ByteBlock> queue)
        {
            void writing (BinaryWriter binaryWriter, ByteBlock block)
            {
                BitConverter.GetBytes(block.Bytes.Length).CopyTo(block.Bytes, 4);
                binaryWriter.Write(block.Bytes, 0, block.Bytes.Length);
            }

            Write(destinationFile, queue, writing);
        }

        public void WriteDecompressedFile(string destinationFile, IReadQueue<ByteBlock> queue)
        {
            void writing(BinaryWriter binaryWriter, ByteBlock block)
            {
                binaryWriter.Write(block.Bytes, 0, block.Bytes.Length);
            }

            Write(destinationFile, queue, writing);
        }

        private void Write(string pathFile, IReadQueue<ByteBlock> queue, Action<BinaryWriter, ByteBlock> writing)
        {
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(pathFile)))
                {
                    while (queue.Count > 0 || !queue.IsEnqueueStoped)
                    {
                        if (_zipController.IsCancelled)
                            throw new AbortedOperationException($"Write file {pathFile}");

                        var block = queue.Dequeue();
                        if (block == null) continue;

                        writing(binaryWriter, block);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerProvider.Logger().Error($"Error. Writing the file: {ex.Message}");
            }
        }
    }
}
