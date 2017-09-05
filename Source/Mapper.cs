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
	/// Armazena o mapeamento usado no GDA.
	/// </summary>
	public class Mapper
	{
		/// <summary>
		/// Nome do campo na tabela na qual a propriedade está relacionada.
		/// </summary>
		private string _name;

		/// <summary>
		/// Tipo do parametro.
		/// </summary>
		private PersistenceParameterType _parameterType;

		/// <summary>
		/// Tamanho do campo.
		/// </summary>
		private int _size;

		/// <summary>
		/// Direção como o parametro se comporta.
		/// </summary>
		private DirectionParameter _direction;

		/// <summary>
		/// Nome da propriedade mapeada.
		/// </summary>
		private string _propertyMapperName;

		/// <summary>
		/// Propriedade mapeada.
		/// </summary>
		private PropertyInfo _propertyMapper;

		/// <summary>
		/// Validação relacionada com o mapeamento.
		/// </summary>
		private ValidationMapper _validation;

		/// <summary>
		/// Informações se houver foreignkeys relacionadas.
		/// </summary>
		private IList<ForeignKeyMapper> _foreignKeys;

		/// <summary>
		/// Tipo da model do mapeamento.
		/// </summary>
		private Type _modelType;

		/// <summary>
		/// Nome da gerador de código relacionado.
		/// </summary>
		private string _generatorKeyName;

		/// <summary>
		/// Instancia do gerador de chave relacionado.
		/// </summary>
		private IGeneratorKey _generatorKey;

		/// <summary>
		/// Identifica se a propriedade aceita ou não valores nulos.
		/// </summary>
		private bool _isNotNull;

		/// <summary>
		/// Nome do campo da tabela na qual a propriedade está relacionada.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Tipo do parâmetro.
		/// </summary>
		public PersistenceParameterType ParameterType
		{
			get
			{
				return _parameterType;
			}
			set
			{
				_parameterType = value;
			}
		}

		/// <summary>
		/// Tamanho máximo do campo.
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
		/// Direção como o parâmetro se comporta.
		/// </summary>
		public DirectionParameter Direction
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
		/// Propriedade mapeada.
		/// </summary>
		public PropertyInfo PropertyMapper
		{
			get
			{
				return _propertyMapper;
			}
		}

		/// <summary>
		/// Nome da propriedade mapeada.
		/// </summary>
		public string PropertyMapperName
		{
			get
			{
				return _propertyMapperName;
			}
		}

		/// <summary>
		/// Informações se houver foreignkey relacionada.
		/// </summary>
		public IList<ForeignKeyMapper> ForeignKeys
		{
			get
			{
				return _foreignKeys;
			}
		}

		/// <summary>
		/// Validação relacionada com o mapeamento.
		/// </summary>
		public ValidationMapper Validation
		{
			get
			{
				return _validation;
			}
			internal set
			{
				_validation = value;
			}
		}

		/// <summary>
		/// Nome da gerador de código relacionado.
		/// </summary>
		public string GeneratorKeyName
		{
			get
			{
				return _generatorKeyName;
			}
			internal set
			{
				_generatorKeyName = value;
			}
		}

		/// <summary>
		/// Instancia do gerador de código relacionado.
		/// </summary>
		public IGeneratorKey GeneratorKey
		{
			get
			{
				if(_generatorKey == null && !string.IsNullOrEmpty(_generatorKeyName))
				{
					_generatorKey = GDASettings.GetGeneratorKey(_generatorKeyName);
					if(_generatorKey == null)
						throw new GDAException("Generator key \"{0}\" not found.", _generatorKeyName);
				}
				return _generatorKey;
			}
		}

		/// <summary>
		/// Identifica se a propriedade aceita ou não valores nulos.
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
		/// Identifica se o tipo é Nullable
		/// </summary>
		public bool IsNullableType
		{
			get
			{
				return Helper.TypeHelper.IsNullableType(PropertyMapper.PropertyType);
			}
		}

		/// <summary>
		/// Tipo de declaração do membro.
		/// </summary>
		public Type DeclaringType
		{
			get
			{
				return PropertyMapper.DeclaringType;
			}
		}

		public Type Type
		{
			get
			{
				return Helper.TypeHelper.GetMemberType(PropertyMapper);
			}
		}

		/// <summary>
		/// Identifica se o membro é uma associação.
		/// </summary>
		public bool IsAssociation
		{
			get
			{
				return (ForeignKeys != null && ForeignKeys.Count > 0);
			}
		}

		/// <summary>
		/// Verifica se o membro é gerado pelo banco de dados.
		/// </summary>
		public bool IsDbGenerated
		{
			get
			{
				return ParameterType == PersistenceParameterType.IdentityKey;
			}
		}

		/// <summary>
		/// Identifica se o membro é uma chave primária.
		/// </summary>
		public bool IsPrimaryKey
		{
			get
			{
				return ParameterType == PersistenceParameterType.Key || ParameterType == PersistenceParameterType.IdentityKey;
			}
		}

		/// <summary>
		/// Cria uma instancia para propriedade mapeada.
		/// </summary>
		/// <param name="modelType"></param>
		/// <param name="ppa"></param>
		/// <param name="propertyMapper"></param>
		public Mapper(Type modelType, PersistencePropertyAttribute ppa, PropertyInfo propertyMapper)
		{
			this._modelType = modelType;
			this._name = ppa.Name;
			this._direction = ppa.Direction;
			this._parameterType = ppa.ParameterType;
			this._size = ppa.Size;
			this._propertyMapper = propertyMapper;
			this._propertyMapperName = propertyMapper.Name;
			this._generatorKeyName = ppa.GeneratorKeyName;
			this._isNotNull = ppa.IsNotNull;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="modelType"></param>
		/// <param name="name"></param>
		/// <param name="direction"></param>
		/// <param name="parameterType"></param>
		/// <param name="size"></param>
		/// <param name="propertyMapper"></param>
		/// <param name="generatorKeyName">Nome do gerador de codigo relacionado.</param>
		public Mapper(Type modelType, string name, DirectionParameter direction, PersistenceParameterType parameterType, int size, PropertyInfo propertyMapper, string generatorKeyName)
		{
			this._modelType = modelType;
			this._name = name;
			this._direction = direction;
			this._parameterType = parameterType;
			this._size = size;
			this._propertyMapper = propertyMapper;
			if(propertyMapper != null)
				this._propertyMapperName = propertyMapper.Name;
			this._generatorKeyName = generatorKeyName;
		}

		/// <summary>
		/// Adiciona uma chave estrangeira para o mapeamento.
		/// </summary>
		/// <param name="fkMapper"></param>
		public void AddForeignKey(ForeignKeyMapper fkMapper)
		{
			if(_foreignKeys == null)
				_foreignKeys = new List<ForeignKeyMapper>();
			_foreignKeys.Add(fkMapper);
		}

		public override string ToString()
		{
			return _propertyMapperName + " : " + _name;
		}
	}
}
