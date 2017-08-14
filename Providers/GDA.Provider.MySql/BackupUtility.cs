using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace GDA.Provider.MySql
{
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
		public string Username {
			get {
				return _username;
			}
			set {
				if (string.IsNullOrEmpty (value))
					throw new ArgumentNullException ("Username");
				_username = value;
			}
		}
		public string Password {
			get {
				return _password;
			}
			set {
				if (string.IsNullOrEmpty (value))
					throw new ArgumentNullException ("Password");
				_password = value;
			}
		}
		public string Host {
			get {
				return _host;
			}
			set {
				if (string.IsNullOrEmpty (value))
					throw new ArgumentNullException ("Host");
				_host = value;
			}
		}
		public int Port {
			get {
				return _port;
			}
			set {
				_port = value;
			}
		}
		public string DefaultCharacterSet {
			get {
				return _defaultCharacterSet;
			}
			set {
				_defaultCharacterSet = value;
			}
		}
		public string Database {
			get {
				return _database;
			}
			set {
				if (string.IsNullOrEmpty (value))
					throw new ArgumentNullException ("Database");
				_database = value;
			}
		}
		public bool ExportXML {
			get {
				return _exportXML;
			}
			set {
				_exportXML = value;
			}
		}
		public string DestinationFile {
			get {
				return _destinationFile;
			}
			set {
				if (string.IsNullOrEmpty (value))
					throw new ArgumentNullException ("DestinationFile");
				_destinationFile = value;
			}
		}
		public string MysqlDumpPath {
			get {
				return _mysqlDumpPath;
			}
			set {
				_mysqlDumpPath = value;
			}
		}
		public string MysqlPath {
			get {
				return _mysqlPath;
			}
			set {
				_mysqlPath = value;
			}
		}
		public BackupUtility (string a, string b, string c, string d, string e)
		{
			Host = a;
			Username = b;
			Password = c;
			Database = d;
			DestinationFile = e;
		}
		public void Export ()
		{
			if (string.IsNullOrEmpty (_mysqlDumpPath))
				throw new Exception ("MysqlDump not defined.");
			else if (!System.IO.File.Exists (_mysqlDumpPath))
				throw new Exception (string.Format ("MysqlDump file \"{0}\" not exists.", _mysqlDumpPath));
			Exception a = null;
			DataReceivedEventHandler b = new DataReceivedEventHandler (delegate (object c, DataReceivedEventArgs d) {
				if (!string.IsNullOrEmpty (d.Data))
					a = new Exception (d.Data);
			});
			Process e = new Process ();
			string f = "/c " + (_mysqlDumpPath.IndexOf (' ') >= 0 ? "\"" + _mysqlDumpPath + "\"" : _mysqlDumpPath) + " ";
			f += "-u " + _username + " -p" + _password + " -h " + _host + " --default-character-set=" + _defaultCharacterSet + " -P " + _port.ToString () + " -x -e" + (!string.IsNullOrEmpty (_database) ? " -B " + _database : "") + " > " + (_destinationFile.IndexOf (' ') >= 0 ? "\"" + _destinationFile + "\"" : _destinationFile);
			e.StartInfo = new ProcessStartInfo (@"cmd", f);
			e.EnableRaisingEvents = false;
			e.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			e.StartInfo.CreateNoWindow = true;
			e.StartInfo.RedirectStandardError = true;
			e.StartInfo.UseShellExecute = false;
			e.ErrorDataReceived += b;
			e.Start ();
			e.BeginErrorReadLine ();
			e.WaitForExit ();
			e.ErrorDataReceived -= b;
			if (a != null)
				throw a;
		}
		public void Import ()
		{
			if (string.IsNullOrEmpty (_mysqlPath))
				throw new Exception ("Mysql not defined.");
			else if (!System.IO.File.Exists (_mysqlPath))
				throw new Exception (string.Format ("Mysql file \"{0}\" not exists.", _mysqlPath));
			Exception a = null;
			DataReceivedEventHandler b = new DataReceivedEventHandler (delegate (object c, DataReceivedEventArgs d) {
				if (!string.IsNullOrEmpty (d.Data))
					a = new Exception (d.Data);
			});
			Process e = new Process ();
			string f = "/c " + (_mysqlPath.IndexOf (' ') >= 0 ? "\"" + _mysqlPath + "\"" : _mysqlPath) + " ";
			f += "-u " + _username + " -p" + _password + " -h " + _host + " -B " + _database + " -P " + _port.ToString () + " < " + (_destinationFile.IndexOf (' ') >= 0 ? "\"" + _destinationFile + "\"" : _destinationFile);
			e.StartInfo = new ProcessStartInfo (@"cmd", f);
			e.EnableRaisingEvents = false;
			e.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			e.StartInfo.CreateNoWindow = true;
			e.StartInfo.RedirectStandardError = true;
			e.StartInfo.UseShellExecute = false;
			e.ErrorDataReceived += b;
			e.Start ();
			e.BeginErrorReadLine ();
			e.WaitForExit ();
			e.ErrorDataReceived -= b;
			if (a != null)
				throw a;
		}
	}
}
