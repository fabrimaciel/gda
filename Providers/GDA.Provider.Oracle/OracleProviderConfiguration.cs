using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Provider.Oracle
{
	public class OracleProviderConfiguration : ProviderConfiguration
	{
		public OracleProviderConfiguration (string a) : base (a, new OracleProvider ())
		{
		}
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			return new OracleAnalyzer (this);
		}
	}
}
