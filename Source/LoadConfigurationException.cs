using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Common.Configuration.Exceptions
{
	public class LoadConfigurationException : GDAException
	{
		public LoadConfigurationException (string a) : base (a)
		{
		}
		public LoadConfigurationException (Exception a) : base (a.Message, a)
		{
		}
		public LoadConfigurationException (string a, Exception b) : base (a, b)
		{
		}
		public LoadConfigurationException (string a, params object[] b) : base (a, b)
		{
		}
	}
}
