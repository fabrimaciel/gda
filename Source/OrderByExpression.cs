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
	class OrderByExpression
	{
		private ContainerSqlExpression _expression;

		private Expression _ascOrDesc;

		/// <summary>
		/// Identifica se o item tem direção ascedente.
		/// </summary>
		private bool _asc = true;

		private bool _nulls = false;

		private bool _first = true;

		public ContainerSqlExpression Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				_expression = value;
			}
		}

		public Expression AscOrDesc
		{
			get
			{
				return _ascOrDesc;
			}
			set
			{
				_ascOrDesc = value;
			}
		}

		public bool Asc
		{
			get
			{
				return _asc;
			}
			set
			{
				_asc = value;
			}
		}

		public bool Nulls
		{
			get
			{
				return _nulls;
			}
			set
			{
				_nulls = value;
			}
		}

		public bool First
		{
			get
			{
				return _first;
			}
			set
			{
				_first = value;
			}
		}
	}
}
