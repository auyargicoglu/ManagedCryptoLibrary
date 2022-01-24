////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                //
//  Copyright eLegislations DT                                                                    //
//                                                                                                //
//  The copyright to the computer program(s) herein is the property of eLegislations DT           //
//  The program(s) may be used and/or copied only with the written permission of eLegislations DT //
//  or in accordance with the terms and conditions stipulated in the agreement/contract under     //
//  which the program(s) have been supplied.                                                      //
//                                                                                                //
////////////////////////////////////////////////////////////////////////////////////////////////////
///
/// Codes are ported from CycloneCRYPTO Open
/// 
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace ManagedCryptoLibrary
{
    internal class Sha512Context
    {
        public UInt64[] h = new UInt64[8];

        public UInt64[] w = new UInt64[16];

        public int size;

        public int totalSize;

        public Sha512Context()
        {
            h[0] = 0x6A09E667F3BCC908;
            h[1] = 0xBB67AE8584CAA73B;
            h[2] = 0x3C6EF372FE94F82B;
            h[3] = 0xA54FF53A5F1D36F1;
            h[4] = 0x510E527FADE682D1;
            h[5] = 0x9B05688C2B3E6C1F;
            h[6] = 0x1F83D9ABFB41BD6B;
            h[7] = 0x5BE0CD19137E2179;

            size = 0;
            totalSize = 0;
        }

        public bool GetDigest(byte[] digest, int offset, out int length)
        {
            length = 0;

            for (int i = 0; i < h.Length; i++)
            {
                byte[] temp = BitConverter.GetBytes(h[i]);
                if (offset + temp.Length > digest.Length)
                    return false;

                digest.CopyIn(offset, temp, 0, temp.Length);
                offset += temp.Length;
                length += temp.Length;
            }

            return true;
        }

        public bool SetBuffer(byte[] buffer, int offset, int length)
        {
            int i = 0;
            int blockSize;

            if (buffer.Length < offset + length)
                return false;

            w.Zeroize();

            while(length > 0)
            {
                blockSize = length > 8 ? 8 : length;

                byte[] subArray = buffer.SubArray(offset, blockSize);
                if (subArray.Length != 8)
                    subArray = subArray.Grow(8);

                w[i] = BitConverter.ToUInt64(subArray);
                offset += blockSize;
                length -= blockSize;
                i++;
            }

            return true;
        }

        public bool SetZeroPaddingStartMark()
        {
            if ((size >= 128) || (size < 0))
                return false;

            int wIndex = size / 8;
            int bIndex = size % 8;

            byte[] wIndexBytes = BitConverter.GetBytes(w[wIndex]);
            wIndexBytes[bIndex] = 0x80;
            w[wIndex] = BitConverter.ToUInt64(wIndexBytes);

            return true;
        }
    }

    public class SHA512
    {
        #region Sha512 constants

        private static readonly byte[] padding = new byte[]
        {
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private static readonly UInt64[] k = new UInt64[80]
        {
            0x428A2F98D728AE22, 0x7137449123EF65CD, 0xB5C0FBCFEC4D3B2F, 0xE9B5DBA58189DBBC,
            0x3956C25BF348B538, 0x59F111F1B605D019, 0x923F82A4AF194F9B, 0xAB1C5ED5DA6D8118,
            0xD807AA98A3030242, 0x12835B0145706FBE, 0x243185BE4EE4B28C, 0x550C7DC3D5FFB4E2,
            0x72BE5D74F27B896F, 0x80DEB1FE3B1696B1, 0x9BDC06A725C71235, 0xC19BF174CF692694,
            0xE49B69C19EF14AD2, 0xEFBE4786384F25E3, 0x0FC19DC68B8CD5B5, 0x240CA1CC77AC9C65,
            0x2DE92C6F592B0275, 0x4A7484AA6EA6E483, 0x5CB0A9DCBD41FBD4, 0x76F988DA831153B5,
            0x983E5152EE66DFAB, 0xA831C66D2DB43210, 0xB00327C898FB213F, 0xBF597FC7BEEF0EE4,
            0xC6E00BF33DA88FC2, 0xD5A79147930AA725, 0x06CA6351E003826F, 0x142929670A0E6E70,
            0x27B70A8546D22FFC, 0x2E1B21385C26C926, 0x4D2C6DFC5AC42AED, 0x53380D139D95B3DF,
            0x650A73548BAF63DE, 0x766A0ABB3C77B2A8, 0x81C2C92E47EDAEE6, 0x92722C851482353B,
            0xA2BFE8A14CF10364, 0xA81A664BBC423001, 0xC24B8B70D0F89791, 0xC76C51A30654BE30,
            0xD192E819D6EF5218, 0xD69906245565A910, 0xF40E35855771202A, 0x106AA07032BBD1B8,
            0x19A4C116B8D2D0C8, 0x1E376C085141AB53, 0x2748774CDF8EEB99, 0x34B0BCB5E19B48A8,
            0x391C0CB3C5C95A63, 0x4ED8AA4AE3418ACB, 0x5B9CCA4F7763E373, 0x682E6FF3D6B2B8A3,
            0x748F82EE5DEFB2FC, 0x78A5636F43172F60, 0x84C87814A1F0AB72, 0x8CC702081A6439EC,
            0x90BEFFFA23631E28, 0xA4506CEBDE82BDE9, 0xBEF9A3F7B2C67915, 0xC67178F2E372532B,
            0xCA273ECEEA26619C, 0xD186B8C721C0C207, 0xEADA7DD6CDE0EB1E, 0xF57D4F7FEE6ED178,
            0x06F067AA72176FBA, 0x0A637DC5A2C898A6, 0x113F9804BEF90DAE, 0x1B710B35131C471B,
            0x28DB77F523047D84, 0x32CAAB7B40C72493, 0x3C9EBE0A15C9BEBC, 0x431D67C49C100D4C,
            0x4CC5D4BECB3E42B6, 0x597F299CFC657E2A, 0x5FCB6FAB3AD6FAEC, 0x6C44198C4A475817
        };

        private static readonly byte[] sha512Oid = new byte[9] { 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x03 };

        #endregion

        #region Sha512 implementation functions

        public static CryptoError Sha512Compute(byte[] data, int offset, int length, ref byte[] digest, int digestOffset, out int digestLength)
        {
            digestLength = 0;

            if (data == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (offset < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (length < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (digest == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (digestOffset < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            int blockSize;
            UInt64 totalSize;
            Sha512Context context = new Sha512Context();

            if (data.Length < offset + length)
                return CryptoError.ERROR_INVALID_INPUT_BUFFER;

            if (digest.Length < digestOffset + 64)
                return CryptoError.ERROR_OUT_BUFFER_INSUFFICIENT;

            //Process the incoming data
            while (length > 0)
            {
                //The buffer can hold at most 128 bytes
                blockSize = length > 128 ? 128 : length;

                //Update the SHA-512 context
                context.SetBuffer(data, offset, blockSize);
                context.size = blockSize;
                context.totalSize += blockSize;

                length -= blockSize;
                offset += blockSize;

                if (blockSize == 128)
                {
                    Sha512ProcessBlock(context);
                    context.size = 0;
                }
            }

            //Length of the original message (before padding)
            totalSize = (UInt64)context.totalSize * 8UL;

            //Pad the message so that its length is congruent to 112 modulo 128
            if (context.size < 112)
            {
                if (context.size == 0)
                    context.w.Zeroize();

                context.SetZeroPaddingStartMark();
                context.w[14] = 0;
                context.w[15] = CryptoCommon.swapInt64(totalSize);

                Sha512ProcessBlock(context);
            }
            else
            {
                context.SetZeroPaddingStartMark();
                Sha512ProcessBlock(context);

                context.w.Zeroize();
                context.w[14] = 0;
                context.w[15] = CryptoCommon.swapInt64(totalSize);

                Sha512ProcessBlock(context);
            }

            for (int i = 0; i < 8; i++)
                context.h[i] = CryptoCommon.swapInt64(context.h[i]);

            context.GetDigest(digest, digestOffset, out digestLength);
            return CryptoError.NO_ERROR;
        }

        public static CryptoError Sha384Compute(byte[] data, int offset, int length, ref byte[] digest, int digestOffset, out int digestLength)
        {
            digestLength = 0;

            if (data == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (offset < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (length < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (digest == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (digestOffset < 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            CryptoError error;
            byte[] longDigest = new byte[64];

            if (data.Length < offset + length)
                return CryptoError.ERROR_INVALID_INPUT_BUFFER;

            if (digest.Length < digestOffset + 48)
                return CryptoError.ERROR_OUT_BUFFER_INSUFFICIENT;

            error = Sha512Compute(data, offset, length, ref longDigest, 0, out _);
            digest.CopyIn(digestOffset, longDigest, 0, 48);
            digestLength = 48;

            return error;
        }

        private static void Sha512ProcessBlock(Sha512Context context)
        {
            int t;
            UInt64 temp1;
            UInt64 temp2;

            //Initialize the 8 working registers
            UInt64 a = context.h[0];
            UInt64 b = context.h[1];
            UInt64 c = context.h[2];
            UInt64 d = context.h[3];
            UInt64 e = context.h[4];
            UInt64 f = context.h[5];
            UInt64 g = context.h[6];
            UInt64 h = context.h[7];

            //Convert from big-endian byte order to host byte order
            for (t = 0; t < 16; t++)
                context.w[t] = CryptoCommon.swapInt64(context.w[t]);

            //SHA-512 hash computation (alternate method)
            for (t = 0; t < 80; t++)
            {
                //Prepare the message schedule
                if (t >= 16)
                {
                    context.w[Circular(t)] += (SIGMA4(context.w[Circular(t + 14)]) + context.w[Circular(t + 9)] + SIGMA3(context.w[Circular(t + 1)]));
                }

                //Calculate T1 and T2
                temp1 = h + SIGMA2(e) + CH(e, f, g) + k[t] + context.w[Circular(t)];
                temp2 = SIGMA1(a) + MAJ(a, b, c);

                //Update the working registers
                h = g;
                g = f;
                f = e;
                e = d + temp1;
                d = c;
                c = b;
                b = a;
                a = temp1 + temp2;
            }

            //Update the hash value
            context.h[0] += a;
            context.h[1] += b;
            context.h[2] += c;
            context.h[3] += d;
            context.h[4] += e;
            context.h[5] += f;
            context.h[6] += g;
            context.h[7] += h;
        }

        #endregion

        #region Simple Math Functions, implemented as defines in C

        private static UInt64 CH(UInt64 x, UInt64 y, UInt64 z)
        {
            return (((x) & (y)) | (~(x) & (z)));
        }

        private static UInt64 MAJ(UInt64 x, UInt64 y, UInt64 z)
        {
            return (((x) & (y)) | ((x) & (z)) | ((y) & (z)));
        }

        private static UInt64 SIGMA1(UInt64 x)
        {
            return (CryptoCommon.ROR64(x, 28) ^ CryptoCommon.ROR64(x, 34) ^ CryptoCommon.ROR64(x, 39));
        }

        private static UInt64 SIGMA2(UInt64 x)
        {
            return (CryptoCommon.ROR64(x, 14) ^ CryptoCommon.ROR64(x, 18) ^ CryptoCommon.ROR64(x, 41));
        }

        private static UInt64 SIGMA3(UInt64 x)
        {
            return (CryptoCommon.ROR64(x, 1) ^ CryptoCommon.ROR64(x, 8) ^ CryptoCommon.SHR64(x, 7));
        }

        private static UInt64 SIGMA4(UInt64 x)
        {
            return (CryptoCommon.ROR64(x, 19) ^ CryptoCommon.ROR64(x, 61) ^ CryptoCommon.SHR64(x, 6));
        }

        private static int Circular(int t)
        {
            return (t & 0x0F);
        }


        #endregion
    }
}
