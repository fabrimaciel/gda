using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GDA.Provider.MsSqlv45
{
	public class OleDbv45ProviderConfiguration : global::GDA.Provider.ProviderConfiguration
	{
		private string _dataBase;
		private string _server;
		private string _user;
		private string _password;
		private global::GDA.Provider.MsSql.MsSqlProviderDialects _msSqlDialect = global::GDA.Provider.MsSql.MsSqlProviderDialects.MsSql2000;
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
		public global::GDA.Provider.MsSql.MsSqlProviderDialects MsSqlDialect {
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
					MsSqlDialect = (global::GDA.Provider.MsSql.MsSqlProviderDialects)Enum.Parse (typeof(global::GDA.Provider.MsSql.MsSqlProviderDialects), value);
				}
				catch {
				}
			}
		}
		public OleDbv45ProviderConfiguration (string a, string b, string c, string d) : base ("", new OleDbv45Provider ())
		{
			_server = a;
			_dataBase = b;
			_user = c;
			_password = d;
		}
		public OleDbv45ProviderConfiguration (string a) : base (a, new OleDbv45Provider ())
		{
		}
		public override global::GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			throw new NotImplementedException ();
		}
	}
}
