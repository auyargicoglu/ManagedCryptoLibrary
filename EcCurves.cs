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

namespace ManagedCryptoLibrary
{
    internal class Secp384r1EC : EcCurveInfo
    {
        public Secp384r1EC()
        {
            p = new byte[] {
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE,
                               0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF
            };

            pLen = 48;

            a = new byte[] {
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE,
                               0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFC,
            };

            aLen = 48;

            b = new byte[] {
                               0xB3, 0x31, 0x2F, 0xA7, 0xE2, 0x3E, 0xE7, 0xE4, 0x98, 0x8E, 0x05, 0x6B, 0xE3, 0xF8, 0x2D, 0x19,
                               0x18, 0x1D, 0x9C, 0x6E, 0xFE, 0x81, 0x41, 0x12, 0x03, 0x14, 0x08, 0x8F, 0x50, 0x13, 0x87, 0x5A,
                               0xC6, 0x56, 0x39, 0x8D, 0x8A, 0x2E, 0xD1, 0x9D, 0x2A, 0x85, 0xC8, 0xED, 0xD3, 0xEC, 0x2A, 0xEF,
            };

            bLen = 48;

            gx = new byte[] {
                               0xAA, 0x87, 0xCA, 0x22, 0xBE, 0x8B, 0x05, 0x37, 0x8E, 0xB1, 0xC7, 0x1E, 0xF3, 0x20, 0xAD, 0x74,
                               0x6E, 0x1D, 0x3B, 0x62, 0x8B, 0xA7, 0x9B, 0x98, 0x59, 0xF7, 0x41, 0xE0, 0x82, 0x54, 0x2A, 0x38,
                               0x55, 0x02, 0xF2, 0x5D, 0xBF, 0x55, 0x29, 0x6C, 0x3A, 0x54, 0x5E, 0x38, 0x72, 0x76, 0x0A, 0xB7,
            };

            gxLen = 48;

            gy = new byte[] {
                               0x36, 0x17, 0xDE, 0x4A, 0x96, 0x26, 0x2C, 0x6F, 0x5D, 0x9E, 0x98, 0xBF, 0x92, 0x92, 0xDC, 0x29,
                               0xF8, 0xF4, 0x1D, 0xBD, 0x28, 0x9A, 0x14, 0x7C, 0xE9, 0xDA, 0x31, 0x13, 0xB5, 0xF0, 0xB8, 0xC0,
                               0x0A, 0x60, 0xB1, 0xCE, 0x1D, 0x7E, 0x81, 0x9D, 0x7A, 0x43, 0x1D, 0x7C, 0x90, 0xEA, 0x0E, 0x5F,
            };

            gyLen = 48;

            q = new byte[] {
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC7, 0x63, 0x4D, 0x81, 0xF4, 0x37, 0x2D, 0xDF,
                               0x58, 0x1A, 0x0D, 0xB2, 0x48, 0xB0, 0xA7, 0x7A, 0xEC, 0xEC, 0x19, 0x6A, 0xCC, 0xC5, 0x29, 0x73,
            };

            qLen = 48;

            h = 1;
            Mod = new CurveFastModularReductionFunction(secp384r1Mod);
        }

        private static void CLEAR_WORD32(ref Mpi a, int i, int n)
        {
            for (int j = 0; j < n; j++)
                a.data[i + j] = 0;
        }

        private static void COPY_WORD32(ref Mpi a, int i, Mpi b, int j, int n)
        {
            for (int k = 0; k < n; k++)
                a.data[i + k] = b.data[j + k];
        }

