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
	public class InfoSortExpression
	{
		private string _sortColumn;

		private bool _reverse;

		private string _aliasTable;

		private string _defaultFieldSort;

		/// <summary>
		/// Coluna a ser ordenada.
		/// </summary>
		public string SortColumn
		{
			get
			{
				return _sortColumn;
			}
			set
			{
				_sortColumn = value;
			}
		}

		/// <summary>
		/// Identifica a ordem da ordenação.
		/// </summary>
		public bool Reverse
		{
			get
			{
				return _reverse;
			}
			set
			{
				_reverse = value;
			}
		}

		/// <summary>
		/// Apelido da tabela na query sql.
		/// </summary>
		public string AliasTable
		{
			get
			{
				return _aliasTable;
			}
			set
			{
				_aliasTable = value;
			}
		}

		/// <summary>
		/// Campo que vem ordenado como padrão.
		/// </summary>
		public string DefaultFieldSort
		{
			get
			{
				return _defaultFieldSort;
			}
			set
			{
				_defaultFieldSort = value;
			}
		}

		public InfoSortExpression()
		{
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="sortExpression">Expressão de ordenação que a GridView passa como parametro.</param>
		public InfoSortExpression(string sortExpression)
		{
			if(sortExpression == null || sortExpression == "")
				return;
			_reverse = sortExpression.EndsWith(" desc", StringComparison.InvariantCultureIgnoreCase);
			if(_reverse)
				_sortColumn = sortExpression.Substring(0, sortExpression.Length - 5);
			else
			{
				if(sortExpression.EndsWith(" asc", StringComparison.InvariantCultureIgnoreCase))
					sortExpression = sortExpression.Substring(0, sortExpression.Length - " asc".Length);
				_sortColumn = sortExpression;
			}
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <param name="defaultFieldSort">Campo que vem ordenado por padrão.</param>
		public InfoSortExpression(string sortExpression, string defaultFieldSort) : this(sortExpression)
		{
			_defaultFieldSort = defaultFieldSort;
			if(sortExpression == null || sortExpression == "")
				sortExpression = defaultFieldSort;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <param name="defaultFieldSort">Campo que vem ordenado por padrão.</param>
		/// <param name="aliasTable">Apelido da tabela aonde o campo está sendo ordenado.</param>
		public InfoSortExpression(string sortExpression, string defaultFieldSort, string aliasTable) : this(sortExpression, defaultFieldSort)
		{
			_aliasTable = aliasTable;
		}
	}
}
