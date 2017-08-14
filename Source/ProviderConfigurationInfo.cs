using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	class ProviderConfigurationInfo
	{
		public readonly string ConnectionString;
		public readonly string ProviderName;
		public readonly string Name;
		public readonly string Dialect;
		public ProviderConfigurationInfo (string a, string b, string c, string d)
		{
			Name = a;
			ConnectionString = c;
			ProviderName = b;
			Dialect = d;
		}
	}
}
