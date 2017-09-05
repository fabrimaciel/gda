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
	/// Armazena os dados do mapeamento do provedor relacionado com a classe mapeada.
	/// </summary>
	public class ProviderMapping : ElementMapping
	{
		/// <summary>
		/// Nome do provedor que será usado.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da configuração relacionada.
		/// </summary>
		public string ConfigurationName
		{
			get;
			set;
		}

		/// <summary>
		/// String de conexão relacionada com o provedor.
		/// </summary>
		public string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// Constrói uma instancia com base no dados do elemento.
		/// </summary>
		/// <param name="element"></param>
		public ProviderMapping(XmlElement element)
		{
			Name = GetAttributeString(element, "name");
			ConfigurationName = GetAttributeString(element, "configurationName", true);
			ConnectionString = GetAttributeString(element, "connectionString");
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento do Provider.
		/// </summary>
		/// <param name="name">Nome do provider.</param>
		/// <param name="configurationName">Nome da configuração do provider.</param>
		/// <param name="connectionString">String de conexão que será usada pelo provider.</param>
		public ProviderMapping(string name, string configurationName, string connectionString)
		{
			if(string.IsNullOrEmpty(configurationName))
				throw new ArgumentNullException("configurationName");
			this.Name = name;
			this.ConfigurationName = ConfigurationName;
			this.ConnectionString = connectionString;
		}

		public PersistenceProviderAttribute GetPersistenceProvider()
		{
			return new PersistenceProviderAttribute {
				ProviderName = Name,
				ConnectionString = ConnectionString,
				ProviderConfigurationName = ConfigurationName
			};
		}
	}
}
