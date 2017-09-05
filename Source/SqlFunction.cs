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
	class SqlFunction : SqlExpression
	{
		private List<List<SqlExpression>> _parameters = new List<List<SqlExpression>>();

		public List<List<SqlExpression>> Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value;
			}
		}

		public SqlFunction(Expression value) : base(value, GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(base.Value.Text).Append("(");
			bool started = false;
			foreach (List<SqlExpression> l in Parameters)
			{
				if(started)
					sb.Append(", ");
				foreach (SqlExpression se in l)
					sb.Append(se.Value.ToString());
				started = true;
			}
			sb.Append(")");
			return sb.ToString();
		}
	}
}
