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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDA.Provider.MsSqlv45
{
	/// <summary>
	/// Implementação do OleDb para acessar o Sql.
	/// </summary>
	public class OleDbv45Provider : global::GDA.Provider.MsSql.MsSqlProvider
	{
		/// <summary>
		/// Cria o comando do banco de dados.
		/// </summary>
		/// <returns></returns>
		public override System.Data.IDbCommand CreateCommand()
		{
			return new System.Data.OleDb.OleDbCommand();
		}

		/// <summary>
		/// Cria uma conexão para o banco de dados.
		/// </summary>
		/// <returns></returns>
		public override System.Data.IDbConnection CreateConnection()
		{
			return new System.Data.OleDb.OleDbConnection();
		}

		/// <summary>
		/// Cria uma DataAdapter.
		/// </summary>
		/// <returns></returns>
		public override System.Data.IDbDataAdapter CreateDataAdapter()
		{
			return new System.Data.OleDb.OleDbDataAdapter();
		}

		/// <summary>
		/// Cria um parametro.
		/// </summary>
		/// <returns></returns>
		public override System.Data.Common.DbParameter CreateParameter()
		{
			return new System.Data.OleDb.OleDbParameter();
		}
	}
}
