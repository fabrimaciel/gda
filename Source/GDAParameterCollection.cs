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
using System.Collections;
using System.Data;

namespace GDA.Collections
{
	/// <summary>
	/// Coleção para trabalhar com os parametros do GDA.
	/// </summary>
	public class GDAParameterCollection : List<GDAParameter>, IGDAParameterContainer
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GDAParameterCollection() : base()
		{
		}

		public GDAParameterCollection(IEnumerable<GDAParameter> collection) : base(collection)
		{
		}

		public GDAParameterCollection(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Adiciona um parametro na consulta.
		/// </summary>
		/// <param name="dbtype">Tipo usado na base de dados</param>
		/// <param name="value">Valor do parametro.</param>
		public GDAParameterCollection Add(DbType dbtype, object value)
		{
			return Add("", dbtype, value);
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public GDAParameterCollection Add(string name, DbType dbtype, object value)
		{
			return Add(name, dbtype, 0, value);
		}

		public GDAParameterCollection Add(string name, object value)
		{
			Add(new GDAParameter(name, value));
			return this;
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public GDAParameterCollection Add(DbType dbtype, int size, object value)
		{
			return Add("", dbtype, size, value);
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="name">parameter name</param>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public GDAParameterCollection Add(string name, DbType dbtype, int size, object value)
		{
			GDAParameter p = new GDAParameter();
			p.ParameterName = name;
			p.DbType = dbtype;
			p.Size = size;
			p.Value = value;
			this.Add(p);
			return this;
		}

		/// <summary>
		/// Tenta recupera o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool TryGet(string name, out GDAParameter parameter)
		{
			foreach (var i in this)
				if(i.ParameterName == name)
				{
					parameter = i;
					return true;
				}
			parameter = null;
			return false;
		}

		/// <summary>
		/// Verifica se existe algum parametro com o nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		public bool ContainsKey(string name)
		{
			foreach (var i in this)
				if(i.ParameterName == name)
					return true;
			return false;
		}

		/// <summary>
		/// Remove o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Remove(string name)
		{
			var index = this.FindIndex(f => f.ParameterName == name);
			if(index >= 0)
			{
				this.RemoveAt(index);
				return true;
			}
			return false;
		}
	}
}
