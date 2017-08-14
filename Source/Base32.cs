using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Helper
{
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
		public static string Encode (byte[] a)
		{
			return Encode (a, 0, a.Length);
		}
		public static string Encode (byte[] a, int b, int c)
		{
			int d = b + c;
			int e = 0, f = 0;
			int g, h;
			StringBuilder i = new StringBuilder ((c + 7) * 8 / 5);
			while (b < d) {
				g = a [b];
				if (e > 3) {
					if ((b + 1) < d)
						h = a [b + 1];
					else
						h = 0;
					f = g & (0xFF >> e);
					e = (e + 5) % 8;
					f <<= e;
					f |= h >> (8 - e);
					b++;
				}
				else {
					f = (g >> (8 - (e + 5))) & 0x1F;
					e = (e + 5) % 8;
					if (e == 0)
						b++;
				}
				i.Append (base32Chars [f]);
			}
			return i.ToString ().Replace ('I', '1');
		}
		public static string Encode (string a)
		{
			return Encode (System.Text.Encoding.Default.GetBytes (a));
		}
		public static byte[] Decode (string a)
		{
			return Decode (a, 0, a.Length);
		}
		public static byte[] Decode (string a, int b, int c)
		{
			a = a.Replace ('1', 'I');
			int d = b + c;
			int e, f, g, h;
			byte[] i = new byte[c * 5 / 8];
			for (e = 0, g = 0; b < d; b++) {
				f = a [b] - '0';
				if (f < 0 || f >= base32Lookup.Length)
					continue;
				h = base32Lookup [f];
				if (h == 0xFF)
					continue;
				if (e <= 3) {
					e = (e + 5) % 8;
					if (e == 0) {
						i [g] |= (byte)h;
						g++;
						if (g >= i.Length)
							break;
					}
					else
						i [g] |= (byte)(h << (8 - e));
				}
				else {
					e = (e + 5) % 8;
					i [g] |= (byte)((uint)h >> e);
					g++;
					if (g >= i.Length)
						break;
					i [g] |= (byte)(h << (8 - e));
				}
			}
			return i;
		}
		public static string DecodeToText (string a)
		{
			byte[] b = Decode (a);
			return System.Text.Encoding.Default.GetString (b, 0, b.Length);
		}
	}
}
