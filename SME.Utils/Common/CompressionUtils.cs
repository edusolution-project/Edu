using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public static class CompressionUtils
    {
        private static int BUFFER_SIZE = 64 * 1024; //64kB

        public static byte[] Compress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData must be non-null");

            using (var compressIntoMs = new MemoryStream())
            {
                using (var gzs = new BufferedStream(new GZipStream(compressIntoMs,
                 CompressionMode.Compress), BUFFER_SIZE))
                {
                    gzs.Write(inputData, 0, inputData.Length);
                }
                return compressIntoMs.ToArray();
            }
        }

        public static byte[] Decompress(byte[] inputData)
        {
            if (inputData == null)
                throw new ArgumentNullException("inputData must be non-null");

            using (var compressedMs = new MemoryStream(inputData))
            {
                using (var decompressedMs = new MemoryStream())
                {
                    using (var gzs = new BufferedStream(new GZipStream(compressedMs,
                     CompressionMode.Decompress), BUFFER_SIZE))
                    {
                        gzs.CopyTo(decompressedMs);
                    }
                    return decompressedMs.ToArray();
                }
            }
        }

        public static string Compress(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            return Convert.ToBase64String(CompressToByte(text));
        }

        public static byte[] CompressToByte(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(text);
            return Compress(buffer);
        }

        public static string DecompressToString(byte[] gzBuffer)
        {
            if (gzBuffer == null)
            {
                return "";
            }
            return Encoding.UTF8.GetString(Decompress(gzBuffer));
        }

        public static string DecompressToString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            byte[] gzBuffer = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(Decompress(gzBuffer));
        }

    }
}
