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

namespace GDA
{
	/// <summary>
	/// Informações sobre a paginação.
	/// </summary>
	public class InfoPaging
	{
		private int _startRow;

		private int _pageSize;

		private string _keyFieldName;

		/// <summary>
		/// Linha inicial da Página.
		/// </summary>
		public int StartRow
		{
			get
			{
				return _startRow;
			}
			set
			{
				_startRow = value;
			}
		}

		/// <summary>
		/// Tamanho do página.
		/// </summary>
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = value;
			}
		}

		/// <summary>
		/// Nome do campo chave que pode ser usado para auxiliar na paginação.
		/// </summary>
		public string KeyFieldName
		{
			get
			{
				return _keyFieldName;
			}
			set
			{
				_keyFieldName = value;
			}
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="startRow">Linha inicial.</param>
		/// <param name="pageSize">Tamanho da página.</param>
		public InfoPaging(int startRow, int pageSize)
		{
			_startRow = startRow;
			_pageSize = pageSize;
		}
	}
}
