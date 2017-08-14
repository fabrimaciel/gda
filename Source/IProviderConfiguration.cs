using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Analysis;
namespace GDA.Interfaces
{
	public interface IProviderConfiguration
	{
		event Provider.CreateConnectionEvent ConnectionCreated;
		Guid ProviderIdentifier {
			get;
		}
		IProvider Provider {
			get;
			set;
		}
		string ConnectionString {
			get;
			set;
		}
		string Dialect {
			get;
			set;
		}
		IDbConnection CreateConnection ();
		DatabaseAnalyzer GetDatabaseAnalyzer ();
	}
}
