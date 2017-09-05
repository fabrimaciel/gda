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
	/// Armazena os dados de um item estrangeiro do mapeamento.
	/// </summary>
	public abstract class ForeignItemMapping : ElementMapping
	{
		/// <summary>
		/// Tipo da classe que representa a tabela relacionada.
		/// </summary>
		public TypeInfo TypeOfClassRelated
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da propriedade da classe do tipo especificado aonde o relacionamento será feito.
		/// </summary>
		public string PropertyName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do grupo de relacionamento.
		/// </summary>
		public string GroupOfRelationship
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
		public ForeignItemMapping(XmlElement element, string mappingNamespace, string mappingAssembly)
		{
			var name = GetAttributeString(element, "typeOfClassRelated", true);
			TypeOfClassRelated = new TypeInfo(name, mappingNamespace, mappingAssembly);
			PropertyName = GetAttributeString(element, "propertyName", true);
			GroupOfRelationship = GetAttributeString(element, "groupOfRelationship");
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do item estrangeiro.
		/// </summary>
		/// <param name="typeOfClassRelated">Tipo da classe relacionada.</param>
		/// <param name="propertyName">Nome da propriedade relacionada.</param>
		/// <param name="groupOfRelationship">Nome do grupo de relacionamento.</param>
		public ForeignItemMapping(TypeInfo typeOfClassRelated, string propertyName, string groupOfRelationship)
		{
			if(typeOfClassRelated == null)
				throw new ArgumentNullException("typeOfClassRelated");
			else if(string.IsNullOrEmpty(propertyName))
				throw new ArgumentNullException("propertyName");
			this.TypeOfClassRelated = typeOfClassRelated;
			this.PropertyName = propertyName;
			this.GroupOfRelationship = groupOfRelationship;
		}
	}
}
