using GZipTest.Core;

namespace GZipTest.Classes.GZip.Core
{
    public class GZipFactory
    {
        public GZip GetGZiper(GZipController gZipController, GZipModes gZipMode)
        {
            switch (gZipMode)
            {
                case GZipModes.Compress:
                    return new GZipCompressor(gZipController);
                case GZipModes.Decompress:
                    return new GZipDecompressor(gZipController);
                default:
                    return null;
            }
        }
    }
}
