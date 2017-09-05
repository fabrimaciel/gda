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

namespace GDA.Provider.MsSql
{
	/// <summary>
	/// Possíveis dialetos do provedor do sql server.
	/// </summary>
	public enum MsSqlProviderDialects
	{
		MsSql2000,
		MsSql2005
	}
	/// <summary>
	/// Configuração para conexão com a base de dados SqlServer.
	/// </summary>
	public class MsSqlProviderConfiguration : ProviderConfiguration
	{
		private MsSqlProvider currentProvider = new MsSqlProvider();

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
		/// Dialeto padrão do provedor.
		/// </summary>
		private MsSqlProviderDialects _msSqlDialect = MsSqlProviderDialects.MsSql2000;

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
		/// Gets And Sets o connectionString do provider.
		/// </summary>
		public override string ConnectionString
		{
			get
			{
				if(base.ConnectionString != null && base.ConnectionString != "")
					return base.ConnectionString;
				else
					return String.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3};", _server, _dataBase, _user, _password);
			}
			set
			{
				base.ConnectionString = value;
			}
		}

		/// <summary>
		/// Dialeto do Provider.
		/// </summary>
		public MsSqlProviderDialects MsSqlDialect
		{
			get
			{
				return _msSqlDialect;
			}
			set
			{
				_msSqlDialect = value;
			}
		}

		public override string Dialect
		{
			get
			{
				return MsSqlDialect.ToString();
			}
			set
			{
				try
				{
					MsSqlDialect = (MsSqlProviderDialects)Enum.Parse(typeof(MsSqlProviderDialects), value);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Gets And Sets o provider relacionado.
		/// </summary>
		public override GDA.Interfaces.IProvider Provider
		{
			get
			{
				currentProvider.MsSqlDialect = this.MsSqlDialect;
				return currentProvider;
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="server">Servidor.</param>
		/// <param name="database">Nome da base de dados.</param>
		/// <param name="user">Usuário para acesso.</param>
		/// <param name="password">Senha para acesso.</param>
		public MsSqlProviderConfiguration(string server, string database, string user, string password) : base("", new MsSqlProvider())
		{
			_server = server;
			_dataBase = database;
			_user = user;
			_password = password;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="connectionString">Connection string para conexão com a BD.</param>
		public MsSqlProviderConfiguration(string connectionString) : base(connectionString, new MsSqlProvider())
		{
		}

		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer()
		{
			return new MsSqlAnalyzer(this);
		}
	}
}
