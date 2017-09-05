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

namespace GDA.Provider.MySql
{
	/// <summary>
	/// Configuração para conexão com a base de dados MySql.
	/// </summary>
	public class MySqlProviderConfiguration : ProviderConfiguration, GDA.Interfaces.IProviderConfiguration
	{
		private GDA.Interfaces.IProvider currentProvider = new MySqlProvider();

		/// <summary>
		/// Nome da base de dados.
		/// </summary>
		private string _dataBase;

		/// <summary>
		/// Nome do servidor da base de dados.
		/// </summary>
		private string _server;

		/// <summary>
		/// Usuário para acesso.
		/// </summary>
		private string _user;

		/// <summary>
		/// Senha do usuário
		/// </summary>
		private string _password;

		/// <summary>
		/// Porta de comunicação com o servidor.
		/// </summary>
		private int _port = 3306;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="server">Servidor.</param>
		/// <param name="database">Nome da base de dados.</param>
		/// <param name="user">Usuário para acesso.</param>
		/// <param name="password">Senha para acesso.</param>
		/// <param name="port">Porta de comunicação.</param>
		public MySqlProviderConfiguration(string server, string database, string user, string password, int port) : base("", new MySqlProvider())
		{
			_server = server;
			_dataBase = database;
			_user = user;
			_password = password;
			_port = port;
		}

		public MySqlProviderConfiguration(string connectionString) : base(connectionString, new MySqlProvider())
		{
		}

		/// <summary>
		/// Gets and Sets o nome da base de dados.
		/// </summary>
		public string DataBase
		{
			get
			{
				return _dataBase;
			}
			set
			{
				_dataBase = value;
			}
		}

		/// <summary>
		/// Gets and Sets o nome do servidor da base de dados.
		/// </summary>
		public string Server
		{
			get
			{
				return _server;
			}
			set
			{
				_server = value;
			}
		}

		/// <summary>
		/// Gets and Set o usuário para acesso.
		/// </summary>
		public string User
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		/// <summary>
		/// Gets and Sets a senha do usuário.
		/// </summary>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		/// <summary>
		/// Gets and Sets a porta de comunicação com o servidor.
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}

		public override string ConnectionString
		{
			get
			{
				if(base.ConnectionString != null && base.ConnectionString != "")
					return base.ConnectionString;
				else
					return "Data Source=" + _server + ";Database=" + _dataBase + ";User ID=" + _user + ";Password=" + _password + ";Port=" + _port.ToString();
			}
			set
			{
				base.ConnectionString = value;
			}
		}

		public GDA.Interfaces.IProvider Provider
		{
			get
			{
				return currentProvider;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Analyzer relacionado com o provider.
		/// </summary>
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer()
		{
			return new MySqlAnalyzer(this);
		}
	}
}
