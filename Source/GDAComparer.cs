using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Helper
{
	public class GDAComparer<T> : IComparer<T>
	{
		private string m_SortColumn;
		private bool m_Reverse;
		public GDAComparer (string a)
		{
			m_Reverse = a.ToLower ().EndsWith (" desc");
			if (m_Reverse) {
				m_SortColumn = a.Substring (0, a.Length - 5);
			}
			else {
				m_SortColumn = a;
			}
		}
		public int Compare (T a, T b)
		{
			int c;
			Type d = typeof(T);
			string e, f;
			e = d.GetProperty (m_SortColumn).GetValue (a, null).ToString ();
			f = d.GetProperty (m_SortColumn).GetValue (b, null).ToString ();
			c = string.Compare (e, f, StringComparison.CurrentCulture);
			return (c * (m_Reverse ? -1 : 1));
		}
	}
}
