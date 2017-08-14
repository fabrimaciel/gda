using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Provider.MsSql
{
	public enum MsSqlProviderDialects
	{
		MsSql2000,
		MsSql2005
	}
	public class MsSqlProviderConfiguration : ProviderConfiguration
	{
		private MsSqlProvider currentProvider = new MsSqlProvider ();
		private string _dataBase;
		private string _server;
		private string _user;
		private string _password;
		private MsSqlProviderDialects _msSqlDialect = MsSqlProviderDialects.MsSql2000;
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
		public override string ConnectionString {
			get {
				if (base.ConnectionString != null && base.ConnectionString != "")
					return base.ConnectionString;
				else
					return String.Format ("Data Source={0};Initial Catalog={1};User Id={2};Password={3};", _server, _dataBase, _user, _password);
			}
			set {
				base.ConnectionString = value;
			}
		}
		public MsSqlProviderDialects MsSqlDialect {
			get {
				return _msSqlDialect;
			}
			set {
				_msSqlDialect = value;
			}
		}
		public override string Dialect {
			get {
				return MsSqlDialect.ToString ();
			}
			set {
				try {
					MsSqlDialect = (MsSqlProviderDialects)Enum.Parse (typeof(MsSqlProviderDialects), value);
				}
				catch {
				}
			}
		}
		public override GDA.Interfaces.IProvider Provider {
			get {
				currentProvider.MsSqlDialect = this.MsSqlDialect;
				return currentProvider;
			}
			set {
				throw new Exception ("The method or operation is not implemented.");
			}
		}
		public MsSqlProviderConfiguration (string a, string b, string c, string d) : base ("", new MsSqlProvider ())
		{
			_server = a;
			_dataBase = b;
			_user = c;
			_password = d;
		}
		public MsSqlProviderConfiguration (string a) : base (a, new MsSqlProvider ())
		{
		}
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			return new MsSqlAnalyzer (this);
		}
	}
}
