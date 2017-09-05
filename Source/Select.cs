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

namespace GDA.Sql.InterpreterExpression.Nodes
{
	/// <summary>
	/// Representa uma expressão que é um SELECT.
	/// </summary>
	class Select : SqlExpression
	{
		private SelectPart _selectPart;

		private FromPart _fromPart;

		private WherePart _wherePart;

		private GroupByPart _groupByPart;

		private HavingPart _havingPart;

		private OrderByPart _orderByPart;

		public SelectPart SelectPart
		{
			get
			{
				return _selectPart;
			}
			set
			{
				_selectPart = value;
			}
		}

		public FromPart FromPart
		{
			get
			{
				return _fromPart;
			}
			set
			{
				_fromPart = value;
			}
		}

		public WherePart WherePart
		{
			get
			{
				return _wherePart;
			}
			set
			{
				_wherePart = value;
			}
		}

		public GroupByPart GroupByPart
		{
			get
			{
				return _groupByPart;
			}
			set
			{
				_groupByPart = value;
			}
		}

		public HavingPart HavingPart
		{
			get
			{
				return _havingPart;
			}
			set
			{
				_havingPart = value;
			}
		}

		public OrderByPart OrderByPart
		{
			get
			{
				return _orderByPart;
			}
			set
			{
				_orderByPart = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression"></param>
		internal Select(Expression expression) : base(expression)
		{
		}
	}
}
