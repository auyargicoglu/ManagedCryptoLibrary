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
    public static class Tester
    {
        public static void TestMain0()
        {
            Mpi privateKey = new Mpi();
            EcPoint publicKey = new EcPoint();
            EllipticCurveCore ellipticCurve = new EllipticCurveCore();

            ellipticCurve.InitDomainParameters();
            ellipticCurve.LoadDomainParameters(new Secp384r1EC());

            ellipticCurve.GenerateKeyPair(ref privateKey, ref publicKey);
            Console.WriteLine("PrivateKey:\n" + privateKey.Dump() + "\nPublicKey:\n" + publicKey.Dump());

            byte[] privateKeyStream = new byte[privateKey.GetByteLength()];
            privateKey.Export(ref privateKeyStream, 0, privateKeyStream.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            Console.WriteLine(privateKeyStream.DumpArray());

            ellipticCurve.GetPublicKeySize(out int publicKeySize);
            byte[] publicKeyStream = new byte[publicKeySize];
            ellipticCurve.Export(publicKey, ref publicKeyStream);
            Console.WriteLine(publicKeyStream.DumpArray());

            Mpi readPrivateKey = new Mpi();
            readPrivateKey.Import(privateKeyStream, 0, privateKeyStream.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            Console.WriteLine(readPrivateKey.Dump());

            EcPoint readPublicKey = new EcPoint();
            ellipticCurve.Import(ref readPublicKey, publicKeyStream);
            Console.WriteLine(readPublicKey.Dump());

            byte[] hash512Result = new byte[64];
            byte[] hash384Result = new byte[64];

            SHA512.Sha512Compute(publicKeyStream, 0, publicKeyStream.Length, ref hash512Result, 0, out int _);
            Console.WriteLine(hash512Result.DumpArray());

            SHA512.Sha384Compute(publicKeyStream, 0, publicKeyStream.Length, ref hash384Result, 0, out _);
            Console.WriteLine(hash384Result.DumpArray());

            byte[] someInput = new byte[4096];

            for (int i = 0; i < 4096; i++)
                someInput[i] = (byte)((i * i * i + i * i + i) & 0xFF);

            Console.WriteLine("4096:\n");
            SHA512.Sha512Compute(someInput, 0, 4096, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());

            Console.WriteLine("256:\n");
            SHA512.Sha512Compute(someInput, 0, 256, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());

            Console.WriteLine("240:\n");
            SHA512.Sha512Compute(someInput, 0, 240, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());

            Console.WriteLine("128:\n");
            SHA512.Sha512Compute(someInput, 0, 128, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());

            Console.WriteLine("112:\n");
            SHA512.Sha512Compute(someInput, 0, 112, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());

            Console.WriteLine("1:\n");
            SHA512.Sha512Compute(someInput, 0, 1, ref hash512Result, 0, out _);
            Console.WriteLine(hash512Result.DumpArray());
        }

        public static void TestMain1()
        {
            EllipticCurve ellipticCurve = new EllipticCurve();

            ellipticCurve.GenerateECKeys(out byte[] privateKey, out byte[] publicKey);

            Console.WriteLine("Private Key");
            Console.WriteLine(privateKey.DumpArray());
            Console.WriteLine("Public Key");
            Console.WriteLine(publicKey.DumpArray());

            byte[] signatureInput = new byte[65536];
            byte[] signature = new byte[EllipticCurve.eccConstantSignatureLength];

            for (int i = 0; i < signatureInput.Length; i++)
                signatureInput[i] = (byte)(i * i * i + i * i + 0x54);

            //ellipticCurve.Sign(privateKey, signatureInput, 0, 1024, ref signature);

            //Console.WriteLine("Signature");
            //Console.WriteLine(signature.DumpArray());

            ellipticCurve.Sign(privateKey, signatureInput, 12, 48821, ref signature);

            Console.WriteLine("Signature");
            Console.WriteLine(signature.DumpArray());

            bool verifyRes = ellipticCurve.VerifySignature(publicKey, signatureInput, 12, 48821, signature);
            Console.WriteLine("Verify Result =" + verifyRes.ToString());
        }

        public static void TestMain2()
        {
            CCryptoEngine cryptoEngine0 = new CCryptoEngine("TestEnd0", 0x21001212, null);
            CCryptoEngine cryptoEngine1 = new CCryptoEngine("TestEnd1", 0x31001212, null);

            byte[] p0 = null;
            byte[] g0 = null;

            cryptoEngine0.ExportPandG(ref p0, ref g0);

            Console.WriteLine("P0:");
            Console.WriteLine(p0.DumpArray());

            Console.WriteLine("G0:");
            Console.WriteLine(g0.DumpArray());

            byte[] p1 = null;
            byte[] g1 = null;

            cryptoEngine1.ExportPandG(ref p1, ref g1);

            Console.WriteLine("P1:");
            Console.WriteLine(p1.DumpArray());

            Console.WriteLine("G1:");
            Console.WriteLine(g1.DumpArray());

            byte[] gAModP0 = null;
            cryptoEngine0.StartDiffieHellman(ref gAModP0);

            Console.WriteLine("gAModP0:");
            Console.WriteLine(gAModP0.DumpArray());

            byte[] gAModP1 = null;
            cryptoEngine1.StartDiffieHellman(ref gAModP1);

            Console.WriteLine("gAModP1:");
            Console.WriteLine(gAModP1.DumpArray());

            byte[] sessionKey0 = null;
            cryptoEngine0.GenerateDiffieHellmanSessionKey(gAModP1, ref sessionKey0);

            Console.WriteLine("sessionKey0:");
            Console.WriteLine(sessionKey0.DumpArray());

            byte[] sessionKey1 = null;
            cryptoEngine1.GenerateDiffieHellmanSessionKey(gAModP0, ref sessionKey1);

            Console.WriteLine("sessionKey1:");
            Console.WriteLine(sessionKey1.DumpArray());
        }

        public static void TestMain3()
        {
            CCryptoEngine cryptoEngine0 = new CCryptoEngine("TestEnd0", 0x21001212, null);
            CCryptoEngine cryptoEngine1 = new CCryptoEngine("TestEnd1", 0x31001212, null);

            byte[] randomNumber0 = null;

            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("0. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("1. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("2. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("3. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("4. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("5. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("6. " + randomNumber0.DumpArray());
            cryptoEngine0.GenerateRandomNumber(ref randomNumber0);
            Console.WriteLine("7. " + randomNumber0.DumpArray());

            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            byte[] data = new byte[4096];

            for (int i = 0; i < key.Length; i++)
                key[i] = (byte)(i * i * i + i + 0x56);

            for (int i = 0; i < iv.Length; i++)
                iv[i] = (byte)(i * i + i + 0x1);

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            Console.WriteLine("Kripto oncesi:");
            Console.WriteLine(data.DumpArray());

            cryptoEngine0.AesEndecrypt(iv, key, ref data, 0, data.Length);

            Console.WriteLine("Kriptolama sonrasi:");
            Console.WriteLine(data.DumpArray());

            cryptoEngine0.AesEndecrypt(iv, key, ref data, 0, data.Length);

            Console.WriteLine("Kripto cozme sonrasi:");
            Console.WriteLine(data.DumpArray());
        }

        public static void TestMain4()
        {
            CCryptoEngine cryptoEngine = new CCryptoEngine("TestEnd1", 0x31001212, null);

            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            byte[] data = new byte[4096];

            for (int i = 0; i < key.Length; i++)
                key[i] = (byte)((i * i + i + 0x1) & (0xFF)); 

            for (int i = 0; i < iv.Length; i++)
                iv[i] = (byte)((i * i * i + i * i + 0x32) & (0xFF));

            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            cryptoEngine.AesEndecrypt(iv, key, ref data, 0, data.Length);

            Console.WriteLine("Kriptolama sonrasi:");
            Console.WriteLine(data.DumpArray());
        }

        public static void TestMain()
        {
            CCryptoEngine cryptoEngine0 = new CCryptoEngine("TestEnd0", 0x21001212, null);
            CCryptoEngine cryptoEngine1 = new CCryptoEngine("TestEnd1", 0x31001212, null);

            byte[] p0 = null;
            byte[] g0 = null;

            cryptoEngine0.ExportPandG(ref p0, ref g0);

            byte[] p1 = null;
            byte[] g1 = null;

            cryptoEngine1.ExportPandG(ref p1, ref g1);

            byte[] gAModP0 = null;
            cryptoEngine0.StartDiffieHellman(ref gAModP0);

            byte[] gAModP1 = null;
            cryptoEngine1.StartDiffieHellman(ref gAModP1);

            byte[] sessionKey0 = null;
            cryptoEngine0.GenerateDiffieHellmanSessionKey(gAModP1, ref sessionKey0);

            Console.WriteLine("sessionKey0:");
            Console.WriteLine(sessionKey0.DumpArray());

            byte[] sessionKey1 = null;
            cryptoEngine1.GenerateDiffieHellmanSessionKey(gAModP0, ref sessionKey1);

            Console.WriteLine("sessionKey1:");
            Console.WriteLine(sessionKey1.DumpArray());

            byte[] iv = null;
            cryptoEngine0.GenerateIV(ref iv);

            byte[] data = new byte[631];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)((i * i * i + i * i + 0x32) & (0xFF));

            byte[] dataToBeEncrypted = data.Clone() as byte[];
            cryptoEngine0.AesEndecrypt(iv, sessionKey0, ref dataToBeEncrypted, 13, 529);

            cryptoEngine1.AesEndecrypt(iv, sessionKey1, ref dataToBeEncrypted, 13, 529);

            bool testOk = true;
            int errIndex = -1;

            for(int i=0; i<data.Length; i++)
                if (data[i]!= dataToBeEncrypted[i])
                {
                    testOk = false;
                    errIndex = i;
                }

            Console.WriteLine("Sonuc = " + testOk.ToString() + " Index = " + errIndex.ToString());
        }
    }
}