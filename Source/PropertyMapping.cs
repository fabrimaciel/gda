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
using System.Xml;

namespace GDA.Mapping
{
	/// <summary>
	/// Armazena as informações básica do mapeamento da propriedade
	/// </summary>
	public interface IPropertyMappingInfo
	{
		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da coluna que a propriedade mapea.
		/// </summary>
		string Column
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da propriedade.
		/// </summary>
		DirectionParameter Direction
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do parametro que a propriedade representa.
		/// </summary>
		PersistenceParameterType ParameterType
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Armazena as informações do mapeamento da propriedade.
	/// </summary>
	public class PropertyMappingInfo : IPropertyMappingInfo
	{
		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da coluna que a propriedade mapea.
		/// </summary>
		public string Column
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da propriedade.
		/// </summary>
		public DirectionParameter Direction
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do parametro que a propriedade representa.
		/// </summary>
		public PersistenceParameterType ParameterType
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Armazena os dados do mapeamento da propriedade.
	/// </summary>
	public class PropertyMapping : ElementMapping, IPropertyMappingInfo
	{
		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da coluna que a propriedade mapea.
		/// </summary>
		public string Column
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do parametro que a propriedade representa.
		/// </summary>
		public PersistenceParameterType ParameterType
		{
			get;
			set;
		}

		/// <summary>
		/// Tamanho do campo.
		/// </summary>
		public int Size
		{
			get;
			set;
		}

		/// <summary>
		/// Direção da propriedade.
		/// </summary>
		public DirectionParameter Direction
		{
			get;
			set;
		}

		/// <summary>
		/// Dados da chave estrangeira relacionada.
		/// </summary>
		public ForeignKeyMapping ForeignKey
		{
			get;
			set;
		}

		/// <summary>
		/// Dados do membro estrangeiro relacionado.
		/// </summary>
		public ForeignMemberMapping ForeignMember
		{
			get;
			set;
		}

		/// <summary>
		/// Validadores da propriedade.
		/// </summary>
		public List<ValidatorMapping> Validators
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do gerador de codigo relacionado.
		/// </summary>
		public string GeneratorKeyName
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica que a propriedade não deve ser persistida.
		/// </summary>
		public bool IsNotPersists
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica que a propriedade não aceita valores nulos.
		/// </summary>
		public bool IsNotNull
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="mappingNamespace">Namespace informado no mapeamento.</param>
		/// <param name="mappingAssembly">Assembly informado no mapeamento.</param>
		public PropertyMapping(XmlElement element, string mappingNamespace, string mappingAssembly)
		{
			Name = GetAttributeString(element, "name", true);
			Column = GetAttributeString(element, "column", Name);
			var value = GetAttributeString(element, "parameterType", "Field");
			ParameterType = (PersistenceParameterType)Enum.Parse(typeof(PersistenceParameterType), value, true);
			value = GetAttributeString(element, "size");
			var size = 0;
			if(Helper.GDAHelper.TryParse(value, out size))
				Size = size;
			bool option = false;
			if(Helper.GDAHelper.TryParse(GetAttributeString(element, "not-persists"), out option))
				IsNotPersists = option;
			if(Helper.GDAHelper.TryParse(GetAttributeString(element, "not-null"), out option))
				IsNotNull = option;
			value = GetAttributeString(element, "direction", "InputOutput");
			Direction = (DirectionParameter)Enum.Parse(typeof(DirectionParameter), value, true);
			var gkElement = FirstOrDefault<XmlElement>(element.GetElementsByTagName("generator"));
			if(gkElement != null)
				GeneratorKeyName = GetAttributeString(gkElement, "name", true);
			var fkElement = FirstOrDefault<XmlElement>(element.GetElementsByTagName("foreignKey"));
			if(fkElement != null)
				ForeignKey = new ForeignKeyMapping(fkElement, mappingNamespace, mappingAssembly);
			var fmElement = FirstOrDefault<XmlElement>(element.GetElementsByTagName("foreignMember"));
			if(fmElement != null)
				ForeignMember = new ForeignMemberMapping(fmElement, mappingNamespace, mappingAssembly);
			Validators = new List<ValidatorMapping>();
			foreach (XmlElement i in element.GetElementsByTagName("validator"))
			{
				var vm = new ValidatorMapping(i);
				if(!Validators.Exists(f => f.Name == vm.Name))
					Validators.Add(vm);
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da propriedade.
		/// </summary>
		/// <param name="name">Nome da propriedade</param>
		/// <param name="column">Nome da coluna relacionada com a propriedade.</param>
		/// <param name="parameterType">Tipo do parametro do mapeamento.</param>
		/// <param name="size">Tamanho da coluna.</param>
		/// <param name="isNotPersists">Identifica se a propriedade não para ser persistida.</param>
		/// <param name="isNotNull">Identifica se a propriedade aceita valores nulos.</param>
		/// <param name="direction">Direção do mapeamento da propriedade.</param>
		/// <param name="generatorKeyName">Nome do gerador de chave usado pela propriedade.</param>
		/// <param name="foreignKey">Dados da chave estrangeira relacionada.</param>
		/// <param name="foreignMember">Dados do membro estrangeiro relacionado.</param>
		/// <param name="validators">Validadores aplicados a propriedade.</param>
		public PropertyMapping(string name, string column, PersistenceParameterType parameterType, int size, bool isNotPersists, bool isNotNull, DirectionParameter direction, string generatorKeyName, ForeignKeyMapping foreignKey, ForeignMemberMapping foreignMember, IEnumerable<ValidatorMapping> validators)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Name = name;
			this.Column = string.IsNullOrEmpty(column) ? name : column;
			this.ParameterType = parameterType;
			this.Size = size;
			this.IsNotPersists = isNotPersists;
			this.IsNotNull = isNotNull;
			this.Direction = direction;
			this.GeneratorKeyName = generatorKeyName;
			this.ForeignKey = foreignKey;
			this.ForeignMember = foreignMember;
			Validators = new List<ValidatorMapping>();
			if(validators != null)
			{
				foreach (var i in validators)
					if(!Validators.Exists(f => f.Name == i.Name))
						Validators.Add(i);
			}
		}

		/// <summary>
		/// Recupera uma instancia do atributo de persistencia relacionado
		/// </summary>
		/// <param name="pi"></param>
		/// <returns></returns>
		public PersistencePropertyAttribute GetPersistenceProperty()
		{
			if(IsNotPersists)
				return null;
			return new PersistencePropertyAttribute {
				Name = Column,
				PropertyName = Name,
				Size = Size,
				ParameterType = ParameterType,
				Direction = Direction,
				GeneratorKeyName = GeneratorKeyName,
				IsNotNull = IsNotNull
			};
		}
	#if CLS_3_5
	
        /// <summary>
        /// Constrói um mapeamento com base no tipo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector"></param>
        /// <param name="column">Nome da coluna relacionada com a propriedade.</param>
        /// <returns></returns>
        public static PropertyMapping Build<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector, string column)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");

            return new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, column, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null);
        }
#endif
	}
}
