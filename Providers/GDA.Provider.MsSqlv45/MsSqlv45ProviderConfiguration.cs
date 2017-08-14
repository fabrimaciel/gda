using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GDA.Provider.MsSqlv45
{
	public class MsSqlv45ProviderConfiguration : GDA.Provider.MsSql.MsSqlProviderConfiguration
	{
		public MsSqlv45ProviderConfiguration (string a) : base (a)
		{
		}
		public MsSqlv45ProviderConfiguration (string a, string b, string c, string d) : base (a, b, c, d)
		{
		}
	}
}
