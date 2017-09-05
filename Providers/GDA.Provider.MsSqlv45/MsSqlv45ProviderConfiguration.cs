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
	/// Representa a configuração do provedor do MsSql.
	/// </summary>
	public class MsSqlv45ProviderConfiguration : GDA.Provider.MsSql.MsSqlProviderConfiguration
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connectionString"></param>
		public MsSqlv45ProviderConfiguration(string connectionString) : base(connectionString)
		{
		}

		/// <summary>
		/// Cria a instancia com os parametros iniciais.
		/// </summary>
		/// <param name="server">Servidor.</param>
		/// <param name="database">Nome da base de dados.</param>
		/// <param name="user">Usuário para acesso.</param>
		/// <param name="password">Senha para acesso.</param>
		public MsSqlv45ProviderConfiguration(string server, string database, string user, string password) : base(server, database, user, password)
		{
		}
	}
}
