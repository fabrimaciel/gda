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

namespace GDA.Common.Configuration.Attributes
{
	/// <summary>
	/// Atributo que identifica que o valor da propriedade ou método será recuperado do
	/// arquivo de configuração.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	internal sealed class ConfigurationAttribute : Attribute
	{
		/// <summary>
		/// Nome do nodo do arquivo XML que o atributo representa.
		/// </summary>
		private string _xmlNodePath;

		/// <summary>
		/// Armazena a informação sobre a presença do nodo no arquivo XML.
		/// Por padrão é obrigatório.
		/// </summary>
		private ConfigKeyPresence _keyPresenceRequirement = ConfigKeyPresence.Mandatory;

		/// <summary>
		/// Número de parametros que o método requer.
		/// </summary>
		private int _requiredParameters;

		/// <summary>
		/// Gets and Sets o nome do nodo do arquivo XML que o atributo representa.
		/// </summary>
		public string XmlNodePath
		{
			get
			{
				return _xmlNodePath;
			}
			set
			{
				_xmlNodePath = value;
			}
		}

		/// <summary>
		/// Get and Sets a informação sobre a presença do nodo no arquivo XML.
		/// </summary>
		public ConfigKeyPresence KeyPresenceRequirement
		{
			get
			{
				return _keyPresenceRequirement;
			}
			set
			{
				_keyPresenceRequirement = value;
			}
		}

		/// <summary>
		/// Número de parametros que o método requer.
		/// </summary>
		public int RequiredParameters
		{
			get
			{
				return _requiredParameters;
			}
			set
			{
				_requiredParameters = value;
			}
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="xmlNodePath">Nome do nodo que o atributo representa.</param>
		public ConfigurationAttribute(string xmlNodePath)
		{
			_xmlNodePath = xmlNodePath;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="xmlNodePath">Nome do nodo que o atributo representa.</param>
		/// <param name="keyPresenceRequirement">Informação sobre a presença do nodo no arquivo XML.</param>
		public ConfigurationAttribute(string xmlNodePath, ConfigKeyPresence keyPresenceRequirement) : this(xmlNodePath)
		{
			_keyPresenceRequirement = keyPresenceRequirement;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="xmlNodePath">Nome do nodo que o atributo representa.</param>
		/// <param name="keyPresenceRequirement">Informação sobre a presença do nodo no arquivo XML.</param>
		/// <param name="requiredParameters">Número de parametros que o método requer.</param>
		public ConfigurationAttribute(string xmlNodePath, ConfigKeyPresence keyPresenceRequirement, int requiredParameters) : this(xmlNodePath, keyPresenceRequirement)
		{
			_requiredParameters = requiredParameters;
		}
	}
}
