
namespace GZipTest.DataTypes
{
    public class CompressedByteBlock : ByteBlock
    {
        public CompressedByteBlock(long serialNumber, byte[] buffer, int decompressedByteBlockSize) : base(serialNumber, buffer)
        {
            DecompressedByteBlockSize = decompressedByteBlockSize;
        }

        public int DecompressedByteBlockSize { get; }
    }
}
