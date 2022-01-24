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
    public class CCryptoEngine : IDisposable
    {
        public const int AesKeySize = 32;

        public const int AesIvSize = 16;

        public const int EccSignatureSize = 128;

        private Random rng;

        private string owner;

        private Aes aesEndecrypter;

        private RandomNumberGenerator randomNumberGenerator;

        private DiffieHellman diffieHellman;

        private EllipticCurve ellipticCurve;

        private bool disposedValue;

        #region Constructor and Destructor

        public CCryptoEngine(string ownerSt, int ownerSeed, string diffieHellmanGroupName)
        {
            rng = new Random((int)DateTime.UtcNow.ToFileTimeUtc() + ownerSeed);
            owner = ownerSt;

            byte[] randomBytes = new byte[128];
            rng.NextBytes(randomBytes);

            byte[] seed = new byte[128];
            for (int i = 0; i < randomBytes.Length; i++)
                seed[i] = randomBytes[i];

            if (diffieHellmanGroupName == null)
                diffieHellmanGroupName = "diffie-hellman-group16";

            randomNumberGenerator = new RandomNumberGenerator();
            aesEndecrypter = new Aes();
            diffieHellman = new DiffieHellman(diffieHellmanGroupName);
            ellipticCurve = new EllipticCurve();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // This is a web assembly library, no unmanaged objects

                rng = null;
                randomNumberGenerator = null;
                aesEndecrypter = null;
                diffieHellman = null;
                ellipticCurve = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region AES Functions

        public bool AesEndecrypt(byte[] iv, byte[] key, byte[] data, int offset, int length)
        {
            return aesEndecrypter.AesEndecrypt(iv, key, data, offset, length);
        }

        #endregion

        #region Random numberand aes iv generation

        public bool GenerateRandomNumber(int length, out byte[] randomNumber)
        {
            return randomNumberGenerator.GenerateRandomNumber(length, out randomNumber);
        }

        public bool GenerateIV(out byte[] iv)
        {
            return randomNumberGenerator.GenerateIV(out iv);
        }

        #endregion

        #region Hash and other digital signature functions

        public bool Sha512(byte[] data, ref byte[] digest)
        {
            digest = new byte[64];
            digest.Zeroize();

            if (SHA512.Sha512Compute(data, 0, data.Length, ref digest, 0, out int _) != CryptoError.NO_ERROR)
                return false;

            return true;
        }

        #endregion

        #region Diffie Hellman functions

        public bool ExportPandG(ref byte[] p, ref byte[] g)
        {
            return diffieHellman.ExportPandG(ref p, ref g);
        }

        public bool StartDiffieHellman(out byte[] gAModP)
        {
            return diffieHellman.StartDiffieHellman(out gAModP);
        }

        public bool GenerateDiffieHellmanSessionKey(byte[] gBModP, out byte[] sessionKey)
        {
            return diffieHellman.GenerateDiffieHellmanSessionKey(gBModP, out sessionKey);
        }

        #endregion

        #region EC key generation function

        public bool GenerateECKeys(out byte[] outPrivateKey, out byte[] outPublicKey)
        {
            if (ellipticCurve.GenerateECKeys(out outPrivateKey, out outPublicKey) == false)
                return false;

            return TestECKeyPair(outPublicKey, outPrivateKey);
        }

        public bool Sign(byte[] inPrivateKey, byte[] inData, ref byte[] outSignature)
        {
            Console.WriteLine("Private Key:");
            Console.WriteLine(inPrivateKey.DumpArray());
            Console.WriteLine("Data:");
            Console.WriteLine(inData.DumpArray());

            outSignature = new byte[EllipticCurve.eccConstantSignatureLength];
            return ellipticCurve.Sign(inPrivateKey, inData, 0, inData.Length, ref outSignature);
        }

        public bool VerifySignature(byte[] inPublicKey, byte[] inData, int offset, int length, byte[] inSignature)
        {
            return ellipticCurve.VerifySignature(inPublicKey, inData, offset, length, inSignature);
        }

        private bool TestECKeyPair(byte[] inPublicKey, byte[] inPrivateKey)
        {
            byte[] data = new byte[221];
            byte[] signature = null;

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            if (Sign(inPrivateKey, data, ref signature) == false)
                return false;

            return VerifySignature(inPublicKey, data, 0, data.Length, signature);
        }

        #endregion
    }
}
