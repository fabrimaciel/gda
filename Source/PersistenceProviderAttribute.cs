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

namespace GDA
{
	/// <summary>
	/// Atribute usado para definir qual provider a classe utilizará
	/// para acessar os dados.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class PersistenceProviderAttribute : Attribute
	{
		private string _providerConfigurationName;

		private string _providerName;

		private string _connectionString;

		/// <summary>
		/// Nome do provider de configuração que a classe irá utilizar.
		/// </summary>
		public string ProviderConfigurationName
		{
			get
			{
				return _providerConfigurationName;
			}
			set
			{
				_providerConfigurationName = value;
			}
		}

		/// <summary>
		/// Nome do provider que a classe irá usar
		/// para comunicação com o banco de dados.
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		/// <summary>
		/// ConnectionString usado para conectar com o banco de dados.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
			set
			{
				_connectionString = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PersistenceProviderAttribute()
		{
		}

		/// <summary>
		/// Instancia um novo <see cref="PersistenceProviderAttribute"/>
		/// já com o nome do ProviderConfiguration definido.
		/// </summary>
		/// <param name="providerConfigurationName"> Nome do provider de configuração que a classe irá utilizar.</param>
		public PersistenceProviderAttribute(string providerConfigurationName)
		{
			_providerConfigurationName = providerConfigurationName;
		}
	}
}
