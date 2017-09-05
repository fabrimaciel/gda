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
	class GroupByPart
	{
		private Expression _group;

		private Expression _by;

		private List<SqlExpression> _expressions = new List<SqlExpression>();

		public Expression Group
		{
			get
			{
				return _group;
			}
			set
			{
				_group = value;
			}
		}

		public Expression By
		{
			get
			{
				return _by;
			}
			set
			{
				_by = value;
			}
		}

		public List<SqlExpression> Expressions
		{
			get
			{
				return _expressions;
			}
			set
			{
				_expressions = value;
			}
		}
	}
}
