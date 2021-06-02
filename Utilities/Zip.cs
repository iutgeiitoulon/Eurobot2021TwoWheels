using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Utilities
{
    public static class Zip
    {
        public static byte[] ZipText(string text)
        {
            if (text == null)
                return null;

            using (Stream memOutput = new MemoryStream())
            {
                using (GZipOutputStream zipOut = new GZipOutputStream(memOutput))
                {
                    using (StreamWriter writer = new StreamWriter(zipOut))
                    {
                        writer.Write(text);

                        writer.Flush();
                        zipOut.Finish();

                        byte[] bytes = new byte[memOutput.Length];
                        memOutput.Seek(0, SeekOrigin.Begin);
                        memOutput.Read(bytes, 0, bytes.Length);

                        return bytes;
                    }
                }
            }
        }

        private static string unzipText(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (Stream memInput = new MemoryStream(bytes))
            using (GZipInputStream zipInput = new GZipInputStream(memInput))
            using (StreamReader reader = new StreamReader(zipInput))
            {
                string text = reader.ReadToEnd();

                return text;
            }
        }


        public static byte[] CompressText(string input)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(input);
            byte[] compressed = CompressBytes(encoded);
            return compressed;
            //return Convert.ToBase64String(compressed);
        }
        public static byte[] CompressBytes(byte[] input)
        {
            using (var result = new MemoryStream())
            {
                var lengthBytes = BitConverter.GetBytes(input.Length);
                result.Write(lengthBytes, 0, 4);

                using (var compressionStream = new GZipStream(result,
                    CompressionMode.Compress))
                {
                    compressionStream.Write(input, 0, input.Length);
                    compressionStream.Flush();

                }
                return result.ToArray();
            }
        }

        public static string DecompressToText(byte[] input)
        {
            //byte[] compressed = Convert.FromBase64String(input);
            byte[] decompressed = DecompressToBytes(input);
            return Encoding.UTF8.GetString(decompressed);
        }

        public static byte[] DecompressToBytes(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }


        public static byte[] ZipBytes(byte[] data)
        {
            if (data == null)
                return null;

            using (Stream memOutput = new MemoryStream())
            {
                using (GZipOutputStream zipOut = new GZipOutputStream(memOutput))
                {
                    using (StreamWriter writer = new StreamWriter(zipOut))
                    {
                        writer.Write(data);
                        writer.Flush();
                        zipOut.Finish();
                        
                        byte[] bytes = new byte[memOutput.Length];
                        memOutput.Seek(0, SeekOrigin.Begin);
                        memOutput.Read(bytes, 0, bytes.Length);


                        return bytes;
                    }
                }
            }
        }

        public static string UnzipText(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (Stream memInput = new MemoryStream(bytes))
            using (GZipInputStream zipInput = new GZipInputStream(memInput))
            using (StreamReader reader = new StreamReader(zipInput))
            {
                string text = reader.ReadToEnd();

                return text;
            }
        }


        public static byte[] UnzipBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (Stream memInput = new MemoryStream(bytes))
            using (GZipInputStream zipInput = new GZipInputStream(memInput))
            using (StreamReader reader = new StreamReader(zipInput))
            {
                using (var memstream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memstream);
                    bytes = memstream.ToArray();
                    return bytes;
                }                
            }
        }
    }
}
