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
	/// Implementação da configuração do Provedor OleDb para o MsSql.
	/// </summary>
	public class OleDbv45ProviderConfiguration : global::GDA.Provider.ProviderConfiguration
	{
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
		private global::GDA.Provider.MsSql.MsSqlProviderDialects _msSqlDialect = global::GDA.Provider.MsSql.MsSqlProviderDialects.MsSql2000;

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
		public global::GDA.Provider.MsSql.MsSqlProviderDialects MsSqlDialect
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

		/// <summary>
		/// Dialeto que será utilizado.
		/// </summary>
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
					MsSqlDialect = (global::GDA.Provider.MsSql.MsSqlProviderDialects)Enum.Parse(typeof(global::GDA.Provider.MsSql.MsSqlProviderDialects), value);
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="server">Servidor.</param>
		/// <param name="database">Nome da base de dados.</param>
		/// <param name="user">Usuário para acesso.</param>
		/// <param name="password">Senha para acesso.</param>
		public OleDbv45ProviderConfiguration(string server, string database, string user, string password) : base("", new OleDbv45Provider())
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
		public OleDbv45ProviderConfiguration(string connectionString) : base(connectionString, new OleDbv45Provider())
		{
		}

		/// <summary>
		/// Recupera o analisador de banco de dados.
		/// </summary>
		/// <returns></returns>
		public override global::GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer()
		{
			throw new NotImplementedException();
		}
	}
}
