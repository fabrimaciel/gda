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

namespace GDA.Collections
{
	/// <summary>
	/// Comparador de propriedades de object.
	/// </summary>
	public class ObjectPropertyComparer<T> : IComparer<T>
	{
		private string PropertyName;

		/// <summary>
		/// Provides Comparison opreations.
		/// </summary>
		/// <param name="propertyName">The property to compare</param>
		public ObjectPropertyComparer(string propertyName)
		{
			PropertyName = propertyName;
		}

		/// <summary>
		/// Compares 2 objects by their properties, given on the constructor
		/// </summary>
		/// <param name="x">First value to compare</param>
		/// <param name="y">Second value to compare</param>
		/// <returns></returns>
		public int Compare(T x, T y)
		{
			Type type = typeof(T);
			object a = type.GetProperty(PropertyName).GetValue(x, null);
			object b = type.GetProperty(PropertyName).GetValue(y, null);
			if(a != null && b == null)
				return 1;
			if(a == null && b != null)
				return -1;
			if(a == null && b == null)
				return 0;
			return ((IComparable)a).CompareTo(b);
		}
	}
}
