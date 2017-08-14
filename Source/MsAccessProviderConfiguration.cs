using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Provider.MsAccess
{
	public class MsAccessProviderConfiguration : ProviderConfiguration
	{
		private GDA.Interfaces.IProvider currentProvider = new MsAccessProvider ();
		public override GDA.Interfaces.IProvider Provider {
			get {
				return currentProvider;
			}
			set {
				throw new Exception ("The method or operation is not implemented.");
			}
		}
		public MsAccessProviderConfiguration (string a) : base (a, new MsAccessProvider ())
		{
		}
		public override GDA.Analysis.DatabaseAnalyzer GetDatabaseAnalyzer ()
		{
			return new MsAccessAnalyzer (this);
		}
	}
}
