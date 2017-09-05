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
	/// Atributo usado para identifica a tabela que a classe ou interface est� mapeando.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class PersistenceClassAttribute : Attribute
	{
		private string _name;

		private string _schema;

		/// <summary>
		/// Nome da tabela que a classe representa.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Esquema da tabela no banco de dados.
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
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome da tabela que a classe representa.</param>
		public PersistenceClassAttribute(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		public PersistenceClassAttribute()
		{
		}

		public Sql.TableName GetTableName()
		{
			return new GDA.Sql.TableName(_name, _schema);
		}
	}
}
