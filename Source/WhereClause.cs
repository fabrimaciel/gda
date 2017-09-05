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
using System.Data;

namespace GDA.Sql
{
	/// <summary>
	/// Representa um clausula condicional Where.
	/// </summary>
	public abstract class WhereClause<T> where T : WhereClause<T>
	{
		private ConditionalContainer _container;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="container"></param>
		public WhereClause(ConditionalContainer container)
		{
			if(container == null)
				throw new ArgumentNullException("container");
			_container = container;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public WhereClause<T> And(Conditional conditional)
		{
			_container.And(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public WhereClause<T> Or(Conditional conditional)
		{
			_container.Or(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public WhereClause<T> And(string expression)
		{
			_container.And(expression);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public WhereClause<T> Or(string expression)
		{
			_container.Or(expression);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual WhereClause<T> Start(Conditional conditional)
		{
			_container.Start(conditional);
			return this;
		}

		/// <summary>
		///Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual WhereClause<T> Start(string expression)
		{
			_container.Start(expression);
			return this;
		}

		public override string ToString()
		{
			return _container.ToString();
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public WhereClause<T> Add(string name, DbType dbtype, object value)
		{
			_container.Add(new GDAParameter(name, value) {
				DbType = dbtype
			});
			return this;
		}

		public WhereClause<T> Add(string name, object value)
		{
			_container.Add(new GDAParameter(name, value));
			return this;
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public WhereClause<T> Add(DbType dbtype, int size, object value)
		{
			_container.Add(new GDAParameter() {
				DbType = dbtype,
				Size = size,
				Value = value
			});
			return this;
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="name">parameter name</param>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public WhereClause<T> Add(string name, DbType dbtype, int size, object value)
		{
			_container.Add(new GDAParameter() {
				ParameterName = name,
				DbType = dbtype,
				Size = size,
				Value = value
			});
			return this;
		}

		public WhereClause<T> Add(GDAParameter parameter)
		{
			_container.Add(parameter);
			return this;
		}
	}
}
