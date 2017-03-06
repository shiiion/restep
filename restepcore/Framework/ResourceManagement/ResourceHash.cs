using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace restep.Framework.ResourceManagement
{
    public delegate byte[] HashGenerator<T>(T dataIn);

    public struct ResourceHash
    {
        internal class RHComparer : IEqualityComparer<ResourceHash>
        {
            public bool Equals(ResourceHash x, ResourceHash y)
            {
                return x.HashEquals(y);
            }

            public int GetHashCode(ResourceHash obj)
            {
                //we want to use Equals function
                return 0;
            }
        }

        private byte[] hashCode;
        
        public ResourceHash(byte[] hash)
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
                    bmp.Save(memStream, ImageFormat.Bmp);
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
