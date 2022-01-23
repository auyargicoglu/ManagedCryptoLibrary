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
    public class EllipticCurve
    {
        public const int eccConstantSignatureLength = 128;

        private const int x509PatternLength = 24;

        private EllipticCurveCore ellipticCurve = null;

        public EllipticCurve()
        {
            ellipticCurve = new EllipticCurveCore();
            ellipticCurve.InitDomainParameters();
            ellipticCurve.LoadDomainParameters(new Secp384r1EC());
        }

        public bool GenerateECKeys(out byte[] outPrivateKey, out byte[] outPublicKey)
        {
            CryptoError error;
            int privateKeySize;
            int publicKeySize;
            Mpi privateKey = new Mpi();
            EcPoint publicKey = new EcPoint();

            outPrivateKey = null;
            outPublicKey = null;

            error = ellipticCurve.GenerateKeyPair(ref privateKey, ref publicKey);
            if (error != CryptoError.NO_ERROR)
                return false;

            privateKeySize = privateKey.GetByteLength();

            error = ellipticCurve.GetPublicKeySize(out publicKeySize);
            if (error != CryptoError.NO_ERROR)
                return false;

            outPrivateKey = new byte[privateKeySize];
            outPublicKey = new byte[publicKeySize];      

            privateKey.Export(ref outPrivateKey, 0, privateKeySize, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            ellipticCurve.Export(publicKey, ref outPublicKey);

            return true;
        }

        public bool Sign(byte[] inPrivateKey, byte[] inData, int offset, int length, ref byte[] outSignature)
        {
            if (inPrivateKey == null)
                return false;

            if (inData == null)
                return false;

            if (outSignature == null)
                return false;

            if (offset < 0)
                return false;

            if (length <= 0)
                return false;

            if (outSignature.Length != eccConstantSignatureLength)
                return false;

            CryptoError error;
            Mpi privateKey = new Mpi();
            EcdsaSignature signature = new EcdsaSignature();
            byte[] signatureTempBuffer = new byte[256];
            byte[] digest = new byte[48];

            error = privateKey.Import(inPrivateKey, 0, inPrivateKey.Length, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = SHA512.Sha384Compute(inData, offset, length, ref digest, 0, out int _);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = ellipticCurve.GenerateSignature(privateKey, digest, ref signature);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = WriteSignature(signature, ref signatureTempBuffer, 0, out int signatureLength);
            if (error != CryptoError.NO_ERROR)
                return false;

            outSignature.Zeroize();
            WriteX509Pattern(ref outSignature, signatureLength);
            outSignature.CopyIn(x509PatternLength, signatureTempBuffer, 0, signatureLength);

            return true;
        }

        public bool VerifySignature(byte[] inPublicKey, byte[] inData, int offset, int length, byte[] inSignature)
        {
            if (inPublicKey == null)
                return false;

            if (inData == null)
                return false;

            if (inSignature == null)
                return false;

            if (offset < 0)
                return false;

            if (length <= 0)
                return false;

            CryptoError error;
            EcPoint publicKey = new EcPoint();
            EcdsaSignature signature = new EcdsaSignature();
            byte[] digest = new byte[48];

            if (inSignature.Length - x509PatternLength <= 0)
                return false;

            if (CheckX509Pattern(inSignature, inSignature.Length, out int actualSignatureLength) == false)
                return false;

            error = ReadSignature(inSignature, x509PatternLength, ref signature, out int _);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = ellipticCurve.Import(ref publicKey, inPublicKey);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = SHA512.Sha384Compute(inData, offset, length, ref digest, 0, out int _);
            if (error != CryptoError.NO_ERROR)
                return false;

            error = ellipticCurve.VerifySignature(publicKey, digest, signature);
            if (error != CryptoError.NO_ERROR)
                return false;

            return true;
        }

        private CryptoError WriteSignature(EcdsaSignature signature, ref byte[] outputStream, int offset, out int length)
        {
            int rLen;
            int sLen;

            length = 0;

            //Calculate the length of R
            rLen = signature.r.GetByteLength();

            //Calculate the length of S
            sLen = signature.s.GetByteLength();

            if (rLen == 0 || sLen == 0)
                return CryptoError.ERROR_INVALID_LENGTH;

            if (rLen + sLen + 8 + offset >  outputStream.Length)
                return CryptoError.ERROR_OUT_BUFFER_INSUFFICIENT;

            outputStream.CopyIn(0, BitConverter.GetBytes(rLen), 0, 4);
            outputStream.CopyIn(4, BitConverter.GetBytes(sLen), 0, 4);

            signature.r.Export(ref outputStream, 8, rLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            signature.s.Export(ref outputStream, 8 + rLen, sLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);

            length = 8 + rLen + sLen;
            return CryptoError.NO_ERROR;
        }

        private CryptoError ReadSignature(byte[] inputStream, int offset, ref EcdsaSignature signature, out int length)
        {
            int rLen;
            int sLen;

            length = 0;

            if (offset + 8 > inputStream.Length)
                return CryptoError.ERROR_INVALID_INPUT_BUFFER;

            rLen = BitConverter.ToInt32(inputStream.SubArray(offset, 4));
            offset += 4;
            sLen = BitConverter.ToInt32(inputStream.SubArray(offset, 4));
            offset += 4;

            if (offset + rLen + sLen > inputStream.Length)
                return CryptoError.ERROR_INVALID_INPUT_BUFFER;

            if ((rLen <= 0) || (rLen > 48))
                return CryptoError.ERROR_INVALID_LENGTH;

            if ((sLen <= 0) || (sLen > 48))
                return CryptoError.ERROR_INVALID_LENGTH;

            signature.r.Import(inputStream, offset, rLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            offset += rLen;
            signature.s.Import(inputStream, offset, sLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            length = offset + sLen;

            return CryptoError.NO_ERROR;
        }

        private void WriteX509Pattern(ref byte[] signatureHeader, int signatureLength)
        {
            int i;
            int j;

            signatureHeader[0] = (byte)'V';
            signatureHeader[1] = (byte)'Z';
            signatureHeader[2] = (byte)'B';
            signatureHeader[3] = (byte)'S';
            signatureHeader[4] = (byte)'H';
            signatureHeader[5] = (byte)'J';
            signatureHeader[6] = (byte)'D';
            signatureHeader[7] = (byte)'P';
            signatureHeader[8] = (byte)'H';
            signatureHeader[9] = (byte)'M';
            signatureHeader[10] = (byte)'V';
            signatureHeader[11] = 0;

            for (i = 0; i < 11; i++)
                signatureHeader[i]--;

            signatureLength = (int)CryptoCommon.swapInt32((UInt32)signatureLength);
            byte[] lengthBytes = BitConverter.GetBytes(signatureLength);

            for (i = 0, j=12; i < 4; i++, j++)
                signatureHeader[j] = lengthBytes[i];
        }

        private bool CheckX509Pattern(byte[] signatureHeader, int bufferLength, out int signatureLength)
        {
            signatureLength = 0;
            if (bufferLength < x509PatternLength)
                return false;

            byte[] tempSignatureHeader = new byte[12];
            tempSignatureHeader.CopyIn(0, signatureHeader, 0, 12);

            for (int i = 0; i < 11; i++)
                tempSignatureHeader[i]++;

            if (tempSignatureHeader[0] != (byte)'V')
                return false;
            if (tempSignatureHeader[1] != (byte)'Z')
                return false;
            if (tempSignatureHeader[2] != (byte)'B')
                return false;
            if (tempSignatureHeader[3] != (byte)'S')
                return false;
            if (tempSignatureHeader[4] != (byte)'H')
                return false;
            if (tempSignatureHeader[5] != (byte)'J')
                return false;
            if (tempSignatureHeader[6] != (byte)'D')
                return false;
            if (tempSignatureHeader[7] != (byte)'P')
                return false;
            if (tempSignatureHeader[8] != (byte)'H')
                return false;
            if (tempSignatureHeader[9] != (byte)'M')
                return false;
            if (tempSignatureHeader[10] != (byte)'V')
                return false;
            if (tempSignatureHeader[11] != 0)
                return false;

            signatureLength = BitConverter.ToInt32(signatureHeader.SubArray(12, 4));
            signatureLength = (int)CryptoCommon.swapInt32((UInt32)signatureLength);

            if (signatureLength + x509PatternLength > bufferLength)
                return false;

            return true;
        }
    }
}