        static CryptoError secp384r1Mod(ref Mpi a, Mpi p)
        {
            CryptoError error;
            Mpi s = new Mpi();
            Mpi t = new Mpi();

            error = a.Grow(96 / Mpi.MPI_INT_SIZE);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = s.Grow(48 / Mpi.MPI_INT_SIZE);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t.Grow(48 / Mpi.MPI_INT_SIZE);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute T = A11 | A10 | A9 | A8 | A7 | A6 | A5 | A4 | A3 | A2 | A1 | A0
            COPY_WORD32(ref t, 0, a, 0, 12);

            //Compute S1 = 0 | 0 | 0 | 0 | 0 | A23 | A22 | A21 | 0 | 0 | 0 | 0
            CLEAR_WORD32(ref s, 0, 4);
            COPY_WORD32(ref s, 4, a, 21, 3);
            CLEAR_WORD32(ref s, 7, 5);

            //Compute T = T + 2 * S1
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute S2 = A23 | A22 | A21 | A20 | A19 | A18 | A17 | A16 | A15 | A14 | A13 | A12
            COPY_WORD32(ref s, 0, a, 12, 12);

            //Compute T = T + S2
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute S3 = A20 | A19 | A18 | A17 | A16 | A15 | A14 | A13 | A12 | A23| A22 | A21
            COPY_WORD32(ref s, 0, a, 21, 3);
            COPY_WORD32(ref s, 3, a, 12, 9);
            
            //Compute T = T + S3
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute S4 = A19 | A18 | A17 | A16 | A15 | A14 | A13 | A12 | A20 | 0 | A23 | 0
            CLEAR_WORD32(ref s, 0, 1);
            COPY_WORD32(ref s, 1, a, 23, 1);
            CLEAR_WORD32(ref s, 2, 1);
            COPY_WORD32(ref s, 3, a, 20, 1);
            COPY_WORD32(ref s, 4, a, 12, 8);
            
            //Compute T = T + S4
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute S5 = 0 | 0 | 0 | 0 | A23 | A22 | A21 | A20 | 0 | 0 | 0 | 0
            CLEAR_WORD32(ref s, 0, 4);
            COPY_WORD32(ref s, 4, a, 20, 4);
            CLEAR_WORD32(ref s, 8, 4);

            //Compute T = T + S5
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute S6 = 0 | 0 | 0 | 0 | 0 | 0 | A23 | A22 | A21 | 0 | 0 | A20
            COPY_WORD32(ref s, 0, a, 20, 1);
            CLEAR_WORD32(ref s, 1, 2);
            COPY_WORD32(ref s, 3, a, 21, 3);
            CLEAR_WORD32(ref s, 6, 6);

            //Compute T = T + S6
            error = t.Add(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute D1 = A22 | A21 | A20 | A19 | A18 | A17 | A16 | A15 | A14 | A13 | A12 | A23
            COPY_WORD32(ref s, 0, a, 23, 1);
            COPY_WORD32(ref s, 1, a, 12, 11);

            //Compute T = T - D1
            error = t.Subtract(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute D2 = 0 | 0 | 0 | 0 | 0 | 0 | 0 | A23 | A22 | A21 | A20 | 0
            CLEAR_WORD32(ref s, 0, 1);
            COPY_WORD32(ref s, 1, a, 20, 4);
            CLEAR_WORD32(ref s, 5, 7);

            //Compute T = T - D2
            error = t.Subtract(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute D3 = 0 | 0 | 0 | 0 | 0 | 0 | 0 | A23 | A23 | 0 | 0 | 0
            CLEAR_WORD32(ref s, 0, 3);
            COPY_WORD32(ref s, 3, a, 23, 1);
            COPY_WORD32(ref s, 4, a, 23, 1);
            CLEAR_WORD32(ref s, 5, 7);

            //Compute T = T - D3
            error = t.Subtract(t, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute (T + 2 * S1 + S2 + S3 + S4 + S5 + S6 - D1 - D2 - D3) mod p
            while (Mpi.Compare(t, p) >= 0)
            {
                error = t.Subtract(t, p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            while (Mpi.CompareInt(t, 0) < 0)
            {
                error = t.Add(t, p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            //Save result
            error = a.Copy(t);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Return status code
            return CryptoError.NO_ERROR;
        }
    }
}