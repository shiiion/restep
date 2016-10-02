using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace restep.Framework.ResourceManagement
{
    internal struct ResourceHash
    {
        private byte[] hashCode;

        private ResourceHash()
        {
            hashCode = null;
        }

        private ResourceHash(byte[] hash)
        {
            hashCode = hash;
        }

        public bool HashEquals(ResourceHash otherHash)
        {
            return System.Linq.Enumerable.SequenceEqual(hashCode, otherHash.hashCode);
        }

        public static ResourceHash CreateHash(Bitmap bmp)
        {
            byte[] hashCode;
            using (MD5 checksum = MD5.Create())
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    bmp.Save(memStream, ImageFormat.MemoryBmp);
                    hashCode = checksum.ComputeHash(memStream);
                }
            }
            return new ResourceHash(hashCode);
        }

        public static ResourceHash CreateHash(string str)
        {
            byte[] hashCode;
            using (MD5 checksum = MD5.Create())
            {
                hashCode = checksum.ComputeHash(Encoding.ASCII.GetBytes(str));
            }
            return new ResourceHash(hashCode);
        }
    }
}
