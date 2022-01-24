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
    public enum MpiFormat
    {
        MPI_FORMAT_LITTLE_ENDIAN = 0,
        MPI_FORMAT_BIG_ENDIAN = 1
    }

    public class Mpi
    {
        private static Random rng = null;

        public static void SetRandomNumberGenerator(Random rngIn)
        {
            rng = rngIn;
        }

        public int sign = 1;

        public int size = 0;

        public UInt32[] data = null;

        public const int MPI_INT_SIZE = sizeof(UInt32);

        public Mpi()
        {
            this.sign = 1;
            this.size = 0;
            this.data = null;
        }

        public Mpi(int value)
        {
            SetValue(value);
        }

        public Mpi Clone()
        {
            Mpi retValue = new Mpi();

            retValue.size = this.size;
            retValue.sign = this.sign;
            retValue.data = this.data.Clone() as UInt32[];

            return retValue;
        }

        public bool IsEven()
        {
            return !GetBitValue(0);
        }

        public bool IsOdd()
        {
            return GetBitValue(0);
        }

        public void Init()
        {
            this.sign = 1;
            this.size = 0;
            this.data = null;
        }

        public void Free()
        {
            this.sign = 1;
            this.size = 0;
            this.data = null;
        }

        public CryptoError Grow(int sizeIn)
        {
            if (sizeIn <= 0)
                sizeIn = 1;

            if (this.size >= sizeIn)
                return CryptoError.NO_ERROR;

            uint[] newData = new uint[sizeIn];
            newData.Zeroize();

            if (this.size > 0)
                newData.CopyIn(0, this.data, 0, (int)this.size);

            this.size = sizeIn;
            this.data = newData;

            return CryptoError.NO_ERROR;
        }

        public int GetLength()
        {
            int i;

            if (this.size <= 0)
                return 0;

            for (i = this.size - 1; i >= 0; i--)
                if (this.data[i] != 0)
                    break;

            return i + 1;
        }

        public int GetByteLength()
        {
            int n;
            UInt32 m;

            if (this.size <= 0)
                return 0;

            for (n = this.size - 1; n > 0; n--)
                if (this.data[n] != 0)
                    break;

            m = this.data[n];
            n *= MPI_INT_SIZE;

            for (; m != 0; m >>= 8)
                n++;

            return n;
        }

        public int GetBitLength()
        {
            int n;
            UInt32 m;

            if (this.size <= 0)
                return 0;

            for (n = this.size - 1; n > 0; n--)
                if (this.data[n] != 0)
                    break;

            m = this.data[n];
            n *= (MPI_INT_SIZE * 8);

            for (; m != 0; m >>= 1)
                n++;

            return n;
        }

        public CryptoError SetBitValue(int index, bool value)
        {
            CryptoError error;
            int n1;
            int n2;

            n1 = (int)index / (MPI_INT_SIZE * 8);
            n2 = (int)index % (MPI_INT_SIZE * 8);

            error = Grow(n1 + 1);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (value)
                this.data[n1] |= (1U << n2);
            else
                this.data[n1] &= ~(1U << n2);

            return CryptoError.NO_ERROR;
        }

        public bool GetBitValue(int index)
        {
            int n1;
            int n2;

            if (index < 0)
                return false;

            n1 = index / (MPI_INT_SIZE * 8);
            n2 = index % (MPI_INT_SIZE * 8);

            if (n1 >= this.size)
                return false;

            return (((this.data[n1] >> n2) & 0x01) == 1);
        }

        public CryptoError Copy(Mpi a)
        {
            int n;

            if (this == a)
                return CryptoError.NO_ERROR;

            n = a.GetLength();

            if (this.size < n)
            {
                this.data = new UInt32[n];
                this.data.CopyIn(0, a.data, 0, n);
                this.size = n;
            }
            else if (this.size > 0)
            {
                this.data.Zeroize();
                this.data.CopyIn(0, a.data, 0, n);
            }

            this.sign = a.sign;
            return CryptoError.NO_ERROR;
        }

        public CryptoError SetValue(int b)
        {
            UInt32 absValue = (b >= 0) ? (UInt32)b : (UInt32)(-b);

            if (this.size <= 0)
            {
                this.size = 1;
                this.data = new UInt32[1];
            }
            else
            {
                this.data.Zeroize();
            }

            this.sign = (b >= 0) ? 1 : -1;
            this.data[0] = absValue;

            return CryptoError.NO_ERROR;
        }

        public CryptoError Random(int length)
        {
            int n;
            int m;

            if (rng == null)
                return CryptoError.ERROR_RANDOM_NUMBER_GENERATOR_UNINITIALIZED;

            n = (length + (MPI_INT_SIZE * 8) - 1) / (MPI_INT_SIZE * 8);
            m = length % (MPI_INT_SIZE * 8);

            this.sign = 1;
            this.size = n;
            byte[] randomBytes = new byte[n * MPI_INT_SIZE];

            rng.NextBytes(randomBytes);
            this.data = new UInt32[n];

            //for (int i = 0; i < length; i++)
            //    this.data[i] = BitConverter.ToUInt32(randomBytes, i * 4);

            for (int i = 0; i < n; i++)
                this.data[i] = (uint)(i * i * i + i) + 0xA0BBC2EFU;

            if (n > 0 && m > 0)
                this.data[n - 1] &= (uint)(1 << m) - 1U;

            return CryptoError.NO_ERROR;
        }

        public CryptoError CheckProbablePrime()
        {
            return CryptoError.ERROR_NOT_IMPLEMENTED;
        }

        public CryptoError Import(byte[] data, int offset, int length, MpiFormat format)
        {
            if (data.Length < length + offset)
                return CryptoError.ERROR_INVALID_LENGTH;

            if (format == MpiFormat.MPI_FORMAT_LITTLE_ENDIAN)
                return ImportLittleEndian(data, offset, length);

            return ImportBigEndian(data, offset, length);
        }

        private CryptoError ImportLittleEndian(byte[] data, int offset, int length)
        {
            CryptoError error;
            int i;

            while (length > 0 && data[offset + length - 1] == 0)
                length--;

            this.sign = 1;
            error = Grow((length + MPI_INT_SIZE - 1) / MPI_INT_SIZE);

            if (error != CryptoError.NO_ERROR)
                return error;

            for (i = 0; i < length; i++)
                this.data[i / MPI_INT_SIZE] |= ((uint)data[i + offset] << ((i % MPI_INT_SIZE) * 8));

            return CryptoError.NO_ERROR;
        }

        private CryptoError ImportBigEndian(byte[] data, int offset, int length)
        {
            CryptoError error;
            int i = 0;
            int j;

            while (length > 1 && data[i + offset] == 0)
            {
                i++;
                length--;
            }

            this.sign = 1;
            error = Grow((length + MPI_INT_SIZE - 1) / MPI_INT_SIZE);

            if (error != CryptoError.NO_ERROR)
                return error;

            i += (length - 1);

            for (j = 0; j < length; j++, i--)
                this.data[j / MPI_INT_SIZE] |= ((uint)data[i + offset] << ((j % MPI_INT_SIZE) * 8));

            return CryptoError.NO_ERROR;
        }

        public CryptoError Export(ref byte[] data, int offset, int length, MpiFormat format)
        {
            if (data.Length < length + offset)
                return CryptoError.ERROR_INVALID_LENGTH;

            if (format == MpiFormat.MPI_FORMAT_LITTLE_ENDIAN)
                return ExportLittleEndian(ref data, offset, length);

            return ExportBigEndian(ref data, offset, length);
        }

        public CryptoError ExportLittleEndian(ref byte[] data, int offset, int length)
        {
            int i;
            int n;

            n = GetByteLength();
            if (n > length)
                return CryptoError.ERROR_INVALID_LENGTH;

            for (i = 0; i < n; i++)
                data[i + offset] = (byte)(this.data[i / MPI_INT_SIZE] >> ((i % MPI_INT_SIZE) * 8));

            return CryptoError.NO_ERROR;
        }

        public CryptoError ExportBigEndian(ref byte[] data, int offset, int length)
        {
            int i;
            int j;
            int n;

            n = GetByteLength();
            if (n > length)
                return CryptoError.ERROR_INVALID_LENGTH;

            i = n - 1;
            for (j = 0; j < n; j++, i--)
                data[i + offset] = (byte)(this.data[j / MPI_INT_SIZE] >> ((j % MPI_INT_SIZE) * 8));

            return CryptoError.NO_ERROR;
        }

        public CryptoError Add(Mpi a, Mpi b)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (a.sign == b.sign)
            {
                error = AddAbs(a, b);
                this.sign = a.sign;

                return error;
            }

            if (CompareAbs(a, b) >= 0)
            {
                error = SubtractAbs(a, b);
                this.sign = a.sign;

                return error;
            }

            error = SubtractAbs(b, a);
            this.sign = b.sign;

            return error;
        }

        public CryptoError AddInt(Mpi a, int b)
        {
            return Add(a, new Mpi(b));
        }

        public CryptoError Subtract(Mpi a, Mpi b)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (a.sign != b.sign)
            {
                error = AddAbs(a, b);
                this.sign = a.sign;

                return error;
            }

            if (CompareAbs(a, b) >= 0)
            {
                error = SubtractAbs(a, b);
                this.sign = a.sign;

                return error;
            }

            error = SubtractAbs(b, a);
            this.sign = -a.sign;

            return error;
        }

        public CryptoError SubtractInt(Mpi a, Int32 b)
        {
            return Subtract(a, new Mpi(b));
        }

        private CryptoError AddAbs(Mpi a, Mpi b)
        {
            int i;
            int n;
            UInt32 c;
            UInt32 d;

            Copy(a);
            n = b.GetLength();

            if (n > this.size)
                Grow(n);

            this.sign = 1;
            c = 0;

            for (i = 0; i < n; i++)
            {
                d = this.data[i] + c;
                if (d != 0)
                    c = 0;

                d += b.data[i];
                if (d < b.data[i])
                    c = 1;

                this.data[i] = d;
            }

            for (i = n; (c == 1) && (i < this.size); i++)
            {
                this.data[i] += c;

                if (this.data[i] != 0)
                    c = 0;
            }

            if (c == 1)
            {
                Grow(n + 1);
                this.data[n] = 1;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError SubtractAbs(Mpi a, Mpi b)
        {
            int i;
            int n;
            UInt32 c;
            UInt32 d;

            Copy(a);
            n = b.GetLength();

            this.sign = 1;
            c = 0;

            for (i = 0; i < n; i++)
            {
                d = this.data[i];

                if (c == 1)
                {
                    if (d != 0)
                    {
                        c = 0;
                        d--;
                    }
                    else
                    {
                        c = 1;
                        d = 0xFFFFFFFFU;
                    }
                }

                if (d < b.data[i])
                    c = 1;

                this.data[i] = d - b.data[i];
            }

            for (i = n; (c == 1) && (i < this.size); i++)
            {
                if (this.data[i] != 0)
                    c = 0;

                this.data[i]--;
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError ShiftLeft(int n)
        {
            CryptoError error;
            int i;
            int n1;
            int n2;

            if (this.size == 0)
                return CryptoError.NO_ERROR;

            if (n == 0)
                return CryptoError.NO_ERROR;

            n1 = n / (MPI_INT_SIZE * 8);
            n2 = n % (MPI_INT_SIZE * 8);

            error = Grow(this.size + (n + 31) / 32);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (n1 > 0)
            {
                for (i = this.size - 1; i >= n1; i--)
                    this.data[i] = this.data[i - n1];

                for (i = 0; i < n1; i++)
                    this.data[i] = 0;
            }

            if (n2 > 0)
            {
                for (i = this.size - 1; i >= 1; i--)
                    this.data[i] = (this.data[i] << n2) | (this.data[i - 1] >> (32 - n2));

                this.data[0] <<= n2;
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError ShiftRight(int n)
        {
            int i;
            int m;
            int n1;
            int n2;

            if (this.size == 0)
                return CryptoError.NO_ERROR;

            n1 = n / (MPI_INT_SIZE * 8);
            n2 = n % (MPI_INT_SIZE * 8);

            if (n1 >= this.size)
            {
                this.data.Zeroize();
                return CryptoError.NO_ERROR;
            }

            if (n1 > 0)
            {
                for (m = this.size - n1, i = 0; i < m; i++)
                    this.data[i] = this.data[i + n1];

                for (i = m; i < this.size; i++)
                    this.data[i] = 0;
            }

            if (n2 > 0)
            {
                for (m = this.size - n1 - 1, i = 0; i < m; i++)
                    this.data[i] = (this.data[i] >> n2) | (this.data[i + 1] << (32 - n2));

                this.data[m] >>= n2;
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError Multiply(Mpi a, Mpi b)
        {
            CryptoError error;
            int i;
            int m;
            int n;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            m = a.GetLength();
            n = b.GetLength();

            error = Grow(m + n);
            if (error != CryptoError.NO_ERROR)
                return error;

            this.sign = (a.sign == b.sign) ? 1 : -1;
            this.data.Zeroize();

            if (m < n)
            {
                for (i = 0; i < m; i++)
                    MultiplySingleInteger(ref this.data, i, ref b.data, n, a.data[i]);
            }
            else
            {
                for (i = 0; i < n; i++)
                    MultiplySingleInteger(ref this.data, i, ref a.data, m, b.data[i]);
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError MultiplyInt(Mpi a, Int32 b)
        {
            return Multiply(a, new Mpi(b));
        }

        public CryptoError Mod(Mpi a, Mpi p)
        {
            CryptoError error;
            int signTemp;
            int m;
            int n;

            if (this == a)
                a = this.Clone();

            if (this == p)
                p = this.Clone();

            Mpi c = new Mpi();

            if (CompareInt(p, 0) <= 0)
                return CryptoError.ERROR_INVALID_PARAMETER;

            signTemp = a.sign;
            m = a.GetBitLength();
            n = p.GetBitLength();

            error = Copy(a);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (m >= n)
            {
                error = c.Copy(p);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = c.ShiftLeft(m - n);
                if (error != CryptoError.NO_ERROR)
                    return error;

                while (CompareAbs(this, p) >= 0)
                {
                    if (CompareAbs(this, c) >= 0)
                    {
                        error = SubtractAbs(this, c);
                        if (error != CryptoError.NO_ERROR)
                            return error;
                    }

                    error = c.ShiftRight(1);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
            }

            if (signTemp < 0)
                SubtractAbs(p, this);

            return CryptoError.NO_ERROR;
        }

        public CryptoError AddMod(Mpi a, Mpi b, Mpi p)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (this == p)
                p = this.Clone();

            error = Add(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            return Mod(this, p);
        }

        public CryptoError SubMod(Mpi a, Mpi b, Mpi p)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (this == p)
                p = this.Clone();

            error = Subtract(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            return Mod(this, p);
        }

        public CryptoError MultiplyMod(Mpi a, Mpi b, Mpi p)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (this == p)
                p = this.Clone();

            error = Multiply(a, b);
            if (error != CryptoError.NO_ERROR)
                return error;

            return Mod(this, p);
        }

        public CryptoError InverseMod(Mpi a, Mpi p)
        {
            CryptoError error;

            if (this == a)
                a = this.Clone();

            if (this == p)
                p = this.Clone();

            Mpi b = new Mpi();
            Mpi c = new Mpi();
            Mpi q0 = new Mpi();
            Mpi r0 = new Mpi();
            Mpi t = new Mpi();
            Mpi u = new Mpi();
            Mpi v = new Mpi();

            error = b.Copy(p);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = c.Copy(a);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = u.SetValue(0);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = v.SetValue(1);
            if (error != CryptoError.NO_ERROR)
                return error;

            while (CompareInt(c, 0) > 0)
            {
                error = Divide(ref q0, ref r0, b, c);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = b.Copy(c);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = c.Copy(r0);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = t.Copy(v);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = q0.Multiply(q0, v);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = v.Subtract(u, q0);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = u.Copy(t);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            if (CompareInt(b, 1) != 0)
                return CryptoError.ERROR_FAILURE;

            if (CompareInt(u, 0) > 0)
            {
                error = Copy(u);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }
            else
            {
                error = Add(u, p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        public CryptoError ExpMod(Mpi a, Mpi e, Mpi p)
        {
            CryptoError error;
            int i;
            int j;
            int n;
            int d;
            int k;
            int u;

            if (this == a)
                a = this.Clone();

            if (this == e)
                e = this.Clone();

            if (this == p)
                p = this.Clone();

            Mpi b = new Mpi();
            Mpi c2 = new Mpi();
            Mpi t = new Mpi();

            Mpi[] s = new Mpi[8];
            for (i = 0; i < 8; i++)
                s[i] = new Mpi();

            d = e.GetBitLength() <= 32 ? 1 : 4;

            if (p.IsEven())
            {
                error = b.MultiplyMod(a, a, p);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = s[0].Copy(a);
                if (error != CryptoError.NO_ERROR)
                    return error;

                for (i = 1; i < (1 << (d - 1)); i++)
                {
                    error = s[i].MultiplyMod(s[i - 1], b, p);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                i = e.GetBitLength() - 1;

                while (i >= 0)
                {
                    if (e.GetBitValue(i) == false)
                    {
                        error = MultiplyMod(this, this, p);
                        if (error != CryptoError.NO_ERROR)
                            return error;

                        i--;
                    }
                    else
                    {
                        n = i - d + 1;
                        if (n < 0)
                            n = 0;

                        while (e.GetBitValue(n) == false)
                            n++;

                        for (u = 0, j = i; j >= n; j--)
                        {
                            error = MultiplyMod(this, this, p);
                            if (error != CryptoError.NO_ERROR)
                                return error;

                            if (e.GetBitValue(j))
                                u = (u << 1) | 1;
                            else
                                u <<= 1;
                        }

                        error = MultiplyMod(this, s[u >> 1], p);
                        if (error != CryptoError.NO_ERROR)
                            return error;

                        i = n - 1;
                    }
                }
            }
            else
            {
                k = p.GetLength();

                error = c2.SetValue(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = c2.ShiftLeft(2 * k * (MPI_INT_SIZE * 8));
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = c2.Mod(c2, p);
                if (error != CryptoError.NO_ERROR)
                    return error;

                if (Compare(a, p) >= 0)
                {
                    error = b.Mod(a, p);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = b.MontgomeryMultiply(b, c2, k, p, ref t);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }
                else
                {
                    error = b.MontgomeryMultiply(a, c2, k, p, ref t);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = MontgomeryMultiply(b, b, k, p, ref t);
                if (error != CryptoError.NO_ERROR)
                    return error;

                error = s[0].Copy(b);
                if (error != CryptoError.NO_ERROR)
                    return error;

                for (i = 1; i < (1 << (d - 1)); i++)
                {
                    s[i].MontgomeryMultiply(s[i - 1], this, k, p, ref t);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = Copy(c2);
                if (error != CryptoError.NO_ERROR)
                    return error;

                MontgomeryReduction(this, k, p, ref t);
                if (error != CryptoError.NO_ERROR)
                    return error;

                i = e.GetBitLength() - 1;

                while (i >= 0)
                {
                    if (e.GetBitValue(i) == false)
                    {
                        MontgomeryMultiply(this, this, k, p, ref t);
                        if (error != CryptoError.NO_ERROR)
                            return error;

                        i--;
                    }
                    else
                    {
                        n = i - d + 1;
                        if (n < 0)
                            n = 0;

                        while (e.GetBitValue(n) == false)
                            n++;

                        for (u = 0, j = i; j >= n; j--)
                        {
                            MontgomeryMultiply(this, this, k, p, ref t);
                            if (error != CryptoError.NO_ERROR)
                                return error;

                            if (e.GetBitValue(j))
                                u = (u << 1) | 1;
                            else
                                u <<= 1;
                        }

                        MontgomeryMultiply(this, s[u >> 1], k, p, ref t);
                        if (error != CryptoError.NO_ERROR)
                            return error;

                        i = n - 1;
                    }
                }

                MontgomeryReduction(this, k, p, ref t);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        public static int Compare(Mpi a, Mpi b)
        {
            int m;
            int n;

            m = a.GetLength();
            n = b.GetLength();

            if ((m == 0) && (n == 0))
                return 0;

            if (m > n)
                return a.sign;

            if (m < n)
                return -b.sign;

            if (a.sign > 0 && b.sign < 0)
                return 1;

            if (a.sign < 0 && b.sign > 0)
                return -1;

            while (n > 0)
            {
                n--;

                if (a.data[n] > b.data[n])
                    return a.sign;

                if (a.data[n] < b.data[n])
                    return -a.sign;
            }

            return 0;
        }

        public static int CompareInt(Mpi a, int b)
        {
            return Compare(a, new Mpi(b));
        }

        public static int CompareAbs(Mpi a, Mpi b)
        {
            int m;
            int n;

            m = a.GetLength();
            n = b.GetLength();

            if ((m == 0) && (n == 0))
                return 0;

            if (m > n)
                return 1;

            if (m < n)
                return -1;

            while (n > 0)
            {
                n--;

                if (a.data[n] > b.data[n])
                    return 1;

                if (a.data[n] < b.data[n])
                    return -1;
            }

            return 0;
        }

        public static CryptoError Divide(ref Mpi q, ref Mpi r, Mpi a, Mpi b)
        {
            CryptoError error;
            int m;
            int n;

            Mpi c = new Mpi();
            Mpi d = new Mpi();
            Mpi e = new Mpi();

            if (CompareInt(b, 0) == 0)
                return CryptoError.ERROR_DIVISION_BY_ZERO;

            error = c.Copy(a);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = d.Copy(b);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = e.SetValue(0);
            if (error != CryptoError.NO_ERROR)
                return error;

            m = c.GetBitLength();
            n = d.GetBitLength();

            if (m > n)
            {
                error = d.ShiftLeft(m - n);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            while (n++ <= m)
            {
                error = e.ShiftLeft(1);
                if (error != CryptoError.NO_ERROR)
                    return error;

                if (Compare(c, d) >= 0)
                {
                    error = e.SetBitValue(0, true);
                    if (error != CryptoError.NO_ERROR)
                        return error;

                    error = c.Subtract(c, d);
                    if (error != CryptoError.NO_ERROR)
                        return error;
                }

                error = d.ShiftRight(1);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            if (q != null)
                q.Copy(e);

            if (r != null)
                r.Copy(c);

            return CryptoError.NO_ERROR;
        }

        public static CryptoError DivideInt(ref Mpi q, ref Mpi r, Mpi a, Int32 b)
        {
            return Divide(ref q, ref r, a, new Mpi(b));
        }

        private CryptoError MontgomeryMultiply(Mpi a, Mpi b, int k, Mpi p, ref Mpi t)
        {
            CryptoError error;
            int i;
            UInt32 m;
            int n;
            UInt32 q;

            if (this == a)
                a = this.Clone();

            if (this == b)
                b = this.Clone();

            if (this == p)
                p = this.Clone();

            if (this == t)
                t = this.Clone();

            for (m = 2 - p.data[0], i = 0; i < 4; i++)
                m = m * (2 - m * p.data[0]);

            m = ~m + 1;

            if (b.size > k)
                n = k;
            else
                n = b.size;

            error = t.Grow(2 * k + 1);
            if (error != CryptoError.NO_ERROR)
                return error;

            error = t.SetValue(0);
            if (error != CryptoError.NO_ERROR)
                return error;

            for (i = 0; i < k; i++)
            {
                if (i < a.size)
                {
                    q = (t.data[i] + a.data[i] * b.data[0]) * m;
                    MultiplySingleInteger(ref t.data, i, ref b.data, n, a.data[i]);
                }
                else
                {
                    q = t.data[i] * m;
                }

                MultiplySingleInteger(ref t.data, i, ref p.data, k, q);
            }

            error = t.ShiftRight(k * (MPI_INT_SIZE * 8));
            if (error != CryptoError.NO_ERROR)
                return error;

            error = Copy(t);
            if (error != CryptoError.NO_ERROR)
                return error;

            if (Compare(this, p) >= 0)
            {
                error = Subtract(this, p);
                if (error != CryptoError.NO_ERROR)
                    return error;
            }

            return CryptoError.NO_ERROR;
        }

        private CryptoError MontgomeryReduction(Mpi a, int k, Mpi p, ref Mpi t)
        {
            return MontgomeryMultiply(a, new Mpi(1), k, p, ref t);
        }

        private static void MultiplySingleInteger(ref UInt32[] r, int rIndex, ref UInt32[] a, int m, UInt32 b)
        {
            int i;
            UInt32 c;
            UInt32 u;
            UInt32 v;
            UInt64 p;

            if (r == a)
                a = r.Clone() as UInt32[];

            c = 0;
            u = 0;
            v = 0;

            for (i = 0; i < m; i++)
            {
                p = (UInt64)a[i] * (UInt64)b;
                u = (UInt32)p;
                v = (UInt32)(p >> 32);

                u += c;
                if (u < c)
                    v++;

                u += r[i + rIndex];
                if (u < r[i + rIndex])
                    v++;

                r[i + rIndex] = u;
                c = v;
            }

            for (; c != 0; i++)
            {
                r[i + rIndex] += c;
                c = r[i + rIndex] < c ? 1U : 0U;
            }
        }

        public string Dump()
        {
            string retVal = "Sign:" + this.sign.ToString() + "\n";

            retVal += ("Size:" + this.size.ToString() + "\n");

            if (this.data != null)
                retVal += ("Data:\n" + this.data.DumpArray() + "\n");
            else
                retVal += "Data:\nnull\n";

            return retVal;
        }
    }
}

