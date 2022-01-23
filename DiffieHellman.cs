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
    internal class DhParameters
    {
        public Mpi p; ///<Prime modulus

        public Mpi g; ///<Generator

        public DhParameters()
        {
            p = new Mpi();
            g = new Mpi();
        }
    }

    internal class DhContext
    {
        public DhParameters dhParams; //Diffie-Hellman parameters

        public Mpi xa;              ///<One's own private value

        public Mpi ya;              ///<One's own public value

        public Mpi yb;              ///<Peer's public value

        public DhContext()
        {
            dhParams = new DhParameters();

            xa = new Mpi();
            ya = new Mpi();
            yb = new Mpi();
        }
    }

    internal class SshDhGroup
    {
        public string name;   ///<Group name

        public byte[] p;

        public int pLen;

        public byte g;

        public SshDhGroup(string chosenContextName)
        {
            if (chosenContextName == "diffie-hellman-group16")
            {
                LoadDiffieHellmanGroup16();
                return;
            }
            else if (chosenContextName == "diffie-hellman-group14")
            {
                LoadDiffieHellmanGroup14();
                return;
            }

            LoadDiffieHellmanGroup1();
        }

        private void LoadDiffieHellmanGroup16()
        {
            name = "diffie-hellman-group16";

            p = new byte[512] {
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC9, 0x0F, 0xDA, 0xA2, 0x21, 0x68, 0xC2, 0x34,
                    0xC4, 0xC6, 0x62, 0x8B, 0x80, 0xDC, 0x1C, 0xD1, 0x29, 0x02, 0x4E, 0x08, 0x8A, 0x67, 0xCC, 0x74,
                    0x02, 0x0B, 0xBE, 0xA6, 0x3B, 0x13, 0x9B, 0x22, 0x51, 0x4A, 0x08, 0x79, 0x8E, 0x34, 0x04, 0xDD,
                    0xEF, 0x95, 0x19, 0xB3, 0xCD, 0x3A, 0x43, 0x1B, 0x30, 0x2B, 0x0A, 0x6D, 0xF2, 0x5F, 0x14, 0x37,
                    0x4F, 0xE1, 0x35, 0x6D, 0x6D, 0x51, 0xC2, 0x45, 0xE4, 0x85, 0xB5, 0x76, 0x62, 0x5E, 0x7E, 0xC6,
                    0xF4, 0x4C, 0x42, 0xE9, 0xA6, 0x37, 0xED, 0x6B, 0x0B, 0xFF, 0x5C, 0xB6, 0xF4, 0x06, 0xB7, 0xED,
                    0xEE, 0x38, 0x6B, 0xFB, 0x5A, 0x89, 0x9F, 0xA5, 0xAE, 0x9F, 0x24, 0x11, 0x7C, 0x4B, 0x1F, 0xE6,
                    0x49, 0x28, 0x66, 0x51, 0xEC, 0xE4, 0x5B, 0x3D, 0xC2, 0x00, 0x7C, 0xB8, 0xA1, 0x63, 0xBF, 0x05,
                    0x98, 0xDA, 0x48, 0x36, 0x1C, 0x55, 0xD3, 0x9A, 0x69, 0x16, 0x3F, 0xA8, 0xFD, 0x24, 0xCF, 0x5F,
                    0x83, 0x65, 0x5D, 0x23, 0xDC, 0xA3, 0xAD, 0x96, 0x1C, 0x62, 0xF3, 0x56, 0x20, 0x85, 0x52, 0xBB,
                    0x9E, 0xD5, 0x29, 0x07, 0x70, 0x96, 0x96, 0x6D, 0x67, 0x0C, 0x35, 0x4E, 0x4A, 0xBC, 0x98, 0x04,
                    0xF1, 0x74, 0x6C, 0x08, 0xCA, 0x18, 0x21, 0x7C, 0x32, 0x90, 0x5E, 0x46, 0x2E, 0x36, 0xCE, 0x3B,
                    0xE3, 0x9E, 0x77, 0x2C, 0x18, 0x0E, 0x86, 0x03, 0x9B, 0x27, 0x83, 0xA2, 0xEC, 0x07, 0xA2, 0x8F,
                    0xB5, 0xC5, 0x5D, 0xF0, 0x6F, 0x4C, 0x52, 0xC9, 0xDE, 0x2B, 0xCB, 0xF6, 0x95, 0x58, 0x17, 0x18,
                    0x39, 0x95, 0x49, 0x7C, 0xEA, 0x95, 0x6A, 0xE5, 0x15, 0xD2, 0x26, 0x18, 0x98, 0xFA, 0x05, 0x10,
                    0x15, 0x72, 0x8E, 0x5A, 0x8A, 0xAA, 0xC4, 0x2D, 0xAD, 0x33, 0x17, 0x0D, 0x04, 0x50, 0x7A, 0x33,
                    0xA8, 0x55, 0x21, 0xAB, 0xDF, 0x1C, 0xBA, 0x64, 0xEC, 0xFB, 0x85, 0x04, 0x58, 0xDB, 0xEF, 0x0A,
                    0x8A, 0xEA, 0x71, 0x57, 0x5D, 0x06, 0x0C, 0x7D, 0xB3, 0x97, 0x0F, 0x85, 0xA6, 0xE1, 0xE4, 0xC7,
                    0xAB, 0xF5, 0xAE, 0x8C, 0xDB, 0x09, 0x33, 0xD7, 0x1E, 0x8C, 0x94, 0xE0, 0x4A, 0x25, 0x61, 0x9D,
                    0xCE, 0xE3, 0xD2, 0x26, 0x1A, 0xD2, 0xEE, 0x6B, 0xF1, 0x2F, 0xFA, 0x06, 0xD9, 0x8A, 0x08, 0x64,
                    0xD8, 0x76, 0x02, 0x73, 0x3E, 0xC8, 0x6A, 0x64, 0x52, 0x1F, 0x2B, 0x18, 0x17, 0x7B, 0x20, 0x0C,
                    0xBB, 0xE1, 0x17, 0x57, 0x7A, 0x61, 0x5D, 0x6C, 0x77, 0x09, 0x88, 0xC0, 0xBA, 0xD9, 0x46, 0xE2,
                    0x08, 0xE2, 0x4F, 0xA0, 0x74, 0xE5, 0xAB, 0x31, 0x43, 0xDB, 0x5B, 0xFC, 0xE0, 0xFD, 0x10, 0x8E,
                    0x4B, 0x82, 0xD1, 0x20, 0xA9, 0x21, 0x08, 0x01, 0x1A, 0x72, 0x3C, 0x12, 0xA7, 0x87, 0xE6, 0xD7,
                    0x88, 0x71, 0x9A, 0x10, 0xBD, 0xBA, 0x5B, 0x26, 0x99, 0xC3, 0x27, 0x18, 0x6A, 0xF4, 0xE2, 0x3C,
                    0x1A, 0x94, 0x68, 0x34, 0xB6, 0x15, 0x0B, 0xDA, 0x25, 0x83, 0xE9, 0xCA, 0x2A, 0xD4, 0x4C, 0xE8,
                    0xDB, 0xBB, 0xC2, 0xDB, 0x04, 0xDE, 0x8E, 0xF9, 0x2E, 0x8E, 0xFC, 0x14, 0x1F, 0xBE, 0xCA, 0xA6,
                    0x28, 0x7C, 0x59, 0x47, 0x4E, 0x6B, 0xC0, 0x5D, 0x99, 0xB2, 0x96, 0x4F, 0xA0, 0x90, 0xC3, 0xA2,
                    0x23, 0x3B, 0xA1, 0x86, 0x51, 0x5B, 0xE7, 0xED, 0x1F, 0x61, 0x29, 0x70, 0xCE, 0xE2, 0xD7, 0xAF,
                    0xB8, 0x1B, 0xDD, 0x76, 0x21, 0x70, 0x48, 0x1C, 0xD0, 0x06, 0x91, 0x27, 0xD5, 0xB0, 0x5A, 0xA9,
                    0x93, 0xB4, 0xEA, 0x98, 0x8D, 0x8F, 0xDD, 0xC1, 0x86, 0xFF, 0xB7, 0xDC, 0x90, 0xA6, 0xC0, 0x8F,
                    0x4D, 0xF4, 0x35, 0xC9, 0x34, 0x06, 0x31, 0x99, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                };

            pLen = 512;
            g = 2;
        }

        private void LoadDiffieHellmanGroup14()
        {
            name = "diffie-hellman-group14";

            p = new byte[256] {
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC9, 0x0F, 0xDA, 0xA2, 0x21, 0x68, 0xC2, 0x34,
                    0xC4, 0xC6, 0x62, 0x8B, 0x80, 0xDC, 0x1C, 0xD1, 0x29, 0x02, 0x4E, 0x08, 0x8A, 0x67, 0xCC, 0x74,
                    0x02, 0x0B, 0xBE, 0xA6, 0x3B, 0x13, 0x9B, 0x22, 0x51, 0x4A, 0x08, 0x79, 0x8E, 0x34, 0x04, 0xDD,
                    0xEF, 0x95, 0x19, 0xB3, 0xCD, 0x3A, 0x43, 0x1B, 0x30, 0x2B, 0x0A, 0x6D, 0xF2, 0x5F, 0x14, 0x37,
                    0x4F, 0xE1, 0x35, 0x6D, 0x6D, 0x51, 0xC2, 0x45, 0xE4, 0x85, 0xB5, 0x76, 0x62, 0x5E, 0x7E, 0xC6,
                    0xF4, 0x4C, 0x42, 0xE9, 0xA6, 0x37, 0xED, 0x6B, 0x0B, 0xFF, 0x5C, 0xB6, 0xF4, 0x06, 0xB7, 0xED,
                    0xEE, 0x38, 0x6B, 0xFB, 0x5A, 0x89, 0x9F, 0xA5, 0xAE, 0x9F, 0x24, 0x11, 0x7C, 0x4B, 0x1F, 0xE6,
                    0x49, 0x28, 0x66, 0x51, 0xEC, 0xE4, 0x5B, 0x3D, 0xC2, 0x00, 0x7C, 0xB8, 0xA1, 0x63, 0xBF, 0x05,
                    0x98, 0xDA, 0x48, 0x36, 0x1C, 0x55, 0xD3, 0x9A, 0x69, 0x16, 0x3F, 0xA8, 0xFD, 0x24, 0xCF, 0x5F,
                    0x83, 0x65, 0x5D, 0x23, 0xDC, 0xA3, 0xAD, 0x96, 0x1C, 0x62, 0xF3, 0x56, 0x20, 0x85, 0x52, 0xBB,
                    0x9E, 0xD5, 0x29, 0x07, 0x70, 0x96, 0x96, 0x6D, 0x67, 0x0C, 0x35, 0x4E, 0x4A, 0xBC, 0x98, 0x04,
                    0xF1, 0x74, 0x6C, 0x08, 0xCA, 0x18, 0x21, 0x7C, 0x32, 0x90, 0x5E, 0x46, 0x2E, 0x36, 0xCE, 0x3B,
                    0xE3, 0x9E, 0x77, 0x2C, 0x18, 0x0E, 0x86, 0x03, 0x9B, 0x27, 0x83, 0xA2, 0xEC, 0x07, 0xA2, 0x8F,
                    0xB5, 0xC5, 0x5D, 0xF0, 0x6F, 0x4C, 0x52, 0xC9, 0xDE, 0x2B, 0xCB, 0xF6, 0x95, 0x58, 0x17, 0x18,
                    0x39, 0x95, 0x49, 0x7C, 0xEA, 0x95, 0x6A, 0xE5, 0x15, 0xD2, 0x26, 0x18, 0x98, 0xFA, 0x05, 0x10,
                    0x15, 0x72, 0x8E, 0x5A, 0x8A, 0xAC, 0xAA, 0x68, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                };

            pLen = 256;
            g = 2;
        }

        private void LoadDiffieHellmanGroup1()
        {
            name = "diffie-hellman-group1";

            p = new byte[128] {
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC9, 0x0F, 0xDA, 0xA2, 0x21, 0x68, 0xC2, 0x34,
                    0xC4, 0xC6, 0x62, 0x8B, 0x80, 0xDC, 0x1C, 0xD1, 0x29, 0x02, 0x4E, 0x08, 0x8A, 0x67, 0xCC, 0x74,
                    0x02, 0x0B, 0xBE, 0xA6, 0x3B, 0x13, 0x9B, 0x22, 0x51, 0x4A, 0x08, 0x79, 0x8E, 0x34, 0x04, 0xDD,
                    0xEF, 0x95, 0x19, 0xB3, 0xCD, 0x3A, 0x43, 0x1B, 0x30, 0x2B, 0x0A, 0x6D, 0xF2, 0x5F, 0x14, 0x37,
                    0x4F, 0xE1, 0x35, 0x6D, 0x6D, 0x51, 0xC2, 0x45, 0xE4, 0x85, 0xB5, 0x76, 0x62, 0x5E, 0x7E, 0xC6,
                    0xF4, 0x4C, 0x42, 0xE9, 0xA6, 0x37, 0xED, 0x6B, 0x0B, 0xFF, 0x5C, 0xB6, 0xF4, 0x06, 0xB7, 0xED,
                    0xEE, 0x38, 0x6B, 0xFB, 0x5A, 0x89, 0x9F, 0xA5, 0xAE, 0x9F, 0x24, 0x11, 0x7C, 0x4B, 0x1F, 0xE6,
                    0x49, 0x28, 0x66, 0x51, 0xEC, 0xE6, 0x53, 0x81, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                };

            pLen = 128;
            g = 2;
        }
    }

    internal class DiffieHellman
    {
        private DhContext context;

        public DiffieHellman(string diffieHellmanGroupName)
        {
            CryptoError error;
            SshDhGroup group = new SshDhGroup(diffieHellmanGroupName);
            
            context = new DhContext();

            error = context.dhParams.p.Import(group.p, 0, group.pLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                throw new Exception("Diffie Hellman creation p variable import error");

            context.dhParams.g.SetValue(group.g);
            if (error != CryptoError.NO_ERROR)
                throw new Exception("Diffie Hellman creation g variable set error");
        }

        public bool ExportPandG(ref byte[] p,ref byte[] g)
        {
            CryptoError error;

            p = new byte[context.dhParams.p.GetByteLength()];
            error = context.dhParams.p.Export(ref p, 0, p.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return false;

            g = new byte[context.dhParams.g.GetByteLength()];
            error = context.dhParams.g.Export(ref g, 0, g.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return false;

            return true;
        }

        public bool StartDiffieHellman(ref byte[] gAModP)
        {
            CryptoError error;

            error = GenerateKeyPair();
            if (error != CryptoError.NO_ERROR)
                return false;

            gAModP = new byte[context.ya.GetByteLength()];
            error = context.ya.Export(ref gAModP, 0, gAModP.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return false;

            return true;
        }

        public bool GenerateDiffieHellmanSessionKey(byte[] gBModP, ref byte[] sessionKey)
        {
            CryptoError error;

            sessionKey = null;

            if (gBModP == null)
                return false;

            error = context.yb.Import(gBModP, 0, gBModP.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = CheckPublicKey(context.yb);
            if (error != CryptoError.NO_ERROR)
                return false;

            byte[] sharedSecret = new byte[512];

            Console.WriteLine("GenerateDiffieHellmanSessionKey YB:");
            Console.WriteLine(context.yb.Dump());

            error = ComputeSharedSecret(ref sharedSecret, 0, sharedSecret.Length, out int outputLength);
            if (error != CryptoError.NO_ERROR)
                return false;

            if (outputLength < 32)
                return false;

            sessionKey = sharedSecret.SubArray(0, 32);
            return true;
        }

        private CryptoError GenerateKeyPair()
        {
            CryptoError error;
            int k;

            //Get the length in bits of the prime p
            k = context.dhParams.p.GetBitLength();
            //Ensure the length is valid
            if (k == 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            //The private value shall be randomly generated
            error = context.xa.Random(k);
            if (error != CryptoError.NO_ERROR)
                return error;

            //The private value shall be less than p
            if (Mpi.Compare(context.xa, context.dhParams.p) >= 0)
            {
                //Shift value to the right
                error = context.xa.ShiftRight(1);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            //Calculate the corresponding public value (ya = g ^ xa mod p)
            error = context.ya.ExpMod(context.dhParams.g, context.xa, context.dhParams.p);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Check public value
            error = CheckPublicKey(context.ya);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Public value successfully generated
            return CryptoError.NO_ERROR;
        }

        private CryptoError CheckPublicKey(Mpi publicKey)
        {
            CryptoError error;
            Mpi a = new Mpi();

            //Precompute p - 1
            error = a.SubtractInt(context.dhParams.p, 1);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Reject weak public values 1 and p - 1
            if (Mpi.CompareInt(publicKey, 1) <= 0)
                return CryptoError.ERROR_ILLEGAL_PARAMETER;

            if (Mpi.Compare(publicKey, a) >= 0)
                return CryptoError.ERROR_ILLEGAL_PARAMETER;

            return CryptoError.NO_ERROR;
        }

        private CryptoError ComputeSharedSecret(ref byte[] output, int offset, int length, out int outputLength)
        {
            CryptoError error;
            int k;
            Mpi z = new Mpi();

            outputLength = 0;
            if (output.Length < offset + length)
                return CryptoError.ERROR_OUT_BUFFER_INSUFFICIENT;

            //Get the length in octets of the prime modulus
            k = context.dhParams.p.GetByteLength();

            //Make sure that the output buffer is large enough
            if (length < k)
                return CryptoError.ERROR_OUT_BUFFER_INSUFFICIENT;


            Console.WriteLine("Z:");
            Console.WriteLine(z.Dump());
            Console.WriteLine("Yb:");
            Console.WriteLine(context.yb.Dump());
            Console.WriteLine("Xa:");
            Console.WriteLine(context.xa.Dump());
            Console.WriteLine("P:");
            Console.WriteLine(context.dhParams.p.Dump());

            //Calculate the shared secret key (k = yb ^ xa mod p)
            error = z.ExpMod(context.yb, context.xa, context.dhParams.p);
            if (error != CryptoError.NO_ERROR)
                return error;

            Console.WriteLine("ComputeSharedSecret Z:");
            Console.WriteLine(z.Dump());

            //Convert the resulting integer to an octet string
            error = z.Export(ref output, offset, length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            Console.WriteLine("ComputeSharedSecret Output:");
            Console.WriteLine(output.DumpArray());

            //Length of the resulting shared secret
            outputLength = k;

            //Return status code
            return CryptoError.NO_ERROR;
        }

    }
}