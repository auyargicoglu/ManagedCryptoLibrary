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

internal static class Extensions
{
    #region Array Template Functions

    public static void Zeroize(this byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = 0;
    }

    public static void Zeroize(this UInt32[] array)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = 0;
    }

    public static void Zeroize(this UInt64[] array)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = 0;
    }

    public static byte[] Grow(this byte[] array, int newSize)
    {
        if (array.Length >= newSize)
            return array;

        byte[] result = new byte[newSize];
        for (int i = 0; i < result.Length; i++)
            result[i] = 0;

        result.CopyIn(0, array, 0, array.Length);
        return result;
    }

    public static byte xor(this byte lhs, byte rhs)
    {
        return (byte)((int)lhs ^ (int)rhs);
    }

    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }

    public static bool IsEqual(this byte[] array, byte[] otherArray)
    {
        if ((otherArray == null) && (array == null))
            return true;

        if (array == null)
            return false;
        if (otherArray == null)
            return false;

        if (array.Length != otherArray.Length)
            return false;

        for (int i = 0; i < array.Length; i++)
            if (array[i] != otherArray[i])
                return false;

        return true;
    }

    public static void CopyIn<T>(this T[] array, int destinationOffset, T[] otherArray, int sourceOffset, int length)
    {
        for (int i = 0; i < length; i++, destinationOffset++, sourceOffset++)
            array[destinationOffset] = otherArray[sourceOffset];
    }

    public static T[] AddItem<T>(this T[] array, T item)
    {
        T[] result;

        if (array == null)
        {
            result = new T[1];
            result[0] = item;
            return result;
        }

        result = new T[array.Length + 1];
        array.CopyTo(result, 0);
        result[array.Length] = item;
        return result;
    }

    public static T[] RemoveAt<T>(this T[] array, int index)
    {
        if (index < 0)
            return array;

        if (index >= array.Length)
            return array;

        if (array.Length <= 1)
            return null;

        T[] itemRemovedArray = new T[array.Length - 1];
        itemRemovedArray.CopyIn(0, array, 0, index);
        itemRemovedArray.CopyIn(index, array, index + 1, array.Length - (index + 1));

        return itemRemovedArray;
    }

    public static double[] AddAnotherArray(this double[] target, double[] otherArray)
    {
        for (int i = 0; i < target.Length; i++)
            target[i] += otherArray[i];

        return target;
    }

    public static double[] SubtractAnotherArray(this double[] target, double[] otherArray)
    {
        for (int i = 0; i < target.Length; i++)
            target[i] -= otherArray[i];

        return target;
    }

    #endregion

    #region Byte Reversing Functions

    public static uint Reverse(this uint value)
    {
        return ((value & 0xFF000000) >> 24) | ((value & 0x00FF0000) >> 8) | ((value & 0x0000FF00) << 8) | ((value & 0x000000FF) << 24);
    }

    public static UInt64 Reverse(this UInt64 value)
    {
        byte[] inputBytes = BitConverter.GetBytes(value);
        byte[] outputBytes = new byte[8];

        for (int i = 0, j = 7; i < 8; i++, j--)
            outputBytes[i] = inputBytes[j];

        return BitConverter.ToUInt64(outputBytes);
    }

    public static uint ReadUInt32(this byte[] array, int offset)
    {
        return ((uint)array[offset] | ((uint)array[offset + 1] << 8) | ((uint)array[offset + 2] << 16) | ((uint)array[offset + 3] << 24));
    }

    public static void WriteUInt32(this byte[] array, uint value, int offset)
    {
        array[offset] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset + 1] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset + 2] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset + 3] = (byte)value;
    }

    public static uint ReadAndReverseUInt32(this byte[] array, int offset)
    {
        return ((uint)array[offset+3] | ((uint)array[offset+2] << 8) | ((uint)array[offset+1] << 16) | ((uint)array[offset] << 24));
    }

    public static void ReverseAndWriteUInt32(this byte[] array, uint value, int offset)
    {
        array[offset+ 3] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset + 2] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset + 1] = (byte)(value & 0xFFU);
        value >>= 8;
        array[offset] = (byte)value;
    }

    #endregion

    #region Logging

    public static string DumpArray(this byte[] array, int maxLength = 1024000)
    {
        string retVal = "";
        string printSt = "";
        int i;
        int length = array.Length > maxLength ? maxLength : array.Length;

        for (i = 0; i < length; i++)
        {
            printSt = printSt + array[i].ToString("X2") + " ";
            if ((i % 32) == 31)
            {
                printSt += "\n";
                retVal += printSt;
                printSt = "";
            }
        }

        if ((i % 32) != 31)
        {
            printSt += "\n";
            retVal += printSt;
        }

        return retVal;
    }

    public static string DumpArray(this uint[] array, int maxLength = 1024000)
    {
        string retVal = "";
        string printSt = "";
        int i;
        int length = array.Length > maxLength ? maxLength : array.Length;

        for (i = 0; i < length; i++)
        {
            printSt = printSt + array[i].ToString("X8") + " ";
            if ((i % 8) == 7)
            {
                retVal += (printSt + "\n");
                printSt = "";
            }
        }

        if ((i % 8) != 7)
        {
            retVal += printSt;
            retVal += "\n";
        }

        return retVal;
    }

    public static string DumpArray(this UInt64[] array, int maxLength = 1024000)
    {
        string retVal = "";
        string printSt = "";
        int i;
        int length = array.Length > maxLength ? maxLength : array.Length;

        for (i = 0; i < length; i++)
        {
            printSt = printSt + array[i].ToString("X16") + " ";
            if ((i % 4) == 3)
            {
                retVal += (printSt + "\n");
                printSt = "";
            }
        }

        if ((i % 4) != 3)
        {
            retVal += printSt;
            retVal += "\n";
        }

        return retVal;
    }

    #endregion
}

