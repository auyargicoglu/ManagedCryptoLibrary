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

        public bool GenerateRandomNumber(ref byte[]randomNumber)
        {
            byte[] intermediateArray = new byte[20];

            randomNumber = hashSeed.SubArray(0, 20);
            rngSeed.NextBytes(intermediateArray);
            hashSeed.CopyIn(0, intermediateArray, 0, 20);

            SHA512.Sha512Compute(hashSeed, 0, hashSeed.Length, ref hashOut, 0, out int _);

            byte[] temp = hashSeed;
            hashSeed = hashOut;
            hashOut = temp;

            return true;
        }

        public bool GenerateIV(ref byte[] iv)
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