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

namespace GDA.Interfaces
{
	public interface IPersistenceObjectBase
	{
		/// <summary>
		/// Provider utilizado para conexão com BD.
		/// </summary>
		IProvider UserProvider
		{
			get;
		}

		/// <summary>
		/// Provider de configuração.
		/// </summary>
		IProviderConfiguration Configuration
		{
			get;
		}

		/// <summary>
		/// Cria uma instância de conexão com o BD.
		/// </summary>
		/// <returns>Nova instância de conexão.</returns>
		IDbConnection CreateConnection();

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		/// <param name="parameters">Parametros a serem passados para o comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int ExecuteCommand(string sqlQuery, params GDAParameter[] parameters);

		/// <summary>
		/// Executa comandos sql.
		/// </summary>
		/// <param name="sqlQuery">Causa sql a ser executada.</param>
		/// <returns>Número de linhas afetadas.</returns>
		int ExecuteCommand(string sqlQuery);

		int ExecuteSqlQueryCount(string sqlQuery, params GDAParameter[] parameters);

		object ExecuteScalar(string sqlQuery, params GDAParameter[] parameters);

		object ExecuteScalar(string sqlQuery);
	}
}
