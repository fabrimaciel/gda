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

namespace GDA.Sql
{
	/// <summary>
	/// Armazena os dados do nome de uma tabela.
	/// </summary>
	public class TableName
	{
		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema onde ela está introduzida.
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TableName()
		{
		}

		/// <summary>
		/// Cria uma instancia já preenchendo os valores.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="schema"></param>
		public TableName(string name, string schema)
		{
			this.Name = name;
			this.Schema = schema;
		}

		public override string ToString()
		{
			return !string.IsNullOrEmpty(Schema) ? Schema + "." + Name : Name;
		}
	}
}
