
namespace GZipTest.DataTypes
{
    public class ByteBlock
    {
        public ByteBlock(long serialNumber, byte[] bytes)
        {
            SerialNumber = serialNumber;
            Bytes = bytes;
        }

        public long SerialNumber { get; }

        public byte[] Bytes { get; }
    }
}
