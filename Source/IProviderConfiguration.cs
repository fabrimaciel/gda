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
using System.Data;
using GDA.Analysis;

namespace GDA.Interfaces
{
	/// <summary>
	/// Fornece as informações adicionais para o provider.
	/// </summary>
	public interface IProviderConfiguration
	{
		/// <summary>
		/// Evento acionado quando uma conexão é criada pelo provedor.
		/// </summary>
		event Provider.CreateConnectionEvent ConnectionCreated;

		/// <summary>
		/// Identificador único do provider de configuração.
		/// </summary>
		Guid ProviderIdentifier
		{
			get;
		}

		/// <summary>
		/// Gets And Sets o provider relacionado.
		/// </summary>
		IProvider Provider
		{
			get;
			set;
		}

		/// <summary>
		/// Gets And Sets o connectionString do provider.
		/// </summary>
		string ConnectionString
		{
			get;
			set;
		}

		/// <summary>
		/// Gets And Sets do dialeto que será usado no provider.
		/// </summary>
		string Dialect
		{
			get;
			set;
		}

		/// <summary>
		/// Cria uma conexão com banco de dados já com o connection string configurado.
		/// </summary>
		/// <returns></returns>
		IDbConnection CreateConnection();

		/// <summary>
		/// Analyzer relacionado com o provider.
		/// </summary>
		DatabaseAnalyzer GetDatabaseAnalyzer();
	}
}
