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
	/// Armazena os dados de uma classe mapeada.
	/// </summary>
	public class ClassMapping : ElementMapping
	{
		/// <summary>
		/// Informações do tipo da classe.
		/// </summary>
		public TypeInfo TypeInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da tabela que a classe mapea.
		/// </summary>
		public string Table
		{
			get;
			set;
		}

		/// <summary>
		/// Esquema da tabela que a classe mapea.
		/// </summary>
		public string Schema
		{
			get;
			set;
		}

		/// <summary>
		/// DAO base relacionada com a classe.
		/// </summary>
		public BaseDAOMapping BaseDAO
		{
			get;
			set;
		}

		/// <summary>
		/// Provedor relacionado com o mapeamento.
		/// </summary>
		public ProviderMapping Provider
		{
			get;
			set;
		}

		/// <summary>
		/// Lista das propriedades mapeada para a classe.
		/// </summary>
		public List<PropertyMapping> Properties
		{
			get;
			set;
		}

		/// <summary>
		/// Constrói uma instancia com base nos dados do elemento.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="mappingNamespace">Namespace informado no mapeamento.</param>
		/// <param name="mappingAssembly">Assembly informado no mapeamento.</param>
		/// <param name="defaultProviderName">Nome do provider padrão que poderá ser usado pela classe.</param>
		/// <param name="defaultProviderConfigurationName">Nome da configuração do provedor que podera ser usada</param>
		/// <param name="defaultConnectionString">String de conexão padrão que poderá ser usada.</param>
		/// <param name="defaultSchema">Esquema padrão da tabela da classe.</param>
		public ClassMapping(XmlElement element, string mappingNamespace, string mappingAssembly, string defaultProviderName, string defaultProviderConfigurationName, string defaultConnectionString, string defaultSchema)
		{
			var name = GetAttributeString(element, "name", true);
			this.TypeInfo = new TypeInfo(name, mappingNamespace, mappingAssembly);
			Table = GetAttributeString(element, "table", this.TypeInfo.Name);
			Schema = GetAttributeString(element, "schema");
			if(string.IsNullOrEmpty(Schema))
				Schema = defaultSchema;
			XmlElement baseDAOElement = FirstOrDefault<XmlElement>(element.GetElementsByTagName("baseDAO"));
			if(baseDAOElement != null)
				BaseDAO = new BaseDAOMapping(baseDAOElement, mappingNamespace, mappingAssembly);
			var providerElement = FirstOrDefault<XmlElement>(element.GetElementsByTagName("provider"));
			if(providerElement != null)
				Provider = new ProviderMapping(providerElement);
			else if(!string.IsNullOrEmpty(defaultProviderConfigurationName))
			{
				Provider = new ProviderMapping(defaultProviderName, defaultProviderConfigurationName, defaultConnectionString);
			}
			Properties = new List<PropertyMapping>();
			foreach (XmlElement i in element.GetElementsByTagName("property"))
			{
				var pm = new PropertyMapping(i, mappingNamespace, mappingAssembly);
				if(!Properties.Exists(f => f.Name == pm.Name))
					Properties.Add(pm);
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da classe.
		/// </summary>
		/// <param name="typeInfo">Informações do tipo.</param>
		/// <param name="table">Nome da tabela equivalente do BD.</param>
		/// <param name="schema">Esquema onde a tabela está inserida.</param>
		/// <param name="baseDAO">Informações sobre a DAO relacionada.</param>
		/// <param name="provider">Informações do Provider relacionado.</param>
		/// <param name="properties">Propriedades mapeadas.</param>
		public ClassMapping(TypeInfo typeInfo, string table, string schema, BaseDAOMapping baseDAO, ProviderMapping provider, IEnumerable<PropertyMapping> properties)
		{
			if(typeInfo == null)
				throw new ArgumentNullException("typeInfo");
			TypeInfo = typeInfo;
			if(string.IsNullOrEmpty(table))
				Table = typeInfo.Name;
			else
				Table = table;
			Schema = schema;
			BaseDAO = baseDAO;
			Provider = provider;
			Properties = new List<PropertyMapping>();
			if(properties != null)
				foreach (var i in properties)
					if(!Properties.Exists(f => f.Name == i.Name))
						Properties.Add(i);
		}

		/// <summary>
		/// Recupera o atribute de classe persistente.
		/// </summary>
		/// <returns></returns>
		public PersistenceClassAttribute GetPersistenceClass()
		{
			return new PersistenceClassAttribute(Table) {
				Schema = Schema
			};
		}
	}
}
