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
	class ProviderConfigurationInfo
	{
		/// <summary>
		/// ConnectionString do provider.
		/// </summary>
		public readonly string ConnectionString;

		/// <summary>
		/// Nome do provider relacionado.
		/// </summary>
		public readonly string ProviderName;

		/// <summary>
		/// Nome do providerConfigurationInfo.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Dialeto que será usado na configuração do provedor.
		/// </summary>
		public readonly string Dialect;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do providerConfigurationInfo.</param>
		/// <param name="providerName">Nome do provider.</param>
		/// <param name="connectionString">ConnectionString do provider.</param>
		/// <param name="dialect">Dialeto que será usado na configuração do provedor.</param>
		public ProviderConfigurationInfo(string name, string providerName, string connectionString, string dialect)
		{
			Name = name;
			ConnectionString = connectionString;
			ProviderName = providerName;
			Dialect = dialect;
		}
	}
}
