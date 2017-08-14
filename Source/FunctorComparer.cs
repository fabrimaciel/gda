using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Collections
{
	internal sealed class FunctorComparer<T> : IComparer<T>
	{
		private Comparer<T> c;
		private Comparison<T> comparison;
		public FunctorComparer (Comparison<T> a)
		{
			this.c = Comparer<T>.Default;
			this.comparison = a;
		}
		public int Compare (T a, T b)
		{
			return this.comparison (a, b);
		}
	}
}
