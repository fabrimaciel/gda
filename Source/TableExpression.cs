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
	class TableNameExpression
	{
		private string _schema;

		private Expression _innerExpression;

		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public string Name
		{
			get
			{
				return _innerExpression.Text;
			}
			set
			{
				_innerExpression.Text = value;
			}
		}

		/// <summary>
		/// Esquema onde a tabela está introduzida.
		/// </summary>
		public string Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				_schema = value;
			}
		}

		/// <summary>
		/// Expressão interna.
		/// </summary>
		public Expression InnerExpression
		{
			get
			{
				return _innerExpression;
			}
			set
			{
				_innerExpression = value;
			}
		}

		public TableNameExpression(Expression innerExpression)
		{
			_innerExpression = innerExpression;
		}
	}
	class TableExpression
	{
		private TableNameExpression _tableName;

		private Select _selectInfo;

		private Expression _tableAlias;

		/// <summary>
		/// Expressão que armazena o AS se existir.
		/// </summary>
		private Expression _asExpression;

		private Expression _leftOrRight;

		private Expression _outerOrInnerOrCrossOrNatural;

		private Expression _join;

		private Expression _on;

		private ContainerSqlExpression _onExpressions;

		public TableNameExpression TableName
		{
			get
			{
				return _tableName;
			}
			set
			{
				_tableName = value;
			}
		}

		public Select SelectInfo
		{
			get
			{
				return _selectInfo;
			}
			set
			{
				_selectInfo = value;
			}
		}

		public Expression TableAlias
		{
			get
			{
				return _tableAlias;
			}
			set
			{
				_tableAlias = value;
			}
		}

		/// <summary>
		/// Expressão que armazena o AS se existir.
		/// </summary>
		public Expression AsExpression
		{
			get
			{
				return _asExpression;
			}
			set
			{
				_asExpression = value;
			}
		}

		public Expression LeftOrRight
		{
			get
			{
				return _leftOrRight;
			}
			set
			{
				_leftOrRight = value;
			}
		}

		public Expression OuterOrInnerOrCrossOrNatural
		{
			get
			{
				return _outerOrInnerOrCrossOrNatural;
			}
			set
			{
				_outerOrInnerOrCrossOrNatural = value;
			}
		}

		public Expression Join
		{
			get
			{
				return _join;
			}
			set
			{
				_join = value;
			}
		}

		public Expression On
		{
			get
			{
				return _on;
			}
			set
			{
				_on = value;
			}
		}

		public ContainerSqlExpression OnExpressions
		{
			get
			{
				return _onExpressions;
			}
			set
			{
				_onExpressions = value;
			}
		}
	}
}
