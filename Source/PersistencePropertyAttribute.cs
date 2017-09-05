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
using System.Reflection;

namespace GDA
{
	/// <summary>
	/// Identifica o tipo de paramentro que a propriedade representa.
	/// </summary>
	public enum PersistenceParameterType
	{
		/// <summary>
		/// Idetifica um campo normal.
		/// </summary>
		Field,
		/// <summary>
		/// Identifica um campo do tipo chave prim�ria.
		/// </summary>
		Key,
		/// <summary>
		/// Identifica um campo do tipo chave prim�ria identidade.
		/// </summary>
		IdentityKey,
		/// <summary>
		/// Identifica um campo do tipo chave estrangeira.
		/// </summary>
		[Obsolete("See PersistenceForeignMemberAttribute")]
		ForeignKey
	}
	/// <summary>
	/// Identifica a dire��o em que os dados devem ser tratados no GDA.
	/// </summary>
	public enum DirectionParameter
	{
		/// <summary>
		/// Identifica que o valor dever� apenas ser enviando para a base de dados.
		/// </summary>
		Output,
		/// <summary>
		/// Identifica que o valor dever� apenas ser recuperado da base de dados.
		/// </summary>
		Input,
		/// <summary>
		/// Identifica que o valor poder� ser enviado ou recuperado da base de dados.
		/// </summary>
		InputOutput,
		/// <summary>
		/// O parametro � inserido apenas pelo comando insert, mas ele tamb�m pode ser considerado como um Input.
		/// </summary>
		OutputOnlyInsert,
		/// <summary>
		/// O parametro � inserido apenas pelo comando insert
		/// </summary>
		OnlyInsert,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado,
		/// e ele se comportar da mesma forma que o parametro Output.
		/// </summary>
		InputOptionalOutput,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado.
		/// </summary>
		InputOptional,
		/// <summary>
		/// O parametro busca o valor se ele existir no resultado, e ele se comportar da mesma forma que o
		/// parametro Output que � inserido apenas pelo comando insert.
		/// </summary>
		InputOptionalOutputOnlyInsert
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class PersistencePropertyAttribute : Attribute
	{
		private string m_Name;

		private PersistenceParameterType m_ParameterType = PersistenceParameterType.Field;

		private int m_Size = 0;

		private DirectionParameter m_Direction = DirectionParameter.InputOutput;

		private string _propertyName;

		private string _generatorKeyName;

		private bool _isNotNull = false;

		/// <summary>
		/// Nome que a propriedade representa no BD.
		/// </summary>
		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		/// <summary>
		/// Tipo de campo representado no banco de dados.
		/// </summary>
		public PersistenceParameterType ParameterType
		{
			get
			{
				return m_ParameterType;
			}
			set
			{
				m_ParameterType = value;
			}
		}

		/// <summary>
		/// Tamaho maximo do campo no BD.
		/// </summary>
		public int Size
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size = value;
			}
		}

		/// <summary>
		/// Sentido em que os dados da propriedade devem ser tratados pelo GDA.
		/// </summary>
		/// <value>Valor default: InputOutput.</value>
		public DirectionParameter Direction
		{
			get
			{
				return m_Direction;
			}
			set
			{
				m_Direction = value;
			}
		}

		/// <summary>
		/// Nome da propriedade mapeada.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return _propertyName;
			}
			set
			{
				_propertyName = value;
			}
		}

		/// <summary>
		/// Nome do gerador de c�digo que ser� usado.
		/// </summary>
		public string GeneratorKeyName
		{
			get
			{
				return _generatorKeyName;
			}
			set
			{
				_generatorKeyName = value;
			}
		}

		/// <summary>
		/// Identifica que a propriedade n�o aceita valores nulos.
		/// </summary>
		public bool IsNotNull
		{
			get
			{
				return _isNotNull;
			}
			set
			{
				_isNotNull = value;
			}
		}

		/// <summary>
		/// Construtor que define o nome do campo como sendo o nome da propriedade.
		/// </summary>
		public PersistencePropertyAttribute()
		{
		}

		/// <summary>
		/// Cria um attributo com base no nome e no tamanho.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="size"></param>
		[Obsolete]
		public PersistencePropertyAttribute(string name, uint size) : this(name)
		{
			Size = (int)size;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameterType"></param>
		public PersistencePropertyAttribute(PersistenceParameterType parameterType)
		{
			this.m_ParameterType = parameterType;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		public PersistencePropertyAttribute(string name)
		{
			m_Name = name;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="direction">Dire��o em que os dados devem ser tratados.</param>
		public PersistencePropertyAttribute(string name, DirectionParameter direction) : this(name)
		{
			m_Direction = direction;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="parameterType">Tipo do campo no BD.</param>
		public PersistencePropertyAttribute(string name, PersistenceParameterType parameterType)
		{
			m_Name = name;
			m_ParameterType = parameterType;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="parameterType">Tipo do campo no BD.</param>
		/// <param name="direction">Dire��o em que os dados devem ser tratados.</param>
		public PersistencePropertyAttribute(string name, PersistenceParameterType parameterType, DirectionParameter direction) : this(name, parameterType)
		{
			m_Direction = direction;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="parameterType">Tipo do campo no BD.</param>
		/// <param name="size">Tamanho que o campo.</param>
		public PersistencePropertyAttribute(string name, PersistenceParameterType parameterType, int size) : this(name, parameterType)
		{
			m_Size = size;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="parameterType">Tipo do campo no BD.</param>
		/// <param name="size">Tamanho que o campo.</param>
		/// <param name="direction">Dire��o em que os dados devem ser tratados.</param>
		public PersistencePropertyAttribute(string name, PersistenceParameterType parameterType, int size, DirectionParameter direction) : this(name, parameterType, direction)
		{
			m_Size = size;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="size">Tamanho que o campo.</param>
		public PersistencePropertyAttribute(string name, int size)
		{
			m_Name = name;
			m_Size = size;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="name">Nome que o campo representa no BD.</param>
		/// <param name="size">Tamanho que o campo.</param>
		/// <param name="direction">Dire��o em que os dados devem ser tratados.</param>
		public PersistencePropertyAttribute(string name, int size, DirectionParameter direction)
		{
			m_Name = name;
			m_Size = size;
			m_Direction = direction;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
