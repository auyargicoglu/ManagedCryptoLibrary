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

//Dependencies

using System;

namespace ManagedCryptoLibrary
{
	public enum CryptoError
	{
		NO_ERROR = 0,
		ERROR_RANDOM_NUMBER_GENERATOR_UNINITIALIZED,
		ERROR_DOMAIN_PARAMETERS_UNINITIALIZED,
		ERROR_NOT_IMPLEMENTED,
		ERROR_INVALID_LENGTH,
		ERROR_DIVISION_BY_ZERO,
		ERROR_INVALID_PARAMETER,
		ERROR_FAILURE,
		ERROR_ILLEGAL_PARAMETER,
		ERROR_OUT_BUFFER_INSUFFICIENT,
		ERROR_INVALID_INPUT_BUFFER,
		ERROR_INVALID_SIGNATURE,
	}

	public static class CryptoCommon
	{
		/**
		* @brief Reverse the byte order of a 16-bit word
		* @param[in] value 16-bit value
		* @return 16-bit value with byte order swapped
		**/
		public static UInt16 swapInt16(UInt16 valueUShort)
		{
			int value = valueUShort;

			value = ((value & 0xFF00) >> 8) | ((value & 0x00FF) << 8);

			return (ushort)value;
		}


		/**
		 * @brief Reverse the byte order of a 32-bit word
		 * @param[in] value 32-bit value
		 * @return 32-bit value with byte order swapped
		 **/

		public static UInt32 swapInt32(UInt32 value)
		{
			return ((value & 0xFF000000) >> 24) | ((value & 0x00FF0000) >> 8) | ((value & 0x0000FF00) << 8) | ((value & 0x000000FF) << 24);
		}


		/**
		 * @brief Reverse the byte order of a 64-bit word
		 * @param[in] value 64-bit value
		 * @return 64-bit value with byte order swapped
		 **/

		public static UInt64 swapInt64(UInt64 value)
		{
			byte[] inputBytes = BitConverter.GetBytes(value);
			byte[] outputBytes = new byte[8];

			for (int i = 0, j = 7; i < 8; i++, j--)
				outputBytes[i] = inputBytes[j];

			return BitConverter.ToUInt64(outputBytes);
		}


		/**
		 * @brief Reverse bit order in a 4-bit word
		 * @param[in] value 4-bit value
		 * @return 4-bit value with bit order reversed
		 **/

		public static byte reverseInt4(byte valueByte)
		{
			int value = valueByte;

			value = ((value & 0x0C) >> 2) | ((value & 0x03) << 2);
			value = ((value & 0x0A) >> 1) | ((value & 0x05) << 1);

			return (byte)value;
		}


		/**
		 * @brief Reverse bit order in a byte
		 * @param[in] value 8-bit value
		 * @return 8-bit value with bit order reversed
		 **/

		public static byte reverseInt8(byte valueByte)
		{
			int value = valueByte;

			value = ((value & 0xF0) >> 4) | ((value & 0x0F) << 4);
			value = ((value & 0xCC) >> 2) | ((value & 0x33) << 2);
			value = ((value & 0xAA) >> 1) | ((value & 0x55) << 1);

			return (byte)value;
		}


		/**
		 * @brief Reverse bit order in a 16-bit word
		 * @param[in] value 16-bit value
		 * @return 16-bit value with bit order reversed
		 **/

		public static UInt16 reverseInt16(UInt16 valueUShort)
		{
			int value = valueUShort;

			value = ((value & 0xFF00) >> 8) | ((value & 0x00FF) << 8);
			value = ((value & 0xF0F0) >> 4) | ((value & 0x0F0F) << 4);
			value = ((value & 0xCCCC) >> 2) | ((value & 0x3333) << 2);
			value = ((value & 0xAAAA) >> 1) | ((value & 0x5555) << 1);

			return (UInt16)value;
		}


		/**
		 * @brief Reverse bit order in a 32-bit word
		 * @param[in] value 32-bit value
		 * @return 32-bit value with bit order reversed
		 **/

		public static UInt32 reverseInt32(UInt32 value)
		{
			value = ((value & 0xFFFF0000U) >> 16) | ((value & 0x0000FFFFU) << 16);
			value = ((value & 0xFF00FF00U) >> 8) | ((value & 0x00FF00FFU) << 8);
			value = ((value & 0xF0F0F0F0U) >> 4) | ((value & 0x0F0F0F0FU) << 4);
			value = ((value & 0xCCCCCCCCU) >> 2) | ((value & 0x33333333U) << 2);
			value = ((value & 0xAAAAAAAAU) >> 1) | ((value & 0x55555555U) << 1);

			return value;
		}


		/**
		 * @brief Reverse bit order in a 64-bit word
		 * @param[in] value 64-bit value
		 * @return 64-bit value with bit order reversed
		 **/

		public static UInt64 reverseInt64(UInt64 value)
		{
			value = ((value & 0xFFFFFFFF00000000UL) >> 32) | ((value & 0x00000000FFFFFFFFUL) << 32);
			value = ((value & 0xFFFF0000FFFF0000UL) >> 16) | ((value & 0x0000FFFF0000FFFFUL) << 16);
			value = ((value & 0xFF00FF00FF00FF00UL) >> 8) | ((value & 0x00FF00FF00FF00FFUL) << 8);
			value = ((value & 0xF0F0F0F0F0F0F0F0UL) >> 4) | ((value & 0x0F0F0F0F0F0F0F0FUL) << 4);
			value = ((value & 0xCCCCCCCCCCCCCCCCUL) >> 2) | ((value & 0x3333333333333333UL) << 2);
			value = ((value & 0xAAAAAAAAAAAAAAAAUL) >> 1) | ((value & 0x5555555555555555UL) << 1);

			return value;
		}

		public static UInt32 ROL32(UInt32 a, int n)
		{
			return (((a) << (n)) | ((a) >> (32 - (n))));
		}

		public static UInt64 ROL64(UInt64 a, int n)
		{
			return (((a) << (n)) | ((a) >> (64 - (n))));
		}

		public static UInt32 ROR32(UInt32 a, int n)
		{
			return (((a) >> (n)) | ((a) << (32 - (n))));
		}

		public static UInt64 ROR64(UInt64 a, int n)
        {
			return (((a) >> (n)) | ((a) << (64 - (n))));
		}

		public static UInt32 SHL32(UInt32 a, int n)
		{
			return ((a) << (n));
		}

		public static UInt64 SHL64(UInt64 a, int n)
		{
			return ((a) << (n));
		}

		public static UInt32 SHR32(UInt32 a, int n)
		{
			return ((a) >> (n));
		}

		public static UInt64 SHR64(UInt64 a, int n)
		{
			return ((a) >> (n));
		}
	}
}