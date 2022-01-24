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
    internal class RandomNumberGenerator
    {
        private Random rngSeed;

        private byte[] hashSeed;

        private byte[] hashOut;

        public RandomNumberGenerator()
        {
            rngSeed = new Random((int)DateTime.UtcNow.ToFileTime());
            hashSeed = new byte[64];
            hashOut = new byte[64];

            rngSeed.NextBytes(hashSeed);
            SHA512.Sha512Compute(hashSeed, 0, hashSeed.Length, ref hashOut, 0, out int _);

            byte[] temp = hashSeed;
            hashSeed = hashOut;
            hashOut = temp;
        }

        public bool GenerateRandomNumber(int length, out byte[]randomNumber)
        {
            const int randomNumberBlockSize = 32;
            int remaining;
            int offset;

            int numberOfBlocks = (length + randomNumberBlockSize - 1) / randomNumberBlockSize;
            byte[] intermediateArray = new byte[randomNumberBlockSize];

            randomNumber = new byte[length];
            remaining = length;
            offset = 0;

            for (int i = 0; i < numberOfBlocks; i++, remaining -= randomNumberBlockSize, offset += randomNumberBlockSize)
            {
                int blockLength = remaining > randomNumberBlockSize ? randomNumberBlockSize : remaining;

                randomNumber.CopyIn(offset, hashSeed, 0, blockLength);
                rngSeed.NextBytes(intermediateArray);
                hashSeed.CopyIn(0, intermediateArray, 0, intermediateArray.Length);

                SHA512.Sha512Compute(hashSeed, 0, hashSeed.Length, ref hashOut, 0, out int _);

                byte[] temp = hashSeed;
                hashSeed = hashOut;
                hashOut = temp;
            }

            return true;
        }

        public bool GenerateIV(out byte[] iv)
        {
            byte[] intermediateArray = new byte[16];

            iv = hashSeed.SubArray(0, 16);
            rngSeed.NextBytes(intermediateArray);
            hashSeed.CopyIn(0, intermediateArray, 0, 16);

            SHA512.Sha512Compute(hashSeed, 0, hashSeed.Length, ref hashOut, 0, out int _);

            byte[] temp = hashSeed;
            hashSeed = hashOut;
            hashOut = temp;

            return true;
        }
    }
}