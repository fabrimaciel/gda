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
	public class ConditionalContainer : Conditional, IGDAParameterContainer
	{
		/// <summary>
		/// Condições do container.
		/// </summary>
		private List<Conditional> _conditionals = new List<Conditional>();

		/// <summary>
		/// Operadores lógicos usados.
		/// </summary>
		private List<LogicalOperator> _logicalOperators = new List<LogicalOperator>();

		private Collections.GDAParameterCollection _parameters = new Collections.GDAParameterCollection();

		private IGDAParameterContainer _parameterContainer;

		public override ConditionalContainer Parent
		{
			get
			{
				return base.Parent;
			}
			internal set
			{
				base.Parent = value;
				ParameterContainer = value.ParameterContainer;
			}
		}

		/// <summary>
		/// Container onde são registrados os parametros.
		/// </summary>
		public virtual IGDAParameterContainer ParameterContainer
		{
			get
			{
				if(_parameterContainer == null)
					_parameterContainer = new Collections.GDAParameterCollection();
				return _parameterContainer;
			}
			set
			{
				if(_parameterContainer != null && value != null && _parameterContainer != value)
					foreach (var i in _parameterContainer)
					{
						var found = false;
						foreach (var j in value)
							if(j.ParameterName == i.ParameterName)
							{
								found = true;
								continue;
							}
						if(!found)
							value.Add(i);
					}
				_parameterContainer = value;
			}
		}

		/// <summary>
		/// Quantidade de itens no container.
		/// </summary>
		public int Count
		{
			get
			{
				return _conditionals.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ConditionalContainer()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression"></param>
		public ConditionalContainer(string expression)
		{
			if(!string.IsNullOrEmpty(expression))
			{
				var cond = new Conditional(expression);
				cond.Parent = this;
				_conditionals.Add(cond);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="conditional"></param>
		public ConditionalContainer(Conditional conditional)
		{
			if(conditional != null)
			{
				conditional.Parent = this;
				_conditionals.Add(conditional);
				if(conditional is ConditionalContainer)
				{
					var cc = (ConditionalContainer)conditional;
					if(cc.ParameterContainer != this.ParameterContainer)
						cc.ParameterContainer = this.ParameterContainer;
				}
			}
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual ConditionalContainer And(Conditional conditional)
		{
			if(conditional == null)
				throw new ArgumentNullException("conditional");
			conditional.Parent = this;
			_conditionals.Add(conditional);
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.And);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Or(Conditional conditional)
		{
			if(conditional == null)
				throw new ArgumentNullException("conditional");
			conditional.Parent = this;
			_conditionals.Add(conditional);
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.Or);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer And(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			_conditionals.Add(new Conditional(expression));
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.And);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Or(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			_conditionals.Add(new Conditional(expression));
			if(_conditionals.Count > 1)
				_logicalOperators.Add(LogicalOperator.Or);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Start(Conditional conditional)
		{
			if(conditional == null)
				throw new ArgumentNullException("conditional");
			foreach (var i in _conditionals)
			{
				i.Parent = null;
			}
			_conditionals.Clear();
			_logicalOperators.Clear();
			conditional.Parent = this;
			_conditionals.Add(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual ConditionalContainer Start(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			foreach (var i in _conditionals)
			{
				i.Parent = null;
			}
			_conditionals.Clear();
			_logicalOperators.Clear();
			_conditionals.Add(new Conditional(expression));
			return this;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			for(int i = 0, j = -1; i < _conditionals.Count; i++, j++)
			{
				if(_logicalOperators.Count > j && j >= 0)
					sb.Append(_logicalOperators[j] == LogicalOperator.Or ? " OR " : " AND ");
				sb.Append(_conditionals[i].ToString());
			}
			if(Parent != null)
				return "(" + sb.ToString() + ")";
			else
				return sb.ToString();
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public ConditionalContainer Add(string name, DbType dbtype, object value)
		{
			ParameterContainer.Add(new GDAParameter(name, value) {
				DbType = dbtype
			});
			return this;
		}

		public ConditionalContainer Add(string name, object value)
		{
			ParameterContainer.Add(new GDAParameter(name, value));
			return this;
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public ConditionalContainer Add(DbType dbtype, int size, object value)
		{
			ParameterContainer.Add(new GDAParameter() {
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
		public ConditionalContainer Add(string name, DbType dbtype, int size, object value)
		{
			ParameterContainer.Add(new GDAParameter() {
				ParameterName = name,
				DbType = dbtype,
				Size = size,
				Value = value
			});
			return this;
		}

		public ConditionalContainer Add(GDAParameter parameter)
		{
			ParameterContainer.Add(parameter);
			return this;
		}

		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator()
		{
			return ParameterContainer.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ParameterContainer.GetEnumerator();
		}

		void IGDAParameterContainer.Add(GDAParameter parameter)
		{
			ParameterContainer.Add(parameter);
		}

		/// <summary>
		/// Tenta recupera o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		bool IGDAParameterContainer.TryGet(string name, out GDAParameter parameter)
		{
			return ParameterContainer.TryGet(name, out parameter);
		}

		/// <summary>
		/// Verifica se existe algum parametro com o nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		bool IGDAParameterContainer.ContainsKey(string name)
		{
			return ParameterContainer.ContainsKey(name);
		}

		/// <summary>
		/// Remove o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool IGDAParameterContainer.Remove(string name)
		{
			return ParameterContainer.Remove(name);
		}
	}
}
