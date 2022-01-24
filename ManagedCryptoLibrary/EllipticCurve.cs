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
    internal class EcCurveInfo
    {
        public delegate CryptoError CurveFastModularReductionFunction(ref Mpi a, Mpi p);

        public byte[] p;
        public int pLen;

        public byte[] a;
        public int aLen;

        public byte[] b;
        public int bLen;

        public byte[] gx;
        public int gxLen;

        public byte[] gy;
        public int gyLen;

        public byte[] q;
        public int qLen;

        public UInt32 h;
        public CurveFastModularReductionFunction Mod;

        public EcCurveInfo()
        {
            p = new byte[66];
            pLen = 0;

            a = new byte[66];
            aLen = 0;

            b = new byte[66];
            bLen = 0;

            gx = new byte[66];
            gxLen = 0;

            gy = new byte[66];
            gyLen = 0;

            q = new byte[66];
            qLen = 0;

            h = 0;
            Mod = null;
        }

        public string Dump()
        {
            string retVal = "P:" + "\n" + this.p.DumpArray(this.pLen);

            retVal += ("A:" + "\n" + this.a.DumpArray(this.aLen));
            retVal += ("B:" + "\n" + this.b.DumpArray(this.bLen));
            retVal += ("GX:" + "\n" + this.gx.DumpArray(this.gxLen));
            retVal += ("GY:" + "\n" + this.gy.DumpArray(this.gyLen));
            retVal += ("Q:" + "\n" + this.gx.DumpArray(this.qLen));
            retVal += ("H:" + "\n" + this.h.ToString("X8"));

            return retVal;
        }
    }

    internal class EcPoint
    {
        public Mpi x; ///<x-coordinate

        public Mpi y; ///<y-coordinate

        public Mpi z; ///<z-coordinate

        public EcPoint()
        {
            x = new Mpi();
            y = new Mpi();
            z = new Mpi();
        }

        public EcPoint Clone()
        {
            EcPoint retVal = new EcPoint();

            retVal.x = this.x.Clone();
            retVal.y = this.y.Clone();
            retVal.z = this.z.Clone();

            return retVal;
        }

        public string Dump()
        {
            string retVal = "x:" + "\n" + this.x.Dump();

            retVal += ("y:" + "\n" + this.y.Dump());
            retVal += ("z:" + "\n" + this.z.Dump());

            return retVal;
        }
    }

    internal class EcDomainParameters
    {
        public Mpi p;              ///<Prime

        public Mpi a;              ///<Curve parameter a

        public Mpi b;              ///<Curve parameter b

        public EcPoint g;          ///<Base point G

        public Mpi q;              ///<Order of the point G

        public UInt32 h;           ///<Cofactor h

        public EcCurveInfo.CurveFastModularReductionFunction Mod;

        public EcDomainParameters()
        {
            p = new Mpi();
            a = new Mpi();
            b = new Mpi();
            g = new EcPoint();
            q = new Mpi();
            h = 0U;
            Mod = null;
        }

        public string Dump()
        {
            string retVal = "p:" + "\n" + this.p.Dump();

            retVal += ("a:" + "\n" + this.a.Dump());
            retVal += ("b:" + "\n" + this.b.Dump());
            retVal += ("g:" + "\n" + this.g.Dump());
            retVal += ("q:" + "\n" + this.q.Dump());
            retVal += h.ToString("X8");

            return retVal;
        }
    }

    internal class EcdsaSignature
    {
        public Mpi r;

        public Mpi s;

        public EcdsaSignature()
        {
            r = new Mpi();
            s = new Mpi();
        }

        public string Dump()
        {
            string retVal = "x:" + "\n" + this.r.Dump();
            retVal += ("y:" + "\n" + this.s.Dump());

            return retVal;
        }
    }

    internal class EllipticCurveCore
    {
        private EcDomainParameters domainParameters = null;

        public void InitDomainParameters()
        {
            Mpi.SetRandomNumberGenerator(new Random());

            domainParameters = new EcDomainParameters();
        }

        public CryptoError LoadDomainParameters(EcCurveInfo curveInfo)
        {
            CryptoError error;

            if (domainParameters == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (curveInfo == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            error = domainParameters.p.Import(curveInfo.p, 0, curveInfo.pLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.a.Import(curveInfo.a, 0, curveInfo.aLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.b.Import(curveInfo.b, 0, curveInfo.bLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.g.x.Import(curveInfo.gx, 0, curveInfo.gxLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.g.y.Import(curveInfo.gy, 0, curveInfo.gyLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.q.Import(curveInfo.q, 0, curveInfo.qLen, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = domainParameters.g.z.SetValue(1);
            if (error != CryptoError.NO_ERROR)
                return error;

            domainParameters.h = curveInfo.h;
            domainParameters.Mod = curveInfo.Mod;

            return CryptoError.NO_ERROR;
        }

        public CryptoError GetPublicKeySize(out int keySize)
        {
            keySize = 0;

            if (domainParameters == null)
                return CryptoError.ERROR_DOMAIN_PARAMETERS_UNINITIALIZED;

            if (domainParameters.p == null)
                return CryptoError.ERROR_DOMAIN_PARAMETERS_UNINITIALIZED;

            if (domainParameters.p.data == null)
                return CryptoError.ERROR_DOMAIN_PARAMETERS_UNINITIALIZED;

            keySize = domainParameters.p.GetByteLength() * 2 + 1;
            return CryptoError.NO_ERROR;
        }

        public CryptoError GenerateKeyPair(ref Mpi privateKey, ref EcPoint publicKey)
        {
            CryptoError error;

            error = GeneratePrivateKey(ref privateKey);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = GeneratePublicKey(privateKey, ref publicKey);
            if (error != CryptoError.NO_ERROR)
                return error;

            return error;
        }

        public CryptoError GeneratePrivateKey(ref Mpi privateKey)
        {
            CryptoError error;
            int n;

            if (domainParameters == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (privateKey == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            //Let N be the bit length of q
            n = domainParameters.q.GetBitLength();

            error = privateKey.Random(n);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (Mpi.Compare(privateKey, domainParameters.q) >= 0)
                privateKey.ShiftRight(1);

            return CryptoError.NO_ERROR;
        }

        public CryptoError GeneratePublicKey(Mpi privateKey, ref EcPoint publicKey)
        {
            CryptoError error;

            if (domainParameters == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (privateKey == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            if (publicKey == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            error = Multiply(ref publicKey, privateKey, domainParameters.g);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = Affinify(ref publicKey, publicKey);
            if (error != CryptoError.NO_ERROR)
                return error;

            return CryptoError.NO_ERROR;
        }

        public CryptoError Copy(ref EcPoint r, EcPoint s)
        {
            CryptoError error;

            //R and S are the same instance?
            if (r == s)
                return CryptoError.NO_ERROR;

            error = r.x.Copy(s.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = r.y.Copy(s.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            return r.z.Copy(s.z);
        }

        public CryptoError Import(ref EcPoint r, byte[] data)
        {
            CryptoError error;

            int k;

            //Get the length in octets of the prime
            k = domainParameters.p.GetByteLength();

            //Check the length of the octet string
            if (data.Length != (k * 2 + 1))
                return CryptoError.ERROR_ILLEGAL_PARAMETER;

            //Compressed point representation is not supported
            if (data[0] != 0x04)
                return CryptoError.ERROR_ILLEGAL_PARAMETER;

            //Convert the x-coordinate to a multiple precision integer
            error = r.x.Import(data, 1, k, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Convert the y-coordinate to a multiple precision integer
            error = r.y.Import(data, k + 1, k, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Successful processing
            return CryptoError.NO_ERROR;
        }

        public CryptoError Export(EcPoint r, ref byte[] data)
        {
            CryptoError error;

            int k;

            //Get the length in octets of the prime
            k = domainParameters.p.GetByteLength();

            //Check the length of the octet string
            if (data.Length != (k * 2 + 1))
                return CryptoError.ERROR_ILLEGAL_PARAMETER;

            //Compressed point representation is not supported
            data[0] = 0x04;

            //Convert the x-coordinate to a multiple precision integer
            error = r.x.Export(ref data, 1, k, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Convert the y-coordinate to a multiple precision integer
            error = r.y.Export(ref data, k + 1, k, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Successful processing
            return CryptoError.NO_ERROR;
        }

        public CryptoError Affinify(ref EcPoint r, EcPoint s)
        {
            CryptoError error;
            Mpi a = new Mpi();
            Mpi b = new Mpi();

            if (r == s)
                s = r.Clone();

            //Point at the infinity?
            if (Mpi.CompareInt(s.z, 0) == 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            //Compute a = 1/Sz mod p
            error = a.InverseMod(s.z, domainParameters.p);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Set Rx = a^2 * Sx mod p
            error = SquareMod(ref b, a);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = MultiplyMod(ref r.x, b, s.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Set Ry = a^3 * Sy mod p
            error = MultiplyMod(ref b, b, a);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = MultiplyMod(ref r.y, b, s.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Set Rz = 1
            error = r.z.SetValue(1);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Return status code
            return CryptoError.NO_ERROR;
        }

        public CryptoError Projectify(ref EcPoint r, EcPoint s)
        {
            CryptoError error;

            if (r == s)
                s = r.Clone();

            //Copy point
            error = Copy(ref r, s);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Map the point to projective space
            return r.z.SetValue(1);
        }

        private CryptoError Multiply(ref EcPoint r, Mpi d, EcPoint s)
        {
            CryptoError error;
            int i;
            Mpi h = new Mpi();

            if (r == s)
                s = r.Clone();

            if (r.x == d)
                d = r.x.Clone();

            if (r.y == d)
                d = r.y.Clone();

            if (r.z == d)
                d = r.z.Clone();

            //Check whether d == 0
            if (Mpi.CompareInt(d, 0) == 0)
            {
                //Set R = (1, 1, 0)
                error = r.x.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.SetValue(0);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            //Check whether d == 1
            else if (Mpi.CompareInt(d, 1) == 0)
            {
                //Set R = S
                error = r.x.Copy(s.x);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.Copy(s.y);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.Copy(s.z);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            //Check whether Sz == 0
            else if (Mpi.CompareInt(s.z, 0) == 0)
            {
                //Set R = (1, 1, 0)
                error = r.x.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.SetValue(0);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                //Check whether Sz != 1
                if (Mpi.CompareInt(s.z, 1) != 0)
                {
                    //Normalize S
                    error = Affinify(ref r, s);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = Projectify(ref r, r);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else
                {
                    //Set R = S
                    error = r.x.Copy(s.x);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.y.Copy(s.y);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.z.Copy(s.z);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                //Left-to-right binary method
                //Precompute h = 3 * d
                error = h.Add(d, d);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = h.Add(h, d);
                if (error != CryptoError.NO_ERROR)
                    return error;

                //Scalar multiplication
                for (i = h.GetBitLength() - 2; i >= 1; i--)
                {
                    //Point doubling
                    error = Double(ref r, r);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    //Check whether h(i) == 1 and k(i) == 0
                    if (h.GetBitValue(i) && !d.GetBitValue(i))
                    {
                        //Compute R = R + S
                        error = FullAdd(ref r, r, s);
                        if (error != CryptoError.NO_ERROR)
                            return error;
                    }
                    //Check whether h(i) == 0 and k(i) == 1
                    else if (!h.GetBitValue(i) && d.GetBitValue(i))
                    {
                        //Compute R = R - S
                        error = FullSubtract(ref r, r, s);
                        if (error != CryptoError.NO_ERROR)
                            return error;
                    }
                }
            }

            //Return status code
            return CryptoError.NO_ERROR;
        }

        private CryptoError FullAdd(ref EcPoint r, EcPoint s, EcPoint t)
        {
            CryptoError error;

            if (r == s)
                s = r.Clone();

            if (r == t)
                t = r.Clone();

            //Check whether Sz == 0
            if (Mpi.CompareInt(s.z, 0) == 0)
            {
                //Set R = T
                error = r.x.Copy(t.x);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.Copy(t.y);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.Copy(t.z);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            //Check whether Tz == 0
            else if (Mpi.CompareInt(t.z, 0) == 0)
            {
                //Set R = S
                error = r.x.Copy(s.x);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.Copy(s.y);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.Copy(s.z);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                //Compute R = S + T
                error = Add(ref r, s, t);
                if (error != CryptoError.NO_ERROR)
                    return error;

                //Check whether R == (0, 0, 0)
                if (Mpi.CompareInt(r.x, 0) == 0 && Mpi.CompareInt(r.y, 0) == 0 && Mpi.CompareInt(r.z, 0) == 0)
                {
                    //Compute R = 2 * S
                    error = Double(ref r, s);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError FullSubtract(ref EcPoint r, EcPoint s, EcPoint t)
        {
            CryptoError error;
            EcPoint u = new EcPoint();

            if (r == s)
                s = r.Clone();

            if (r == t)
                t = r.Clone();

            //Set Ux = Tx and Uz = Tz
            error = u.x.Copy(t.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = u.z.Copy(t.z);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = u.y.Subtract(domainParameters.p, t.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = FullAdd(ref r, s, u);
            if (error != CryptoError.NO_ERROR)
                return error;

            return CryptoError.NO_ERROR;
        }

        public CryptoError Double(ref EcPoint r, EcPoint s)
        {
            CryptoError error;
            Mpi t1 = new Mpi();
            Mpi t2 = new Mpi();
            Mpi t3 = new Mpi();
            Mpi t4 = new Mpi();
            Mpi t5 = new Mpi();

            if (r == s)
                s = r.Clone();

            error = t1.Copy(s.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t2.Copy(s.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t3.Copy(s.z);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Point at the infinity?
            if (Mpi.CompareInt(t3, 0) == 0)
            {
                //Set R = (1, 1, 0)
                //Set R = (1, 1, 0)
                error = r.x.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.SetValue(0);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                //SECP K1 elliptic curve?
                error = SquareMod(ref t4, t3);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t5, t1, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t4, t1, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t5, t4, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t4, t5, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t4, t4, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t3, t3, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t3, t3, t3);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t5, t1, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t5, t5, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t5, t5, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t1, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t1, t1, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t1, t1, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t2, t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t2, t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t2, t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t5, t5, t1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t5, t4, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t2, t5, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.x.Copy(t1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.Copy(t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.Copy(t3);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError Add(ref EcPoint r, EcPoint s, EcPoint t)
        {
            CryptoError error;
            Mpi t1 = new Mpi();
            Mpi t2 = new Mpi();
            Mpi t3 = new Mpi();
            Mpi t4 = new Mpi();
            Mpi t5 = new Mpi();
            Mpi t6 = new Mpi();
            Mpi t7 = new Mpi();

            if (r == s)
                s = r.Clone();

            if (r == t)
                t = r.Clone();

            error = t1.Copy(s.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t2.Copy(s.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t3.Copy(s.z);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t4.Copy(t.x);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t5.Copy(t.y);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Check whether Tz != 1
            if (Mpi.CompareInt(t.z, 1) != 0)
            {
                error = t6.Copy(t.z);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t7, t6);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t1, t1, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t7, t6, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t2, t2, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            error = SquareMod(ref t7, t3);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = MultiplyMod(ref t4, t4, t7);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = MultiplyMod(ref t7, t3, t7);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = MultiplyMod(ref t5, t5, t7);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = SubtractMod(ref t4, t1, t4);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = SubtractMod(ref t5, t2, t5);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Check whether t4 == 0
            if (Mpi.CompareInt(t4, 0) == 0)
            {
                //Check whether t5 == 0
                if (Mpi.CompareInt(t5, 0) == 0)
                {
                    //Set R = (0, 0, 0)
                    error = r.x.SetValue(0);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.y.SetValue(0);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.z.SetValue(0);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else
                {
                    //Set R = (1, 1, 0)
                    error = r.x.SetValue(1);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.y.SetValue(1);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = r.z.SetValue(0);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
            }
            else
            {
                error = AddMod(ref t1, t1, t1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t1, t1, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t2, t2, t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t2, t2, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                //Check whether Tz != 1
                if (Mpi.CompareInt(t.z, 1) != 0)
                {
                    //Compute t3 = t3 * t6
                    error = MultiplyMod(ref t3, t3, t6);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = MultiplyMod(ref t3, t3, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t7, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t4, t4, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t7, t1, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SquareMod(ref t1, t5);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t1, t1, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = AddMod(ref t6, t1, t1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t7, t7, t6);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t5, t5, t7);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = MultiplyMod(ref t4, t2, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = SubtractMod(ref t2, t5, t4);
                if (error != CryptoError.NO_ERROR)
                    return error;

                //Compute t2 = t2 / 2
                if (t2.IsEven())
                {
                    error = t2.ShiftRight(1);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else
                {
                    error = t2.Add(t2, domainParameters.p);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = t2.ShiftRight(1);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = r.x.Copy(t1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.y.Copy(t2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = r.z.Copy(t3);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError AddMod(ref Mpi r, Mpi a, Mpi b)
        {
            CryptoError error;

            if (r == a)
                a = r.Clone();

            if (r == b)
                b = r.Clone();

            //Compute R = A + B
            error = r.Add(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute R = (A + B) mod p
            if (Mpi.Compare(r, domainParameters.p) >= 0)
            {
                error = r.Subtract(r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError SubtractMod(ref Mpi r, Mpi a, Mpi b)
        {
            CryptoError error;

            if (r == a)
                a = r.Clone();

            if (r == b)
                b = r.Clone();

            //Compute R = A - B
            error = r.Subtract(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute R = (A - B) mod p
            if (Mpi.CompareInt(r, 0) < 0)
            {
                error = r.Add(r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError MultiplyMod(ref Mpi r, Mpi a, Mpi b)
        {
            CryptoError error;

            if (r == a)
                a = r.Clone();

            if (r == b)
                b = r.Clone();

            error = r.Multiply(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (domainParameters.Mod != null)
            {
                error = domainParameters.Mod(ref r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                error = r.Mod(r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError SquareMod(ref Mpi r, Mpi a)
        {
            CryptoError error;

            if (r == a)
                a = r.Clone();

            //Compute R = A ^ 2
            error = r.Multiply(a, a);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (domainParameters.Mod != null)
            {
                error = domainParameters.Mod(ref r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                error = r.Mod(r, domainParameters.p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private UInt32 TwinMultiplyInteger(UInt32 t)
        {
            UInt32 h;

            //Check the value of T
            if (18 <= t && t < 22)
            {
                h = 9;
            }
            else if (14 <= t && t < 18)
            {
                h = 10;
            }
            else if (22 <= t && t < 24)
            {
                h = 11;
            }
            else if (4 <= t && t < 12)
            {
                h = 14;
            }
            else
            {
                h = 12;
            }

            //Return value
            return h;
        }

        private CryptoError TwinMultiply(ref EcPoint r, Mpi d0, EcPoint s, Mpi d1, EcPoint t)
        {
            CryptoError error;
            int k;
            int m;
            int m0;
            int m1;
            UInt32 c0;
            UInt32 c1;
            UInt32 h0;
            UInt32 h1;
            int u0;
            int u1;
            EcPoint spt = new EcPoint();
            EcPoint smt = new EcPoint();

            //Precompute SpT = S + T
            error = FullAdd(ref spt, s, t);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Precompute SmT = S - T
            error = FullSubtract(ref smt, s, t);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Let m0 be the bit length of d0
            m0 = d0.GetBitLength();
            //Let m1 be the bit length of d1
            m1 = d1.GetBitLength();
            //Let m = MAX(m0, m1)
            m = m0 > m1 ? m0 : m1;

            //Let c be a 2 x 6 binary matrix
            c0 = d0.GetBitValue(m - 4) == true ? 1U : 0;
            c0 |= d0.GetBitValue(m - 3) == true ? 2U : 0;
            c0 |= d0.GetBitValue(m - 2) == true ? 4U : 0;
            c0 |= d0.GetBitValue(m - 1) == true ? 8U : 0;

            c1 = d1.GetBitValue(m - 4) == true ? 1U : 0;
            c1 |= d1.GetBitValue(m - 3) == true ? 2U : 0;
            c1 |= d1.GetBitValue(m - 2) == true ? 4U : 0;
            c1 |= d1.GetBitValue(m - 1) == true ? 8U : 0;

            //Set R = (1, 1, 0)
            error = r.x.SetValue(1);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = r.y.SetValue(1);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = r.z.SetValue(0);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Calculate both multiplications at the same time
            for (k = m; k >= 0; k--)
            {
                //Compute h(0) = 16 * c(0,1) + 8 * c(0,2) + 4 * c(0,3) + 2 * c(0,4) + c(0,5)
                h0 = c0 & 0x1FU;

                //Check whether c(0,0) == 1
                if ((c0 & 0x20U) != 0)
                {
                    h0 = 31U - h0;
                }

                //Compute h(1) = 16 * c(1,1) + 8 * c(1,2) + 4 * c(1,3) + 2 * c(1,4) + c(1,5)
                h1 = c1 & 0x1FU;

                //Check whether c(1,0) == 1
                if ((c1 & 0x20U) != 0)
                {
                    h1 = 31U - h1;
                }

                //Compute u(0)
                if (h0 < TwinMultiplyInteger(h1))
                {
                    u0 = 0;
                }
                else if ((c0 & 0x20U) != 0)
                {
                    u0 = -1;
                }
                else
                {
                    u0 = 1;
                }

                //Compute u(1)
                if (h1 < TwinMultiplyInteger(h0))
                {
                    u1 = 0;
                }
                else if ((c1 & 0x20U) != 0)
                {
                    u1 = -1;
                }
                else
                {
                    u1 = 1;
                }

                //Update c matrix
                c0 <<= 1;
                c0 |= d0.GetBitValue(k - 5) == true ? 1U : 0;
                c0 ^= (u0 != 0) ? 0x20U : 0x00;
                c1 <<= 1;
                c1 |= d1.GetBitValue(k - 5) == true ? 1U : 0;
                c1 ^= (u1 != 0) ? 0x20U : 0x00;

                //Point doubling
                error = Double(ref r, r);
                if (error != CryptoError.NO_ERROR)
                    return error;

                //Check u(0) and u(1)
                if (u0 == -1 && u1 == -1)
                {
                    //Compute R = R - SpT
                    error = FullSubtract(ref r, r, spt);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == -1 && u1 == 0)
                {
                    //Compute R = R - S
                    error = FullSubtract(ref r, r, s);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == -1 && u1 == 1)
                {
                    //Compute R = R - SmT
                    error = FullSubtract(ref r, r, smt);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == 0 && u1 == -1)
                {
                    //Compute R = R - T
                    error = FullSubtract(ref r, r, t);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == 0 && u1 == 1)
                {
                    //Compute R = R + T
                    error = FullAdd(ref r, r, t);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == 1 && u1 == -1)
                {
                    //Compute R = R + SmT
                    error = FullAdd(ref r, r, smt);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == 1 && u1 == 0)
                {
                    //Compute R = R + S
                    error = FullAdd(ref r, r, s);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else if (u0 == 1 && u1 == 1)
                {
                    //Compute R = R + SpT
                    error = FullAdd(ref r, r, spt);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
            }

            //Return status code
            return error;
        }

        public CryptoError GenerateSignature(Mpi privateKey, byte[] digest, ref EcdsaSignature signature)
        {
            CryptoError error;
            int n;

            Mpi k = new Mpi();
            Mpi z = new Mpi();
            EcPoint r1 = new EcPoint();

            //Check parameters
            if (domainParameters == null || privateKey == null || digest == null || signature == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            //Let N be the bit length of q
            n = domainParameters.q.GetBitLength();

            //Generated a pseudorandom number
            error = k.Random(n);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Make sure that 0 < k < q
            if (Mpi.Compare(k, domainParameters.q) >= 0)
            {
                error = k.ShiftRight(1);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            n = n > digest.Length * 8 ? digest.Length * 8 : n;

            error = z.Import(digest, 0, (n + 7) / 8, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Keep the leftmost N bits of the hash value
            if ((n % 8) != 0)
            {
                error = z.ShiftRight(8 - (n % 8));
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            //Compute R1 = (x1, y1) = k.G
            error = Multiply(ref r1, k, domainParameters.g);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = Affinify(ref r1, r1);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute r = x1 mod q
            error = signature.r.Mod(r1.x, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute k ^ -1 mod q
            error = k.InverseMod(k, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute s = k ^ -1 * (z + x * r) mod q
            error = signature.s.Multiply(privateKey, signature.r);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = signature.s.Add(signature.s, z);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = signature.s.Mod(signature.s, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = signature.s.MultiplyMod(signature.s, k, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Return status code
            return CryptoError.NO_ERROR;
        }

        public CryptoError VerifySignature(EcPoint publicKey, byte[] digest, EcdsaSignature signature)
        {
            CryptoError error;
            int n;

            Console.WriteLine("Public Key:");
            Console.WriteLine(publicKey.Dump());
            Console.WriteLine("Digest:");
            Console.WriteLine(digest.DumpArray());
            Console.WriteLine("Signature:");
            Console.WriteLine(signature.Dump());

            //Check parameters
            if (domainParameters == null || publicKey == null || digest == null || signature == null)
                return CryptoError.ERROR_INVALID_PARAMETER;

            //The verifier shall check that 0 < r < q
            if (Mpi.CompareInt(signature.r, 0) <= 0 || Mpi.Compare(signature.r, domainParameters.q) >= 0)
                return CryptoError.ERROR_INVALID_SIGNATURE;

            //The verifier shall check that 0 < s < q
            if (Mpi.CompareInt(signature.s, 0) <= 0 || Mpi.Compare(signature.s, domainParameters.q) >= 0)
                return CryptoError.ERROR_INVALID_SIGNATURE;

            Mpi w = new Mpi();
            Mpi z = new Mpi();
            Mpi u1 = new Mpi();
            Mpi u2 = new Mpi();
            Mpi v = new Mpi();
            EcPoint v0 = new EcPoint();
            EcPoint v1 = new EcPoint();

            //Let N be the bit length of q
            n = domainParameters.q.GetBitLength();

            //Compute N = MIN(N, outlen)
            n = n > digest.Length * 8 ? digest.Length * 8 : n;

            //Convert the digest to a multiple precision integer
            error = z.Import(digest, 0, (n + 7) / 8, MpiFormat.MPI_FORMAT_BIG_ENDIAN);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Keep the leftmost N bits of the hash value
            if ((n % 8) != 0)
            {
                error = z.ShiftRight(8 - (n % 8));
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            //Compute w = s ^ -1 mod q
            error = w.InverseMod(signature.s, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute u1 = z * w mod q
            error = u1.MultiplyMod(z, w, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute u2 = r * w mod q
            error = u2.MultiplyMod(signature.r, w, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute V0 = (x0, y0) = u1.G + u2.Q
            error = Projectify(ref v1, publicKey);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = TwinMultiply(ref v0, u1, domainParameters.g, u2, v1);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = Affinify(ref v0, v0);
            if (error != CryptoError.NO_ERROR)
                return error;

            //Compute v = x0 mod q
            error = v.Mod(v0.x, domainParameters.q);
            if (error != CryptoError.NO_ERROR)
                return error;

            //If v = r, then the signature is verified. If v does not equal r,
            //then the message or the signature may have been modified
            if (Mpi.Compare(v, signature.r) == 0)
                error = CryptoError.NO_ERROR;
            else
                error = CryptoError.ERROR_INVALID_SIGNATURE;

            //Return status code
            return error;
        }

    }
}