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
using System.Diagnostics;

namespace GDA.Provider.MySql
{
	/// <summary>
	/// Representa um utilitarios para backup.
	/// </summary>
	public class BackupUtility
	{
		private string _username;

		private string _password;

		private string _host;

		private int _port = 3306;

		private string _defaultCharacterSet = "utf8";

		private string _database;

		private bool _exportXML = false;

		private string _destinationFile;

		private string _mysqlDumpPath;

		private string _mysqlPath;

		/// <summary>
		/// Usuário de acesso.
		/// </summary>
		public string Username
		{
			get
			{
				return _username;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Username");
				_username = value;
			}
		}

		/// <summary>
		/// Senha de acesso.
		/// </summary>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Password");
				_password = value;
			}
		}

		/// <summary>
		/// Host do banco de dados.
		/// </summary>
		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Host");
				_host = value;
			}
		}

		/// <summary>
		/// Porta de comunicação.
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

		public string DefaultCharacterSet
		{
			get
			{
				return _defaultCharacterSet;
			}
			set
			{
				_defaultCharacterSet = value;
			}
		}

		/// <summary>
		/// Base de dados que será importada ou exportada.
		/// </summary>
		public string Database
		{
			get
			{
				return _database;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("Database");
				_database = value;
			}
		}

		/// <summary>
		/// Identifica se será usado o formato XML.
		/// </summary>
		public bool ExportXML
		{
			get
			{
				return _exportXML;
			}
			set
			{
				_exportXML = value;
			}
		}

		/// <summary>
		/// Nome do arquivo de importação ou exportação.
		/// </summary>
		public string DestinationFile
		{
			get
			{
				return _destinationFile;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException("DestinationFile");
				_destinationFile = value;
			}
		}

		/// <summary>
		/// Caminho do Mysqldump.
		/// </summary>
		public string MysqlDumpPath
		{
			get
			{
				return _mysqlDumpPath;
			}
			set
			{
				_mysqlDumpPath = value;
			}
		}

		/// <summary>
		/// Caminho do programa Mysql para importação dos dados.
		/// </summary>
		public string MysqlPath
		{
			get
			{
				return _mysqlPath;
			}
			set
			{
				_mysqlPath = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host">Nome do host onde está o banco de dados.</param>
		/// <param name="username">Usuário de acesso ao banco de dados.</param>
		/// <param name="password">Senha de acesso.</param>
		/// <param name="database">Nome da base de dados.</param>
		/// <param name="destinationFile">Arquivo de destino dos dados importados ou exportados.</param>
		public BackupUtility(string host, string username, string password, string database, string destinationFile)
		{
			Host = host;
			Username = username;
			Password = password;
			Database = database;
			DestinationFile = destinationFile;
		}

		/// <summary>
		/// Exporta os dados.
		/// </summary>
		public void Export()
		{
			if(string.IsNullOrEmpty(_mysqlDumpPath))
				throw new Exception("MysqlDump not defined.");
			else if(!System.IO.File.Exists(_mysqlDumpPath))
				throw new Exception(string.Format("MysqlDump file \"{0}\" not exists.", _mysqlDumpPath));
			Exception exceptionOcurred = null;
			DataReceivedEventHandler dataError = new DataReceivedEventHandler(delegate(object sender, DataReceivedEventArgs e) {
				if(!string.IsNullOrEmpty(e.Data))
					exceptionOcurred = new Exception(e.Data);
			});
			Process p1 = new Process();
			string cmdArgs = "/c " + (_mysqlDumpPath.IndexOf(' ') >= 0 ? "\"" + _mysqlDumpPath + "\"" : _mysqlDumpPath) + " ";
			cmdArgs += "-u " + _username + " -p" + _password + " -h " + _host + " --default-character-set=" + _defaultCharacterSet + " -P " + _port.ToString() + " -x -e" + (!string.IsNullOrEmpty(_database) ? " -B " + _database : "") + " > " + (_destinationFile.IndexOf(' ') >= 0 ? "\"" + _destinationFile + "\"" : _destinationFile);
			p1.StartInfo = new ProcessStartInfo(@"cmd", cmdArgs);
			p1.EnableRaisingEvents = false;
			p1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p1.StartInfo.CreateNoWindow = true;
			p1.StartInfo.RedirectStandardError = true;
			p1.StartInfo.UseShellExecute = false;
			p1.ErrorDataReceived += dataError;
			p1.Start();
			p1.BeginErrorReadLine();
			p1.WaitForExit();
			p1.ErrorDataReceived -= dataError;
			if(exceptionOcurred != null)
				throw exceptionOcurred;
		}

		/// <summary>
		/// Importa os dados.
		/// </summary>
		public void Import()
		{
			if(string.IsNullOrEmpty(_mysqlPath))
				throw new Exception("Mysql not defined.");
			else if(!System.IO.File.Exists(_mysqlPath))
				throw new Exception(string.Format("Mysql file \"{0}\" not exists.", _mysqlPath));
			Exception exceptionOcurred = null;
			DataReceivedEventHandler dataError = new DataReceivedEventHandler(delegate(object sender, DataReceivedEventArgs e) {
				if(!string.IsNullOrEmpty(e.Data))
					exceptionOcurred = new Exception(e.Data);
			});
			Process p1 = new Process();
			string cmdArgs = "/c " + (_mysqlPath.IndexOf(' ') >= 0 ? "\"" + _mysqlPath + "\"" : _mysqlPath) + " ";
			cmdArgs += "-u " + _username + " -p" + _password + " -h " + _host + " -B " + _database + " -P " + _port.ToString() + " < " + (_destinationFile.IndexOf(' ') >= 0 ? "\"" + _destinationFile + "\"" : _destinationFile);
			p1.StartInfo = new ProcessStartInfo(@"cmd", cmdArgs);
			p1.EnableRaisingEvents = false;
			p1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p1.StartInfo.CreateNoWindow = true;
			p1.StartInfo.RedirectStandardError = true;
			p1.StartInfo.UseShellExecute = false;
			p1.ErrorDataReceived += dataError;
			p1.Start();
			p1.BeginErrorReadLine();
			p1.WaitForExit();
			p1.ErrorDataReceived -= dataError;
			if(exceptionOcurred != null)
				throw exceptionOcurred;
		}
	}
}
