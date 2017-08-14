using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Analysis;
namespace GDA.Provider
{
	public class ProviderConfiguration : GDA.Interfaces.IProviderConfiguration
	{
		private GDA.Interfaces.IProvider _Provider;
		private string _ConnectionString;
		private string _dialect;
		private Guid _providerIdentifier = Guid.NewGuid ();
		public event CreateConnectionEvent ConnectionCreated;
		public Guid ProviderIdentifier {
			get {
				return _providerIdentifier;
			}
		}
		public virtual GDA.Interfaces.IProvider Provider {
			get {
				return _Provider;
			}
			set {
				_Provider = value;
			}
		}
		public virtual string ConnectionString {
			get {
				return _ConnectionString;
			}
			set {
				_ConnectionString = value;
			}
		}
		public virtual string Dialect {
			get {
				return _dialect;
			}
			set {
				_dialect = value;
			}
		}
		public ProviderConfiguration (string a, GDA.Interfaces.IProvider b)
		{
			_ConnectionString = a;
			_Provider = b;
		}
		public virtual IDbConnection CreateConnection ()
		{
			IDbConnection a = Provider.CreateConnection ();
			a.ConnectionString = ConnectionString;
			if (ConnectionCreated != null)
				ConnectionCreated (this, new CreateConnectionEventArgs (a));
			return a;
		}
		public virtual DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			throw new NotImplementedException ();
		}
	}
}
