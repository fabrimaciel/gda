using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	static class ArrayHelper
	{
		public static bool Exists<T> (T[] a, Predicate<T> b)
		{
			return (FindIndex<T> (a, b) != -1);
		}
		public static int FindIndex<T> (T[] a, Predicate<T> b)
		{
			if (a == null)
				throw new ArgumentNullException ("array");
			return FindIndex<T> (a, 0, a.Length, b);
		}
		public static int FindIndex<T> (T[] a, int b, Predicate<T> c)
		{
			if (a == null)
				throw new ArgumentNullException ("array");
			return FindIndex<T> (a, b, a.Length - b, c);
		}
		public static int FindIndex<T> (T[] a, int b, int c, Predicate<T> d)
		{
			if (a == null)
				throw new ArgumentNullException ("array");
			if ((b < 0) || (b > a.Length))
				throw new ArgumentOutOfRangeException ("startIndex", "ArgumentOutOfRange Index");
			if ((c < 0) || (b > (a.Length - c)))
				throw new ArgumentOutOfRangeException ("count", "ArgumentOutOfRange Count");
			if (d == null)
				throw new ArgumentNullException ("match");
			int e = b + c;
			for (int f = b; f < e; f++) {
				if (d (a [f])) {
					return f;
				}
			}
			return -1;
		}
	}
}
