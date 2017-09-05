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
	public class SqlPropertyInfo
	{
		private string _propertyName;

		private object _value;

		private Operator _sqlOperator;

		private bool _useOrderBy;

		private int _orderByPos;

		private Order _orderType = Order.Ascending;

		/// <summary>
		/// Constrói uma instância de SqlPropertyInfo com seus valores apropriados.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que representa.</param>
		/// <param name="value">Valor da propriedades que representa.</param>
		public SqlPropertyInfo(string propertyName, object value) : this(propertyName, value, Operator.Equals)
		{
		}

		/// <summary>
		/// Constrói uma instância de SqlPropertyInfo com seus valores apropriados.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que representa.</param>
		/// <param name="value">Valor da propriedades que representa.</param>
		/// <param name="sqlOperator">Operação a ser usada.</param>
		public SqlPropertyInfo(string propertyName, object value, Operator sqlOperator)
		{
			_propertyName = propertyName.Trim();
			_value = value;
			_sqlOperator = sqlOperator;
		}

		/// <summary>
		/// Nome da propriedade a ser usada na consulta.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
			set
			{
				_propertyName = value.Trim();
			}
		}

		/// <summary>
		/// Valor a ser utilizado na consulta.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Operador a ser usado na consulta.
		/// </summary>
		public Operator SqlOperator
		{
			get
			{
				return _sqlOperator;
			}
			set
			{
				_sqlOperator = value;
			}
		}

		/// <summary>
		/// Identifica se será usado ordenação pela propriedade.
		/// </summary>
		public bool UseOrderBy
		{
			get
			{
				return _useOrderBy;
			}
			set
			{
				_useOrderBy = value;
			}
		}

		/// <summary>
		/// Posição de ordenação em relacionação as outras propriedades.
		/// </summary>
		public int OrderByPos
		{
			get
			{
				return _orderByPos;
			}
			set
			{
				_orderByPos = value;
			}
		}

		/// <summary>
		/// Tipo de ordenação.
		/// </summary>
		public Order OrderType
		{
			get
			{
				return _orderType;
			}
			set
			{
				_orderType = value;
			}
		}
	}
}
