using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Collections
{
	public class ObjectPropertyComparer<T> : IComparer<T>
	{
		private string PropertyName;
		public ObjectPropertyComparer (string a)
		{
			PropertyName = a;
		}
		public int Compare (T a, T b)
		{
			Type c = typeof(T);
			object d = c.GetProperty (PropertyName).GetValue (a, null);
			object e = c.GetProperty (PropertyName).GetValue (b, null);
			if (d != null && e == null)
				return 1;
			if (d == null && e != null)
				return -1;
			if (d == null && e == null)
				return 0;
			return ((IComparable)d).CompareTo (e);
		}
	}
}
