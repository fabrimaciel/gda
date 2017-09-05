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
	/// Classe usada para ordenar a lista.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GDAComparer<T> : IComparer<T>
	{
		private string m_SortColumn;

		private bool m_Reverse;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sortExpression">Expressão de ordenação que a GridView passa como parametro.</param>
		public GDAComparer(string sortExpression)
		{
			m_Reverse = sortExpression.ToLower().EndsWith(" desc");
			if(m_Reverse)
			{
				m_SortColumn = sortExpression.Substring(0, sortExpression.Length - 5);
			}
			else
			{
				m_SortColumn = sortExpression;
			}
		}

		public int Compare(T a, T b)
		{
			int retVal;
			Type type = typeof(T);
			string s1, s2;
			s1 = type.GetProperty(m_SortColumn).GetValue(a, null).ToString();
			s2 = type.GetProperty(m_SortColumn).GetValue(b, null).ToString();
			retVal = string.Compare(s1, s2, StringComparison.CurrentCulture);
			return (retVal * (m_Reverse ? -1 : 1));
		}
	}
}
