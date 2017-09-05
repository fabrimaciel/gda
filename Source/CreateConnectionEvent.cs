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

namespace GDA.Provider
{
	/// <summary>
	/// Representa o evento que é acionado quando uma conexão é criada no 
	/// provedor de configuração.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void CreateConnectionEvent (object sender, CreateConnectionEventArgs args);
	/// <summary>
	/// Classe que armazena os argumentos que são informados quando
	/// uma conexão do provedor de configuração é criada.
	/// </summary>
	public class CreateConnectionEventArgs : EventArgs
	{
		private System.Data.IDbConnection _connection;

		/// <summary>
		/// Instancia da conexão que foi criada.
		/// </summary>
		public System.Data.IDbConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connection">Instancia da conexão criada.</param>
		public CreateConnectionEventArgs(System.Data.IDbConnection connection)
		{
			_connection = connection;
		}
	}
}
