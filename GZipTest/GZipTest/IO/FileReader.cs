using GZipTest.DataStructure;
using GZipTest.DataTypes;
using GZipTest.Exceptions;
using GZipTest.Logs;
using System;
using System.IO;

namespace GZipTest.IO
{
    public class FileReader
    {
        private readonly IZipController _zipController;

        public FileReader(IZipController zipController)
        {
            _zipController = zipController;
        }

        public void ReadOriginalFile(string sourceFile, IWriteQueue<ByteBlock> queue, int byteBlockSize)
        {
            ByteBlock reading(BinaryReader binaryReader, long count)
            {
                var remainingFileSize = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
                var bytesRead = remainingFileSize <= byteBlockSize ? (int)remainingFileSize : byteBlockSize;
                var lastBuffer = new byte[bytesRead];
                binaryReader.Read(lastBuffer, 0, bytesRead);
                return new ByteBlock(count, lastBuffer);
            }

            Read(sourceFile, queue, reading);
        }

        public void ReadCompressedFile(string sourceFile, IWriteQueue<ByteBlock> queue)
        {
            ByteBlock reading(BinaryReader binaryReader, long count) 
            {
                byte[] lengthBuffer = new byte[8];
                binaryReader.Read(lengthBuffer, 0, lengthBuffer.Length);
                var blockLength = BitConverter.ToInt32(lengthBuffer, 4);
                byte[] compressedData = new byte[blockLength];
                lengthBuffer.CopyTo(compressedData, 0);

                binaryReader.Read(compressedData, 8, blockLength - 8);
                int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
                return new CompressedByteBlock(count, compressedData, dataSize);
            };

            Read(sourceFile, queue, reading);
        }

        private void Read(string filePath, IWriteQueue<ByteBlock> queue, Func<BinaryReader, long, ByteBlock> reading)
        {
            try
            {
                var count = 0;
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(filePath)))
                {
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        if (_zipController.IsCancelled)
                            throw new AbortedOperationException($"Read file {filePath}");
                        
                        var dataBlock = reading(binaryReader, count);
                        queue.Enqueue(dataBlock);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerProvider.Logger().Error($"Error. Read the file: {ex.Message}");
            }
        }
    }
}
