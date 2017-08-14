using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Provider.MySql
{
	public class MySqlProviderConfiguration : ProviderConfiguration, GDA.Interfaces.IProviderConfiguration
	{
		private GDA.Interfaces.IProvider currentProvider = new MySqlProvider ();
		private string _dataBase;
		private string _server;
		private string _user;
		private string _password;
		private int _port = 3306;
		public MySqlProviderConfiguration (string a, string b, string c, string d, int e) : base ("", new MySqlProvider ())
		{
			_server = a;
			_dataBase = b;
			_user = c;
			_password = d;
			_port = e;
		}
		public MySqlProviderConfiguration (string a) : base (a, new MySqlProvider ())
		{
		}
		public string DataBase {
			get {
				return _dataBase;
			}
			set {
				_dataBase = value;
			}
		}
		public string Server {
			get {
				return _server;
			}
			set {
				_server = value;
			}
		}
		public string User {
			get {
				return _user;
			}
			set {
				_user = value;
			}
		}
		public string Password {
			get {
				return _password;
			}
			set {
				_password = value;
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
		public override string ConnectionString {
			get {
				if (base.ConnectionString != null && base.ConnectionString != "")
					return base.ConnectionString;
				else
					return "Data Source=" + _server + ";Database=" + _dataBase + ";User ID=" + _user + ";Password=" + _password + ";Port=" + _port.ToString ();
			}
			set {
				base.ConnectionString = value;
			}
		}
		public GDA.Interfaces.IProvider Provider {
			get {
				return currentProvider;
			}
			set {
				throw new Exception ("The method or operation is not implemented.");
			}
		}
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			return new MySqlAnalyzer (this);
		}
	}
}
