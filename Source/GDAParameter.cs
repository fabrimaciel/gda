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
using System.Data.Common;
using System.Data;

namespace GDA
{
	/// <summary>
	/// Assinatura das instancia que contem parametros.
	/// </summary>
	public interface IGDAParameterContainer : IEnumerable<GDAParameter>
	{
		/// <summary>
		/// Adiciona uma novo parametro.
		/// </summary>
		/// <param name="parameter"></param>
		void Add(GDAParameter parameter);

		/// <summary>
		/// Tenta recupera o parametro pelo nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="parameter">Instancia do parametro encontrado.</param>
		/// <returns></returns>
		bool TryGet(string name, out GDAParameter parameter);

		/// <summary>
		/// Verifica se existe algum parametro com o nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		bool ContainsKey(string name);

		/// <summary>
		/// Remove o parametro informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		bool Remove(string name);
	}
	/// <summary>
	/// Representa um parametro do GDA.
	/// </summary>
	public class GDAParameter
	{
		/// <summary>
		/// Tipo do parametro.
		/// </summary>
		private System.Data.DbType _dbType;

		private Type _valueType;

		private string _parameterName;

		private object _value;

		private int _size;

		private string _sourceColumn;

		private ParameterDirection _direction = ParameterDirection.Input;

		private bool _isNullable;

		private bool _dbTypeIsDefined = false;

		private bool _nativeDbTypeIsDefined = false;

		private object _nativeDbType;

		/// <summary>
		/// Construtor padr�o.
		/// </summary>
		public GDAParameter()
		{
		}

		public GDAParameter(string parameterName, object value)
		{
			_parameterName = parameterName;
			if(value != null && value.GetType().IsEnum)
			{
				switch(Enum.GetUnderlyingType(value.GetType()).Name)
				{
				case "Int16":
					_value = (short)value;
					break;
				case "UInt16":
					_value = (ushort)value;
					break;
				case "Int32":
					_value = (int)value;
					break;
				case "UInt32":
					_value = (uint)value;
					break;
				case "Byte":
					_value = (byte)value;
					break;
				default:
					_value = value;
					break;
				}
			}
			else
				_value = value;
		}

		public GDAParameter(string parameterName, object value, int size) : this(parameterName, value)
		{
			_size = size;
		}

		public GDAParameter(string parameterName, object value, string fieldName) : this(parameterName, value)
		{
			_sourceColumn = fieldName;
		}

		public GDAParameter(string parameterName, object value, string fieldName, int size) : this(parameterName, value, fieldName)
		{
			_size = size;
		}

		public GDAParameter(string parameterName, object value, ParameterDirection direction) : this(parameterName, value)
		{
			this._direction = direction;
		}

		/// <summary>
		/// Tipo da base de dados tratado pelo parametro.
		/// </summary>
		public DbType DbType
		{
			get
			{
				return _dbType;
			}
			set
			{
				_dbType = value;
				_dbTypeIsDefined = true;
			}
		}

		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string ParameterName
		{
			get
			{
				return _parameterName;
			}
			set
			{
				_parameterName = value;
			}
		}

		/// <summary>
		/// Valor do parametro.
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
		/// Tamanho do parametro.
		/// </summary>
		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Dire��o do parametro.
		/// </summary>
		public System.Data.ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		/// <summary>
		/// Identifica se o parametro aceita valores nulos.
		/// </summary>
		public bool IsNullable
		{
			get
			{
				return _isNullable;
			}
			set
			{
				_isNullable = value;
			}
		}

		/// <summary>
		/// Fonte da coluna do parametro.
		/// </summary>
		public string SourceColumn
		{
			get
			{
				return _sourceColumn;
			}
			set
			{
				_sourceColumn = value;
			}
		}

		/// <summary>
		/// Identifica se o tipo do banco de dados foi definido.
		/// </summary>
		public bool DbTypeIsDefined
		{
			get
			{
				return _dbTypeIsDefined;
			}
			set
			{
				_dbTypeIsDefined = value;
			}
		}

		/// <summary>
		/// Tipo do DB nativo.
		/// </summary>
		public object NativeDbType
		{
			get
			{
				return _nativeDbType;
			}
			set
			{
				_nativeDbType = value;
				_nativeDbTypeIsDefined = true;
			}
		}

		/// <summary>
		/// Identifica se o tipo do DB nativo foi definido.
		/// </summary>
		public bool NativeDbTypeIsDefined
		{
			get
			{
				return _nativeDbTypeIsDefined;
			}
			set
			{
				_nativeDbTypeIsDefined = value;
			}
		}
	}
}
