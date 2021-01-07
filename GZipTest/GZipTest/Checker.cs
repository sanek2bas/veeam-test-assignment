using System;
using System.IO;
using GZipTest.Core;

namespace GZipTest
{
    internal static class Checker
    {
        public static bool Check(string sourceFilePath, string destinationFilePath)
        {
            if (!File.Exists(sourceFilePath)) return false;
            if (File.Exists(destinationFilePath)) File.Delete(destinationFilePath);
            return true;
        }

        public static bool TryGetGZipMode(string zipMode, out GZipModes gZipMode)
        {         
            zipMode = zipMode.ToLower();

            gZipMode = GZipModes.Compress;
            if (GZipModes.Compress.ToString().ToLower().Equals(zipMode, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (GZipModes.Decompress.ToString().ToLower().Equals(zipMode, StringComparison.OrdinalIgnoreCase))
            {
                gZipMode = GZipModes.Decompress;
                return true;
            }

            return false;
        }
    }
}
