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

namespace GDA.Analysis
{
	/// <summary>
	/// Armazena as informações de uma chave estrangeira.
	/// </summary>
	public class ForeignKeyMap
	{
		private string _constraintName;

		private string _constraintSchema;

		private string _foreignKeyTable;

		private string _foreignKeyTableSchema;

		private string _foreignKeyColumn;

		private string _primaryKeyTable;

		private string _primaryKeyTableSchema;

		private string _primaryKeyColumn;

		/// <summary>
		/// Nome da constraint da chave estrangeira.
		/// </summary>
		public string ConstraintName
		{
			get
			{
				return _constraintName;
			}
			set
			{
				_constraintName = value;
			}
		}

		/// <summary>
		/// Nome do esquema da constraint da chave estrangeira.
		/// </summary>
		public string ConstraintSchema
		{
			get
			{
				return _constraintSchema;
			}
			set
			{
				_constraintSchema = value;
			}
		}

		/// <summary>
		/// Nome da tabela da chave estrangeira.
		/// </summary>
		public string ForeignKeyTable
		{
			get
			{
				return _foreignKeyTable;
			}
			set
			{
				_foreignKeyTable = value;
			}
		}

		/// <summary>
		/// Esquema da tabela da chave estrangeira.
		/// </summary>
		public string ForeignKeyTableSchema
		{
			get
			{
				return _foreignKeyTableSchema;
			}
			set
			{
				_foreignKeyTableSchema = value;
			}
		}

		/// <summary>
		/// Nome da coluna da chave estrangeira.
		/// </summary>
		public string ForeignKeyColumn
		{
			get
			{
				return _foreignKeyColumn;
			}
			set
			{
				_foreignKeyColumn = value;
			}
		}

		/// <summary>
		/// Nome da tabela onde está a chave primaria.
		/// </summary>
		public string PrimaryKeyTable
		{
			get
			{
				return _primaryKeyTable;
			}
			set
			{
				_primaryKeyTable = value;
			}
		}

		/// <summary>
		/// Esquema da tabela da chave primária.
		/// </summary>
		public string PrimaryKeyTableSchema
		{
			get
			{
				return _primaryKeyTableSchema;
			}
			set
			{
				_primaryKeyTableSchema = value;
			}
		}

		/// <summary>
		/// Nome da coluna da chave primaria.
		/// </summary>
		public string PrimaryKeyColumn
		{
			get
			{
				return _primaryKeyColumn;
			}
			set
			{
				_primaryKeyColumn = value;
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return !string.IsNullOrEmpty(ConstraintSchema) ? string.Format("[{0}].[{1}]", ConstraintSchema, ConstraintName) : string.Format("[{0}]", ConstraintName);
		}
	}
}
