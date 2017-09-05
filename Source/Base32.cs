/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace GDA.Helper
{
	/// <summary>
	/// Encodes and decodes 'Canonical' base32 format.
	/// </summary>
	public static class Base32
	{
		static string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

		private static int[] base32Lookup =  {
			0xFF,
			0xFF,
			0x1A,
			0x1B,
			0x1C,
			0x1D,
			0x1E,
			0x1F,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0x00,
			0x01,
			0x02,
			0x03,
			0x04,
			0x05,
			0x06,
			0x07,
			0x08,
			0x09,
			0x0A,
			0x0B,
			0x0C,
			0x0D,
			0x0E,
			0x0F,
			0x10,
			0x11,
			0x12,
			0x13,
			0x14,
			0x15,
			0x16,
			0x17,
			0x18,
			0x19,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0x00,
			0x01,
			0x02,
			0x03,
			0x04,
			0x05,
			0x06,
			0x07,
			0x08,
			0x09,
			0x0A,
			0x0B,
			0x0C,
			0x0D,
			0x0E,
			0x0F,
			0x10,
			0x11,
			0x12,
			0x13,
			0x14,
			0x15,
			0x16,
			0x17,
			0x18,
			0x19,
			0xFF,
			0xFF,
			0xFF,
			0xFF,
			0xFF
		};

		/// <summary>
		/// Codifica os bytes para a base 32.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string Encode(byte[] bytes)
		{
			return Encode(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Codifica os bytes para a base 32.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="loc"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public static string Encode(byte[] bytes, int loc, int len)
		{
			int total = loc + len;
			int index = 0, digit = 0;
			int currByte, nextByte;
			StringBuilder base32 = new StringBuilder((len + 7) * 8 / 5);
			while (loc < total)
			{
				currByte = bytes[loc];
				if(index > 3)
				{
					if((loc + 1) < total)
						nextByte = bytes[loc + 1];
					else
						nextByte = 0;
					digit = currByte & (0xFF >> index);
					index = (index + 5) % 8;
					digit <<= index;
					digit |= nextByte >> (8 - index);
					loc++;
				}
				else
				{
					digit = (currByte >> (8 - (index + 5))) & 0x1F;
					index = (index + 5) % 8;
					if(index == 0)
						loc++;
				}
				base32.Append(base32Chars[digit]);
			}
			return base32.ToString().Replace('I', '1');
		}

		public static string Encode(string text)
		{
			return Encode(System.Text.Encoding.Default.GetBytes(text));
		}

		public static byte[] Decode(string base32)
		{
			return Decode(base32, 0, base32.Length);
		}

		public static byte[] Decode(string base32, int loc, int len)
		{
			base32 = base32.Replace('1', 'I');
			int total = loc + len;
			int index, lookup, offset, digit;
			byte[] bytes = new byte[len * 5 / 8];
			for(index = 0, offset = 0; loc < total; loc++)
			{
				lookup = base32[loc] - '0';
				if(lookup < 0 || lookup >= base32Lookup.Length)
					continue;
				digit = base32Lookup[lookup];
				if(digit == 0xFF)
					continue;
				if(index <= 3)
				{
					index = (index + 5) % 8;
					if(index == 0)
					{
						bytes[offset] |= (byte)digit;
						offset++;
						if(offset >= bytes.Length)
							break;
					}
					else
						bytes[offset] |= (byte)(digit << (8 - index));
				}
				else
				{
					index = (index + 5) % 8;
					bytes[offset] |= (byte)((uint)digit >> index);
					offset++;
					if(offset >= bytes.Length)
						break;
					bytes[offset] |= (byte)(digit << (8 - index));
				}
			}
			return bytes;
		}

		public static string DecodeToText(string base32)
		{
			byte[] result = Decode(base32);
			return System.Text.Encoding.Default.GetString(result, 0, result.Length);
		}
	}
}
