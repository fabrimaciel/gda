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
	/// Armazena os dados da DAO base relacionada com a classe.
	/// </summary>
	public class BaseDAOMapping : ElementMapping
	{
		/// <summary>
		/// Informações do tipo relacionado.
		/// </summary>
		public TypeInfo TypeInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Nome dos tipos genericos usados para instancia da DAO.
		/// </summary>
		public string[] GenericTypes
		{
			get;
			set;
		}

		/// <summary>
		/// Constroi uma instancia com base no dados do elemento.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="mappingNamespace">Namespace informado no mapeamento.</param>
		/// <param name="mappingAssembly">Assembly informado no mapeamento.</param>
		public BaseDAOMapping(XmlElement element, string mappingNamespace, string mappingAssembly)
		{
			var name = GetAttributeString(element, "name", true);
			this.TypeInfo = new TypeInfo(name, mappingNamespace, mappingAssembly);
			var generics = new List<string>();
			foreach (XmlElement i in element.GetElementsByTagName("genericType"))
				generics.Add(GetAttributeString(i, "name", true));
			GenericTypes = generics.ToArray();
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da DAO.
		/// </summary>
		/// <param name="typeInfo">Informações do tipo da DAO.</param>
		/// <param name="genericTypes">Tipo</param>
		public BaseDAOMapping(TypeInfo typeInfo, string[] genericTypes)
		{
			if(typeInfo == null)
				throw new ArgumentNullException("typeInfo");
			this.TypeInfo = typeInfo;
			this.GenericTypes = genericTypes ?? new string[0];
		}

		public PersistenceBaseDAOAttribute GetPersistenceBaseDAO()
		{
			var type = Type.GetType(this.TypeInfo.FullnameWithAssembly, true, true);
			var gTypes = new List<Type>();
			foreach (var i in GenericTypes)
				gTypes.Add(Type.GetType(i, true, true));
			return new PersistenceBaseDAOAttribute(type, gTypes.ToArray());
		}
	}
}
