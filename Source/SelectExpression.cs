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
using GDA.Sql.InterpreterExpression.Enums;

namespace GDA.Sql.InterpreterExpression.Nodes
{
	/// <summary>
	/// Representa uma expressão da projeção da consulta
	/// </summary>
	class SelectExpression
	{
		private SqlExpression _columnName;

		private SqlExpression _columnAlias;

		/// <summary>
		/// Expressão que armazena o AS se existir.
		/// </summary>
		private Expression _asExpression;

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

		/// <summary>
		/// Expressão que contem o nome da coluna.
		/// </summary>
		public SqlExpression ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
			}
		}

		/// <summary>
		/// Expressão que contem o apelido da coluna.
		/// </summary>
		public SqlExpression ColumnAlias
		{
			get
			{
				return _columnAlias;
			}
			set
			{
				_columnAlias = value;
			}
		}

		/// <summary>
		/// Informações da coluna.
		/// </summary>
		public Column Column
		{
			get
			{
				if(ColumnName.Type == SqlExpressionType.Column)
					return new Column(ColumnName, ColumnAlias);
				else
					throw new SqlParserException("Invalid load column.");
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SelectExpression()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName">Expressão contendo o nome da coluna.</param>
		public SelectExpression(SqlExpression columnName) : this(columnName, null)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName">Expressão contendo o nome da coluna.</param>
		/// <param name="columnAlias">Expressão contendo o apelido da coluna.</param>
		public SelectExpression(SqlExpression columnName, SqlExpression columnAlias) : this(columnName, columnAlias, null)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName">Expressão contendo o nome da coluna.</param>
		/// <param name="columnAlias">Expressão contendo o apelido da coluna.</param>
		/// <param name="asExpression">Expressão AS.</param>
		public SelectExpression(SqlExpression columnName, SqlExpression columnAlias, Expression asExpression)
		{
			_columnName = columnName;
			_columnAlias = columnAlias;
			_asExpression = asExpression;
		}
	}
}
