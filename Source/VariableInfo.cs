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
using System.Linq;
using System.Text;

namespace GDA.Sql
{
	/// <summary>
	/// Armazena as informações de uma variável.
	/// </summary>
	public class VariableInfo
	{
		private GDA.Sql.InterpreterExpression.Expression _expression;

		private string _name;

		/// <summary>
		/// Nome da variável associada.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Construtor padrão,
		/// </summary>
		/// <param name="expression">Expressão associada.</param>
		/// <param name="name">Nome da variavel.</param>
		internal VariableInfo(GDA.Sql.InterpreterExpression.Expression expression)
		{
			_expression = expression;
			_name = expression.Text;
		}

		/// <summary>
		/// Executa o replace da variável.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="parameterContainer">Container dos parametros que serão processados.</param>
		/// <param name="classesDictionary"></param>
		/// <returns></returns>
		public bool Replace(GDA.Interfaces.IProvider provider, IGDAParameterContainer parameterContainer, Dictionary<string, Type> classesDictionary)
		{
			if(parameterContainer == null)
				return false;
			GDAParameter parameter = null;
			if(!parameterContainer.TryGet(Name, out parameter))
				return false;
			QueryReturnInfo returnInfo = null;
			if(parameter.Value is IQuery)
			{
				var query = (IQuery)parameter.Value;
				returnInfo = query.BuildResultInfo2(provider, query.AggregationFunctionProperty, classesDictionary);
			}
			else
				returnInfo = parameter.Value as QueryReturnInfo;
			if(returnInfo != null)
			{
				_expression.Text = returnInfo.CommandText;
				foreach (var p in returnInfo.Parameters)
					if(!parameterContainer.ContainsKey(p.ParameterName))
						parameterContainer.Add(p);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Implementação do comparador para as informações da variável.
		/// </summary>
		public class VariableInfoComparer : IComparer<VariableInfo>
		{
			public static readonly IComparer<VariableInfo> Instance;

			static VariableInfoComparer()
			{
				Instance = new VariableInfoComparer();
			}

			/// <summary>
			/// Compara s duas instancia com as informações das variáveis.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(VariableInfo x, VariableInfo y)
			{
				if(x == null && y != null)
					return -1;
				else if(x != null && y == null)
					return 1;
				else if(x == null && y == null)
					return 0;
				return StringComparer.Ordinal.Compare(x.Name, y.Name);
			}
		}
	}
}
