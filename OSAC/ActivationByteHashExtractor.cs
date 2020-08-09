using System.IO;
using System.Text;

namespace OSAC
{
    public static class ActivationByteHashExtractor
    {
        public static string GetActivationChecksum(string path)
        {
            using (var fs = System.IO.File.OpenRead(path))
            using (var br = new BinaryReader(fs))
            {
                fs.Position = 0x251 + 56 + 4;
                var checksum = br.ReadBytes(20);
                return checksum.ToHexString();
            }
        }

        public static string ToHexString(this byte[] source)
        {
            return source.ToHexString(0, source.Length);
        }

        public static string ToHexString(this byte[] source, int offset, int length)
        {
            StringBuilder stringBuilder = new StringBuilder(length * 2);
            for (int i = offset; i < offset + length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", source[i]);
            }

            return stringBuilder.ToString();
        }
    }
}