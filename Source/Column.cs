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
	/// Armazena as informações do campo.
	/// </summary>
	internal class Column
	{
		/// <summary>
		/// Nome do campo.
		/// </summary>
		private string _name;

		/// <summary>
		/// Nome da tabela que o campo está relacionado.
		/// </summary>
		private string _tableName;

		/// <summary>
		/// Apelido atribuido a coluna.
		/// </summary>
		private string _alias;

		/// <summary>
		/// Nome do campo.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Nome da tabela que o campo está relacionado.
		/// </summary>
		public string TableName
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

		/// <summary>
		/// Apelido atribuido a coluna.
		/// </summary>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tableName">Nome da tabela.</param>
		/// <param name="columnName">Nome da coluna.</param>
		/// <param name="alias">Apelido da coluna.</param>
		public Column(string tableName, string columnName, string alias)
		{
			_tableName = tableName;
			_name = columnName;
			_alias = alias;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression">Expressão onde está contidos os dados da coluna.</param>
		/// <param name="aliasExpression">Expressão do apelido da coluna.</param>
		internal Column(SqlExpression expression, SqlExpression aliasExpression)
		{
			string info = expression.Value.Text;
			int pos = info.IndexOf('.');
			if(pos >= 0)
			{
				_tableName = info.Substring(0, pos);
				_name = info.Substring(pos + 1, info.Length - pos - 1);
			}
			else
				_name = info;
			if(aliasExpression != null)
				_alias = aliasExpression.Value.Text;
		}
	}
}
